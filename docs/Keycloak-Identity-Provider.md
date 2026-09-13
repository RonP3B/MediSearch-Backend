# Keycloak Identity Provider

This document describes how MediSearch uses Keycloak:

- what Keycloak owns and what the application still owns
- how each account flow works against it
- how the realm is provisioned and why it is configured the way it is

## Overview

Accounts and credentials live in a Keycloak realm instead of the ASP.NET Core Identity tables.

Keycloak owns:

- the username, the email, and whether the email is verified
- the password hash and the password policy
- brute-force protection on repeated failed attempts

The application still owns:

- the access and refresh tokens returned to clients
- roles, permissions, and every business fact about a user
- the login and account endpoints
- the account emails and their templates

The boundary itself did not move. `users.external_id` still points at the account in the external identity system. It now holds a Keycloak user id instead of an `AspNetUsers` id.

## Keycloak Concepts Used Here

Realm:

- an isolated tenant inside the Keycloak server, with its own users, clients, settings, and signing keys
- MediSearch uses the `medisearch` realm
- the built-in `master` realm exists only to administer the server and holds no application users

Client:

- an application allowed to talk to a realm
- `medisearch-api` is a confidential client: it holds a secret and authenticates as itself

Service account:

- a machine identity attached to a confidential client
- it authorizes the Admin REST API calls and carries only `manage-users` and `view-users`

Grants:

- `client_credentials` authenticates the Web API itself
- `password`, the direct access grant, is the only supported way to ask Keycloak whether a password is correct

Realm import:

- a realm export Keycloak applies when the realm does not already exist
- `Realms/medisearch-realm.json` creates the realm, the client, and the service-account roles

## The Port Boundary

`KeycloakAccountService` implements the same three application ports the ASP.NET Core Identity adapter implemented:

- `ICredentialsValidator` for login
- `IAccountManager` for registration, deletion, and email confirmation
- `IAccountPasswordManager` for password reset and change

No command handler, DTO, validator, or endpoint changed. The single port change is one added method, `IAccountManager.FindExternalUserIdByUsernameOrDefaultAsync`, used by the database initialiser to decide whether the seeded administrator already exists.

## Where The Code Lives

Everything sits in `src/Infrastructure/MediSearch.Infrastructure.Security/Authentication`:

```
Authentication/
├── AccountTokens/
│   ├── AccountActionPurposes.cs        purposes an email token can be issued for
│   ├── AccountActionTokenService.cs    issues and validates those tokens
│   └── AccountTokenOptions.cs          secret and lifetimes
├── Jwt/                                unchanged: the tokens the API returns
├── Keycloak/
│   ├── Contracts/                      request and response shapes
│   ├── KeycloakAccountService.cs       implements the three ports
│   ├── KeycloakAdminApiClient.cs       user CRUD over the Admin REST API
│   ├── KeycloakAdminTokenProvider.cs   caches the service-account token
│   ├── KeycloakTokenClient.cs          client-credentials and password grants
│   ├── KeycloakErrorCodeMap.cs         Keycloak failure to application error code
│   ├── KeycloakErrorTranslator.cs      error code to (ErrorKey, ErrorCode)
│   └── KeycloakOptions.cs              url, realm, client id, client secret
└── DependencyInjection.Authentication.cs
```

## Registration

`RegisterUserAsync` posts the account to `/admin/realms/{realm}/users` with the password as a non-temporary credential. Keycloak answers `201 Created`, and the new user id is the last segment of the `Location` header. That id becomes `users.external_id`.

The account is created enabled even when the email is not yet verified. The password grant must succeed before confirmation so that credential validation can distinguish a wrong password from an unconfirmed email, which is what `IdentityUser.EmailConfirmed` allowed before.

The phone number is not copied into Keycloak. It was only ever written to `IdentityUser.PhoneNumber` and never read back; the domain `users` table holds the real value.

Registration runs through the compensation manager, so a failure after the account exists publishes the compensation that deletes it. See [Compensations And External Consistency](Compensations-And-External-Consistency.md).

## Credential Validation

`ValidateCredentialsAsync`:

1. looks the account up by exact username through the Admin REST API
2. posts a `password` grant to the token endpoint
3. returns `AccountEmailNotConfirmed` when the account exists and the password is right but the email is not verified

The tokens Keycloak returns are discarded. They exist only to prove the password was accepted, and each successful check opens a short Keycloak session that expires on its own.

Anything that is not a credential rejection is raised as an exception rather than reported as a wrong password, so an unreachable or misconfigured Keycloak never surfaces to the caller as invalid credentials.

## Email Confirmation And Password Reset

Keycloak exposes no API that returns a confirmation or reset token. It can only send its own email, which would mean giving up the project's Razor templates, localization, and the MailPit/Resend pipeline. The application therefore signs its own tokens in `AccountActionTokenService`:

- a small JWT signed with `AccountTokens:SecretKey`, separate from the login secrets
- carrying the account id, a `purpose` claim, and an expiry
- base64url encoded, so it drops into the existing email links unchanged

The `purpose` claim is what prevents a confirmation link from being replayed as a reset link.

Single use is handled differently for the two flows:

- a reset token embeds the password credential's `createdDate`, which Keycloak exposes and which changes on every reset. `ResetPasswordAsync` re-reads it before accepting the token, so older links stop working once the password actually changes. This replaces the ASP.NET Core Identity security stamp.
- a confirmation token needs no stamp. After a successful confirmation `emailVerified` is `true`, and a replay returns `AccountEmailAlreadyConfirmed`.

Confirming an email is a full user update with `emailVerified` set. Partial updates are not safe: since the declarative user profile became mandatory, fields missing from the representation can be cleared rather than left alone.

## Change Password

Keycloak has no endpoint that changes a password using the current one. `ChangePasswordAsync` verifies the current password through the password grant, then sets the new one through `reset-password`. A failed verification returns `PasswordMismatch`, the same code ASP.NET Core Identity returned.

Because the verification is a real login attempt, repeated wrong current passwords count toward brute-force protection.

## Administrator Seeding

`AppDbContextInitialiser` asks Keycloak whether an account named `administrator` exists, creates it from the `AdminPassword` setting if not, and then ensures the matching domain user row. Both stores are persistent, so the check runs safely on every startup.

## Token Vocabulary

Four different things in this system are called a token:

- the access token, signed by the API with `JWT:AccessTokenSecretKey`, used for bearer authentication
- the refresh token, signed by the API with `JWT:RefreshTokenSecretKey`
- the account action token, signed by the API with `AccountTokens:SecretKey`, carried in confirmation and reset links
- Keycloak's own tokens, which never leave the infrastructure layer

## Error Translation

`IdentityResult.Errors` produced a predictable list of codes. Keycloak is less consistent: some endpoints answer with a message key such as `invalidPasswordMinLengthMessage`, others with English text such as `User exists with same username`.

`KeycloakErrorCodeMap` handles both, message keys first and text fragments as a fallback, and maps them onto the existing `AccountErrorCodes`. API responses therefore keep the same codes and the same localized messages. `KeycloakErrorTranslator` then derives the error key from the mapped code.

One behaviour changed: unrecognized failures were previously reported under the key `IdentityResult` and are now reported under `Account`, with the same `UnknownAccountError` code.

## Realm Configuration

The realm import encodes the following decisions:

- `registrationAllowed`, `resetPasswordAllowed`, `editUsernameAllowed`, and `rememberMe` are off, because the application owns every account flow
- `verifyEmail` is off, because Keycloak would otherwise attach a `VERIFY_EMAIL` required action that breaks the password grant
- `loginWithEmailAllowed` is off, so credentials are always checked by username, matching the previous `FindByNameAsync` behaviour
- `duplicateEmailsAllowed` is off, which keeps the `DuplicateEmail` error meaningful
- the password policy mirrors `SharedValidationExtensions.Password()`, so Keycloak rejects what the application validators reject
- `bruteForceProtected` is on, with temporary lockout
- `firstName` and `lastName` are not required in the user profile. Keycloak marks them required by default, the application keeps names in the domain `users` table and never sends them, and the mismatch makes every password grant fail with `Account is not fully set up`
- the client has direct access grants and service accounts enabled and standard flow disabled, because the API never performs a browser login

## Local Development

The Aspire AppHost starts Keycloak alongside the other infrastructure:

- image `quay.io/keycloak/keycloak:26.7.3`
- fixed port `8080`, so the admin console is always at <http://localhost:8080>
- a persistent data volume and `ContainerLifetime.Persistent`
- the realm imported from `src/Hosting/MediSearch.Hosting.AppHost/Realms/medisearch-realm.json`

Admin console credentials come from the AppHost parameters in its `appsettings.Development.json`.

The console shows one realm at a time and opens on `master`. Application users are under `medisearch`, reachable through the realm selector. Service accounts are hidden from the user list by default.

## Realm Import Is Applied Once

Keycloak imports a realm only when it does not already exist. Because the data volume is persistent, editing `medisearch-realm.json` after the first run has no effect.

To pick up a change, either apply it by hand in the admin console, or delete the Keycloak volume and let Aspire recreate it. Deleting the volume also deletes every local account, which leaves `users.external_id` pointing at ids that no longer exist, so the database has to be recreated as well.

## Migrating Existing Accounts

Password hashes cannot be moved from ASP.NET Core Identity to Keycloak. The formats differ and hashes cannot be converted.

For a database that already holds accounts there are two realistic options:

- create a Keycloak user per `AspNetUsers` row, update `users.external_id`, and force a password reset for everyone
- write a Keycloak credential provider that verifies the old hash once and re-hashes with Keycloak's algorithm

The migration in this repository does neither. It drops the tables, on the assumption that the local database is disposable.

## Production Checklist

The shipped realm is tuned for local development. Before running it anywhere real:

- generate a new client secret and keep it in a secret store
- generate a strong `AccountTokens:SecretKey`
- change the Keycloak admin credentials
- run Keycloak behind HTTPS and set `sslRequired` to `all`
- give Keycloak a real database rather than the dev-mode store, and back it up, because it is the system of record for credentials
- review the brute-force and session settings

## Troubleshooting

`Configuration key 'Keycloak' is missing or empty`:

- the section or one of its values is not set

`Keycloak rejected the credentials of client 'medisearch-api'`:

- `Keycloak:ClientSecret` does not match the secret in the realm

Admin calls fail with 403:

- the service account lost its `realm-management` roles

Login always fails and Keycloak reports `Account is not fully set up`:

- the user profile marks an attribute required that the application never sends, typically `firstName` or `lastName`

Login fails with `unauthorized_client`:

- direct access grants are disabled on the client

Every login fails immediately after recreating the realm:

- the realm was imported into a fresh volume while the database kept the old `external_id` values

A user that exists cannot log in for no apparent reason:

- brute-force lockout

## Related Docs

- [Authorization And Identity Flow](Authorization-And-Identity-Flow.md)
- [Configuration And Secrets Reference](Configuration-Secrets-Reference.md)
- [Compensations And External Consistency](Compensations-And-External-Consistency.md)
