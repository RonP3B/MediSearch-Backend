# Localization And Error Codes

One of the most important conventions in MediSearch is that client-facing errors are driven by error codes, not by hardcoded strings.

## The Rule

If the client is supposed to read it, it should normally come from an `ErrorCode`.

That applies to:

- FluentValidation failures
- business-rule failures
- not-found responses
- unauthorized/forbidden responses

Hardcoded English strings are acceptable for:

- logs
- internal diagnostics
- developer-only exceptions
- invariant-corruption messages that are not meant to be user-facing

## What an `ErrorCode` Is

`ErrorCode` carries:

- a stable key
- optional named parameters

That means the real contract is the key, not the final text.

Examples of parameterized situations:

- entity not found
- enumeration id/name not found
- length or format errors that need extra details

## Where the Messages Live

Localized resources are JSON files under `Infrastructure/MediSearch.Infrastructure.Localization`.

Current supported UI cultures are:

- `en`
- `es`

The localization store scans resource directories, loads the matching culture file, and resolves the text for each error-code key.

## How Error Localization Happens

The normal flow is:

1. Application/domain code produces an `ErrorCode`.
2. Exceptions or validation failures carry that `ErrorCode`.
3. The Web API exception handler resolves an `IErrorLocalizer`.
4. The localizer picks the current UI culture.
5. The localized template is looked up and parameters are applied.
6. The final localized message is written to the HTTP response.

If a translation is missing, the system logs the missing key and falls back to returning the key itself. That is a safety net, not a desired outcome.

## Validators Must Carry Error Codes

This project is strict about validator output.

Every client-facing validation rule should end up with a custom error code.

Two important helpers:

- `WithCustomErrorCode(...)`
  - attaches the full `ErrorCode` to the FluentValidation failure
- `ValidValueObject(...)`
  - preserves the domain error code when a value object fails validation

`ValidationBehavior` actively checks for missing error codes:

- in development, missing codes are treated as a programming error
- outside development, they are logged as application errors

This protects the API contract from silently drifting into random hardcoded text.

## Error Code Ownership

Error codes are organized close to the domain/application area that owns them.

Typical examples:

- domain-wide shared codes
- application shared codes
- feature-specific codes such as accounts, companies, products, chat, favorites, comments, users

When adding a new client-facing failure:

- first decide which module owns the rule
- add the error code there
- then add translations for every supported language

## Parameterized Errors

Prefer parameters over string interpolation in business code.

Good:

- create an `ErrorCode` with a stable key and named parameters
- let localization templates render the final text

Avoid:

- assembling the final client message directly in the application/domain layer

This keeps translations consistent and makes message wording changeable without touching behavior code.

## What to Do When Adding a New Rule

Checklist:

1. Add the new error-code key in the correct module.
2. Add translations in `en.json` and `es.json`.
3. Attach it with `WithCustomErrorCode(...)` or by returning it from a value object/domain rule.
4. Make sure the rule still fits the domain-vs-application validation split.

## Practical Guidance

Use hardcoded text for:

- logs
- `ExpectedResultNotFoundException`
- `CorruptedInvariantException`
- other developer-facing diagnostics

Use localized error codes for:

- anything the API client or UI should show directly to the end user
