# Repositories And Query Services

This document explains two related conventions:

- why MediSearch separates repositories from query services
- what `OrDefault` means in method names

## Write Side vs Read Side

The solution uses two data-access styles on purpose.

Repositories:

- live on the write side
- return domain entities/aggregates
- are usually EF Core based
- are used by command handlers and domain-centric workflows

Query services:

- live on the read side
- return DTOs
- are usually Dapper based
- are used by query handlers and read-optimized workflows

This split keeps commands focused on business state changes and queries focused on efficient read models.

## The `OrDefault` Contract

If a repository or query-service method name contains `OrDefault`, it means:

- missing data is an expected outcome
- returning `null` is part of the contract
- the caller must decide what "missing" means

Typical follow-up choices by the caller:

- convert it to `NotFoundException`
- treat it as "not logged in", "not favorited", or "not applicable"
- branch into a different flow

Examples:

- `GetByIdOrDefaultAsync(...)`
- `GetUserClaimsByUsernameOrDefaultAsync(...)`
- `GetCompanyDetailsByIdOrDefaultAsync(...)`

## Methods Without `OrDefault`

If a repository or query-service method does not contain `OrDefault`, and it is not a list-returning method, the method is declaring stronger semantics:

- the data is expected to exist
- calling code is supposed to use it only when that expectation is already established
- if the data is missing, the method throws

In query services, that throw is normally `ExpectedResultNotFoundException`.

This is not a stylistic detail. It is a contract signal.

## Why This Convention Exists

The naming convention makes intent visible at the call site.

You can often tell the expected control flow just from the method name:

- `...OrDefaultAsync(...)`
  - "I am not sure the data exists."
- `...Async(...)`
  - "At this point, the data must exist. If not, something is wrong with the surrounding assumptions."

That clarity matters a lot in a codebase that mixes:

- client-facing 404 cases
- internal invariant assumptions
- asynchronous event handlers where retries should not hide missing guaranteed data

## The Exception for Lists

List-returning methods are the special case.

They usually do not need `OrDefault` because:

- an empty list is already a natural "no rows" result
- `null` would add ambiguity without value

So the rule of thumb is:

- single expected item: use `OrDefault` if nullable, omit it if guaranteed
- list: return an empty list when there is nothing

## How Callers Should Use the Contract

Use `OrDefault` methods when:

- the request may legitimately reference missing data
- the missing case needs explicit caller handling

Use guaranteed methods when:

- previous validation or flow guarantees the data exists
- or the method itself represents an invariant-based read model

Do not call guaranteed methods just to save a null-check. That turns a normal business absence into an internal failure.

## Why `ExpectedResultNotFoundException` Matters

Guaranteed query-service methods throw `ExpectedResultNotFoundException` when their contract is broken.

That exception is intentionally treated as non-transient in messaging flows:

- retrying will not create the missing data
- the message should go to the error queue instead of retrying forever

So when you choose between `OrDefault` and non-`OrDefault`, you are also choosing how the rest of the system interprets missing data.

## Practical Guidance

When adding a new lookup method:

- ask first whether absence is a valid, expected outcome
- if yes, use `OrDefault`
- if no, make it guaranteed and throw if the expectation is violated

When reviewing code:

- if you see a nullable lookup without `OrDefault`, question it
- if you see a guaranteed lookup used before existence was established, question that too
