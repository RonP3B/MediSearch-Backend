# Exceptions And Error Responses

MediSearch is intentionally exception-friendly.

That does not mean "throw everywhere without structure." It means the application uses a small set of meaningful exceptions as part of the normal error flow, and the Web API translates those exceptions into clean HTTP responses.

## Why This Project Uses Exceptions

Internally, exceptions keep the happy path readable:

- handlers can focus on the use case instead of threading result objects through every call
- the domain can defend invariants immediately
- cross-cutting error translation is centralized in one place

Results still exist at some infrastructure boundaries, especially around external identity/auth operations, but those results are usually translated into application exceptions as soon as they cross into the application flow.

## Main Exception Types

### `ValidationException`

Use when:

- request validation found one or more client-fixable issues
- an external service returned client-fixable validation-style errors that should be surfaced like input errors

HTTP result:

- `400 Bad Request`
- response body is a `ValidationProblemDetails`

### `BusinessRuleException`

Use when:

- the request passed application validation
- but a domain rule rejects the attempted state transition

Examples:

- trying to create an invalid aggregate state
- violating entity rules such as "cannot favorite itself" or "cannot reply to a reply"

HTTP result:

- `400 Bad Request`
- also rendered as validation-style errors keyed by property name

### `NotFoundException`

Use when:

- the client asked for a resource that is legitimately missing
- the absence is expected and should become a 404

HTTP result:

- `404 Not Found`

### `UnauthorizedException`

Use when:

- the caller is not authenticated
- or an auth flow fails in a way that should be expressed as 401

HTTP result:

- `401 Unauthorized`

### `ForbiddenAccessException`

Use when:

- the caller is authenticated
- but is not allowed to perform the action

This includes both permission failures and ownership/scope failures discovered inside handlers.

HTTP result:

- `403 Forbidden`

### `CorruptedInvariantException`

Use when:

- the system reaches a state that should be impossible if our own invariants are intact
- retrying will not fix it

Examples:

- a company user without company data where one is guaranteed
- a chat room without exactly two distinct participants

This is not a client error. It signals a broken internal invariant.

### `ExpectedResultNotFoundException`

Use in repositories/query services when:

- a method contract says the result must exist
- but the underlying data is missing

This is intentionally different from `NotFoundException`.

- `NotFoundException` is for client-facing 404 cases.
- `ExpectedResultNotFoundException` is for internal "this should have existed" cases.

It is also excluded from MassTransit retries because retries will not repair missing required data.

## HTTP Translation

The Web API does not scatter error translation across endpoints.

Instead, `CustomExceptionHandler` converts known exceptions into:

- localized `ValidationProblemDetails` for validation/business-rule failures
- localized `ProblemDetails` for not found, unauthorized, and forbidden
- a generic 500 in non-development for unrecognized exceptions

This keeps the endpoint layer thin and consistent.

## Important Distinction: Client Error vs Bug

There are two very different "bad" situations:

- the client sent something invalid
- the code reached a state that should never happen

In MediSearch, they are represented differently on purpose.

Client-fixable failures:

- `ValidationException`
- `BusinessRuleException`
- `NotFoundException`
- `UnauthorizedException`
- `ForbiddenAccessException`

Internal bug or broken invariant signals:

- `CorruptedInvariantException`
- `InvalidValueObjectStateException<T>` after validated input
- `ExpectedResultNotFoundException` from guaranteed lookup paths

Do not collapse these categories into one generic exception type. The distinction is part of the design.

## Practical Guidance

When writing handlers:

- throw `NotFoundException` for expected user-facing missing resources
- throw `ForbiddenAccessException` for scope/ownership violations
- let domain entities throw `BusinessRuleException`
- translate boundary `ServiceResult` failures into the appropriate application exception as soon as possible

When writing repositories/query services:

- use `OrDefault` methods for nullable lookups
- throw `ExpectedResultNotFoundException` only from methods that promise data exists

When writing endpoints:

- keep them thin
- do not manually build ad hoc error responses for these cases
- let the exception handler own the HTTP contract

## Visual Studio Debugger Setup
 
All five application exceptions (`ValidationException`, `BusinessRuleException`, `NotFoundException`, `UnauthorizedException`, `ForbiddenAccessException`) are part of the normal error flow. They are thrown intentionally and handled by `CustomExceptionHandler`. The debugger should not stop on them during local development.
 
Go to `Debug → Exception Settings`, find each of these exception types, and uncheck **Break when this exception type is user-unhandled**. Without this, the debugger will stop on every expected validation or not-found result as if it were a crash.