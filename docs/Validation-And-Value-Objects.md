# Validation And Value Objects

MediSearch deliberately splits validation responsibilities between the application layer and the domain layer.

## The Core Rule

Domain invariants belong to the domain, not to FluentValidation.

That means:

- Value objects decide whether a value is valid.
- Entities decide whether a state transition is valid.
- Application validators only make requests safe to execute and pleasant to report back to clients.

This avoids duplicating domain rules in validators and keeps the domain as the single source of truth.

## `TryFrom(...)` vs `From(...)`

Value objects follow a two-path construction pattern:

- `TryFrom(...)`
  - Returns `Result<TValueObject>`.
  - Collects every discovered `DomainFailure`.
  - Used by validators and any place where we want to report all client-facing failures cleanly.
- `From(...)`
  - Builds the value object directly.
  - Throws `InvalidValueObjectStateException<T>` if the value is invalid.
  - Used only after the application has already established that the value is valid.

Practical meaning:

- Validators use `TryFrom(...)`.
- Handlers use `From(...)`.

If a handler calls `From(...)` after the request has passed validation and it still throws, we treat that as a bug or a broken invariant, not as a normal client mistake.

## What Application Validators Do

Application validators are still important, but their job is different:

- validate required fields and generic request shape
- validate files, sizes, and transport-level concerns
- validate existence checks and uniqueness checks
- validate business eligibility checks that depend on persistence or the current user
- validate that request input can be converted into the required domain value objects

What they should not do is reimplement value-object rules inline.

## `ValidValueObject(...)`

`ValidValueObject(...)` is an application-layer validation helper.

It is not a separate architectural concern by itself. It is simply the small adapter we use so FluentValidation can reuse value-object `TryFrom(...)` rules cleanly.

It:

- calls a value object's `TryFrom(...)`
- reads all returned `DomainFailure` values
- converts those failures into FluentValidation failures
- preserves the domain `ErrorCode` in `CustomState`

This is what lets the application reuse domain invariants without rewriting them.

## Single-Field and Multi-Field Value Objects

Single-field value objects are straightforward:

- the validator usually looks like `RuleFor(v => v.SomeProperty)`
- the property name already exists in the rule

Multi-field value objects are the tricky case:

- the validator usually needs `RuleFor(v => v)`
- there is no single request property name to attach to the rule
- the value object's own failure property names become important

For those cases, `ValidValueObject(...)` supports a `prefix` so the failures can be reported under meaningful request paths instead of under an empty or generic key.

Examples in the codebase:

- `FullName.TryFrom(firstName, lastName)`
- `Location.TryFrom(province, municipality, address)`
- `Price.TryFrom(amount, currency)`
- `Agent.TryFrom(agentTypeId, agentId)`

This is why some validators validate the whole command object rather than a single field.

## Normalized Values

Some values must compare case-insensitively — two usernames, company names or emails that differ only in casing are the same value and must not both exist.

The wrong way to get that is to lower-case the value on the way in. It works for uniqueness, and it silently destroys what the user typed: a company called "MediSearch Labs" comes back out of the database as "medisearch labs" and is rendered that way everywhere.

So a value object that needs case-insensitive comparison keeps both forms:

- `Value` (or `Address`, for `Email`) holds exactly what the user typed, trimmed
- `Normalized` holds the lower-cased form
- `GetEqualityComponents()` yields `Normalized`, so equality is case-insensitive

The value objects that currently do this are `Username`, `Email`, `CompanyName`, `ProductName`, and the product classification `Name`.

A value object with **no** uniqueness rule does not get a `Normalized`, and it must not lower-case anything either — `FullName` and `CompanyCeoName` are display-only and keep the user's casing as-is.

### How they are mapped

A normalized value object maps to two columns through a complex property, not a value converter:

```csharp
builder.ComplexProperty(
    c => c.Name,
    name =>
    {
        name.Property(p => p.Value).HasColumnName("name").HasMaxLength(100);
        name.Property(p => p.Normalized).HasColumnName("normalized_name").HasMaxLength(100);
    }
);
```

### How they are indexed

EF Core cannot build an index over a member of a complex property, so the unique index on the normalized column is **created by hand in a migration**, and the entity configuration carries a comment saying so. `ix_users_normalized_username` was the first of these; the rest were added alongside it.

Uniqueness checks in repositories must compare the normalized forms, not the value objects:

```csharp
return await dbContext.Companies.AnyAsync(
    company => company.Name.Normalized == companyName.Normalized,
    cancellationToken
);
```

Read-side SQL follows the same split: filter and sort on `normalized_*`, select the original column for display.

## Why `DependentRules(...)` Matters

You will see many rules structured like this:

- first, validate the value object with `TryFrom(...)`
- only then run follow-up rules that need the domain object to be valid

This matters because later rules often call `From(...)` safely.

Typical examples:

- uniqueness checks that convert strings into `Username`, `Email`, `CompanyName`, or `ProductName`
- relationship checks that convert GUIDs into `EntityId<T>`

Without `DependentRules(...)`, a follow-up rule could call `From(...)` too early and turn a normal client error into an exception.

## Error Aggregation

Validation is intentionally aggregated instead of failing fast.

The flow is:

1. Every validator runs.
2. All FluentValidation failures are collected.
3. `ValidationBehavior` converts them into a single application `ValidationException`.
4. The exception reaches the Web API exception handler.
5. The response becomes a structured HTTP 400 payload.

This is why `TryFrom(...)` returning all failures is so valuable: the client gets the complete list in one response.

## Mandatory Error Codes

Every client-facing validation failure must carry an error code.

In this project:

- `WithCustomErrorCode(...)` stores the full `ErrorCode`
- `ValidationBehavior` checks for missing error codes
- in development, missing error codes are treated as a programming error
- in non-development, they are logged loudly

So when adding validators, do not stop at "the rule works." Make sure the rule also produces the correct error code.

## Practical Guidance

When adding a new request field:

- create or reuse a value object if the field carries a domain invariant
- validate it with `ValidValueObject(...)`
- only call `From(...)` in the handler after validation has guaranteed safety

When adding a new multi-field concept:

- prefer modeling it as a value object
- validate it at the object level with `RuleFor(v => v)`
- use a prefix so the resulting property names are still meaningful to the client

When adding generic request-only checks:

- keep them in the validator
- but still attach proper error codes
