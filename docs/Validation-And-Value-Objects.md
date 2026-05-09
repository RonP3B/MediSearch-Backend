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
