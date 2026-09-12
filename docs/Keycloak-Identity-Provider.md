# Keycloak Identity Provider

MediSearch stores accounts and passwords in [Keycloak](https://www.keycloak.org/) instead of the ASP.NET Core Identity tables it used before.

This doc assumes you have never used Keycloak. It explains the product first, then exactly how MediSearch uses it, then how to run and troubleshoot it.

## What Changed, In One Paragraph

Nothing above the infrastructure layer changed. The API still issues its own JWTs, still owns the login and refresh endpoints, still sends its own emails from its own Razor templates, and still keeps roles and permissions in PostgreSQL. The only thing that moved is where usernames, emails and password hashes live: they used to be rows in `AspNetUsers`, and now they are accounts inside a Keycloak realm. `UserManager<IdentityUser>` was replaced by HTTP calls to Keycloak's Admin REST API.

## Keycloak In One Page

Keycloak is an open source identity and access management server. You run it as a container, it stores its own data (users, credentials, configuration) in its own database, and you talk to it over HTTP.

The concepts you need for this project:

- **Server**  
  One Keycloak process. It has an **admin console** (a web UI) and a set of REST APIs.

- **Realm**  
  An isolated tenant inside the server: its own users, its own clients, its own settings, its own signing keys. MediSearch uses a realm named `medisearch`. The built-in `master` realm is only for administering the server itself — never put application users there.

- **Client**  
  An application that is allowed to talk to a realm. A *public* client (a SPA, a mobile app) cannot keep a secret. A *confidential* client can, because it runs on a server. MediSearch registers one confidential client, `medisearch-api`, which is the Web API itself.

- **Client secret**  
  The password of a confidential client. The Web API sends it whenever it authenticates to Keycloak.

- **Service account**  
  A confidential client can have its own machine user. Turning on service accounts gives the client an identity it can log in as, without any human being involved. That is how the Web API gets permission to manage users.

- **Realm roles vs client roles**  
  Roles defined at realm level apply to the realm; client roles belong to a specific client. Keycloak ships a built-in client called `realm-management` whose client roles (`manage-users`, `view-users`, …) are the permissions for the Admin REST API. MediSearch grants `manage-users` and `view-users` to its service account and nothing else.

- **Token endpoint and grants**  
  `POST /realms/{realm}/protocol/openid-connect/token` is where you exchange credentials for tokens. A *grant* is the kind of exchange:
  - `client_credentials` — "here is my client id and secret, give me a token for my service account". The Web API uses this to call the Admin REST API.
  - `password` (also called *direct access grant*) — "here is a username and password, give me a token for that user". The Web API uses this **only as a password check**; the tokens that come back are thrown away.

- **Admin REST API**  
  `/admin/realms/{realm}/...` — create, read, update and delete users, reset passwords, read credential metadata. Every call needs a bearer token from the service account.

- **Realm import**  
  Keycloak can create a whole realm from a JSON file on first start. That is how this repo ships a ready-to-run realm instead of asking you to click through the admin console.

## What MediSearch Uses Keycloak For — And What It Does Not

Used for:

- storing the account: username, email, "is the email verified", "is the account enabled"
- storing and verifying the password, including the password policy
- brute-force protection on repeated failed password attempts

**Not** used for:

- issuing the tokens the API returns to clients. Those are still signed by `AuthenticationJwtService` with the secrets in the `JWT` configuration section.
- roles and permissions. Those stay in the `roles`, `user_roles`, `permissions` and `role_permissions` tables and are still resolved by `IPermissionService`.
- the login UI. There is no redirect to a Keycloak login page; clients keep posting to the API's own `/login` endpoint.
- sending emails. Keycloak can send its own confirmation and reset emails, but that would mean giving up the project's Razor templates, localization and MailPit/Resend pipeline, so the API keeps sending them.

That combination is deliberate. Keycloak is the credential store; the application is still the authority on identity, business roles and messaging.

## The Domain Boundary

The domain `users` table already had an `external_id` column pointing at the account in the external identity system. That has not changed — only what it points at:

| Before | After |
| --- | --- |
| `users.external_id` = `AspNetUsers.id` (a GUID string produced by Identity) | `users.external_id` = the Keycloak user id (a UUID string) |

Everything that reads `external_id` — the account query services, `ICurrentUser.ExternalId`, the `ExternalUserId` claim, the email links — works unchanged. No domain code was touched.

## Where The Code Lives

Everything is inside `src/Infrastructure/MediSearch.Infrastructure.Security/Authentication`:

```
Authentication/
├── AccountTokens/
│   ├── AccountActionPurposes.cs        purposes an email token can be issued for
│   ├── AccountActionTokenService.cs    issues/validates confirmation and reset tokens
│   └── AccountTokenOptions.cs          secret + lifetimes for those tokens
├── Jwt/                                unchanged: the access/refresh tokens the API returns
├── Keycloak/
│   ├── Contracts/                      the request/response shapes of the Keycloak API
│   ├── KeycloakAccountService.cs       implements the three application ports
│   ├── KeycloakAdminApiClient.cs       user CRUD over the Admin REST API
│   ├── KeycloakAdminTokenProvider.cs   caches the service-account token
│   ├── KeycloakTokenClient.cs          token endpoint: client credentials + password grant
│   ├── KeycloakErrorCodeMap.cs         Keycloak failure -> application error code
│   ├── KeycloakErrorTranslator.cs      error code -> (ErrorKey, ErrorCode) pair
│   └── KeycloakOptions.cs              url, realm, client id, client secret
└── DependencyInjection.Authentication.cs
```

`KeycloakAccountService` implements the same three ports the Identity adapter implemented, so the application layer never learns that anything changed:

- `ICredentialsValidator` — login
- `IAccountManager` — register, delete, email confirmation
- `IAccountPasswordManager` — reset and change password

The only port change in the whole migration is one added method, `IAccountManager.FindExternalUserIdByUsernameOrDefaultAsync`, used by the database initialiser to check whether the seeded administrator already exists in Keycloak.

## How Each Flow Works Now

### Registration

1. A command handler calls `IAccountManager.RegisterUserAsync` through the compensation manager.
2. `POST /admin/realms/medisearch/users` creates the account with `enabled: true` and `emailVerified` set from `IsActive`, plus the password as a non-temporary credential.
3. Keycloak answers `201 Created` with a `Location` header; the new user id is its last segment, and that id becomes `users.external_id`.
4. If the surrounding transaction fails, the existing compensation event calls `DeleteUserAsync`, which issues `DELETE /admin/realms/medisearch/users/{id}`.

The account is created **enabled even when the email is not verified**. That is on purpose: the password grant has to succeed so the API can tell "wrong password" apart from "email not confirmed", exactly like `IdentityUser.EmailConfirmed` allowed before.

The phone number is not copied into Keycloak. It was only ever written to `IdentityUser.PhoneNumber` and never read back; the real value lives in the domain `users` table.

### Login

1. `LoginCommandHandler` loads the user's claims from PostgreSQL (unchanged).
2. `ValidateCredentialsAsync` looks the account up by exact username through the Admin API.
3. It posts a `password` grant to the token endpoint. Success means the password is right; the returned Keycloak tokens are discarded.
4. If the account's email is not verified, it returns `AccountEmailNotConfirmed`, same as before.
5. The handler then mints the application's own access and refresh tokens.

Note that the password grant creates a short Keycloak session per successful check. Those sessions expire on their own (30 minutes idle in the shipped realm) and are not used for anything.

### Refresh

Completely untouched. Refresh tokens are signed and validated by the application, and the claims are rebuilt from PostgreSQL.

### Email confirmation and password reset

Keycloak has no API that gives you a confirmation or reset token — it can only send its own email. So the API issues these tokens itself, in `AccountActionTokenService`:

- a small JWT signed with the `AccountTokens:SecretKey` secret (separate from the login secrets)
- carrying the account id (`sub`), a `purpose` claim, and an expiry
- URL-safe, so it drops straight into the existing email links

The `purpose` claim is what stops a confirmation link from being replayed as a reset link.

Making a reset link **single use** needed one more ingredient. ASP.NET Core Identity used the user's security stamp, which changed whenever the password changed. Keycloak never exposes password hashes, but `GET /admin/realms/{realm}/users/{id}/credentials` does return the password credential's `createdDate`, and that timestamp changes on every reset. The reset token embeds it, and `ResetPasswordAsync` re-reads it before accepting the token — so once a password has actually been changed, older reset links stop working.

Email confirmation links need no stamp: after a successful confirmation `emailVerified` is `true`, and a replay returns `AccountEmailAlreadyConfirmed` just like before.

Confirming an email is `PUT /admin/realms/{realm}/users/{id}` with `{ "emailVerified": true }`. Resetting a password is `PUT .../users/{id}/reset-password`.

### Change password

Keycloak has no "change password using the current one" endpoint. `ChangePasswordAsync` therefore:

1. loads the account to get its username
2. verifies the current password with a `password` grant — a failure returns `PasswordMismatch`, the same code Identity returned
3. sets the new password through `reset-password`

Because step 2 is a real login attempt, repeated wrong current passwords count toward brute-force protection.

### Administrator seeding

`AppDbContextInitialiser` no longer touches `UserManager`. It asks Keycloak whether an account named `administrator` exists, creates it if not (using the `AdminPassword` setting), and then makes sure the matching domain user row exists. Because both Keycloak and PostgreSQL keep their data in persistent volumes, this is safe to run on every startup.

## The Two Families Of Tokens

It is worth being explicit, because three different things in this system are called "token":

| Token | Signed by | Secret | Used for |
| --- | --- | --- | --- |
| Access token | The API | `JWT:AccessTokenSecretKey` | Bearer auth on every request |
| Refresh token | The API | `JWT:RefreshTokenSecretKey` | Getting a new access token |
| Account action token | The API | `AccountTokens:SecretKey` | Email confirmation and password reset links |
| Keycloak tokens | Keycloak | Keycloak's realm keys | Internal only: calling the Admin API, and checking a password |

Keycloak-issued tokens never leave the infrastructure layer.

## Error Mapping

`IdentityResult.Errors` used to give a tidy list of codes. Keycloak is less consistent: some endpoints answer with a message key such as `invalidPasswordMinLengthMessage`, others with English text such as `User exists with same username`.

`KeycloakErrorCodeMap` handles both — message keys first, then text fragments — and maps them onto the existing `AccountErrorCodes`, so API responses keep the same codes and the same localized messages. `KeycloakErrorTranslator` then derives the error key (`Password`, `Email`, `Username`, `Token`, `Account`) from the mapped code.

One small difference: unrecognized failures used to be reported under the key `IdentityResult`. They are now reported under `Account`, with the same `UnknownAccountError` code.

## Configuration

Two new sections. See [Configuration And Secrets Reference](Configuration-Secrets-Reference.md) for the full picture.

```json
{
  "AccountTokens": {
    "SecretKey": "",
    "EmailConfirmationTokenLifetimeHours": 24,
    "PasswordResetTokenLifetimeHours": 2
  },
  "Keycloak": {
    "Url": "",
    "Realm": "medisearch",
    "ClientId": "medisearch-api",
    "ClientSecret": ""
  }
}
```

- `AccountTokens:SecretKey` is a real secret and belongs in user secrets. It must be at least 32 characters, because it signs with HMAC-SHA256.
- `Keycloak:Url` is injected by the Aspire AppHost as the `Keycloak__Url` environment variable. The value in `appsettings.Development.json` is only the fallback for running the Web API without the AppHost.
- `Keycloak:ClientSecret` has a development value in `appsettings.Development.json` that matches the realm import file. Outside development it belongs in user secrets or Key Vault.

Startup fails fast with an explicit message if any of these is missing.

## Running It Locally

`dotnet run --project src/Hosting/MediSearch.Hosting.AppHost` now also starts a Keycloak container:

- image `quay.io/keycloak/keycloak:26.7.3`
- fixed port `8080`, so the admin console is always at <http://localhost:8080>
- a persistent data volume, so accounts survive restarts
- `ContainerLifetime.Persistent`, like the other infrastructure containers
- the realm imported from `src/Hosting/MediSearch.Hosting.AppHost/Realms/medisearch-realm.json`

Admin console credentials come from the AppHost parameters in `appsettings.Development.json` (`admin` / `admin` by default). Sign in, switch the realm selector from `master` to `medisearch`, and you can browse **Users**, **Clients** and **Realm settings**.

Useful places in the console:

- **Users** — every registered account. The **Credentials** tab can reset a password; the **Details** tab shows *Email verified*.
- **Clients → medisearch-api → Credentials** — the client secret.
- **Clients → medisearch-api → Service accounts roles** — the `realm-management` roles the API was granted.
- **Realm settings → Sessions / Security defenses** — session lifetimes and brute-force settings.

### The realm import only runs once

Keycloak imports a realm when it does not already exist. Because the data volume is persistent, editing `medisearch-realm.json` after the first run changes nothing. To pick up changes, either apply them by hand in the admin console, or delete the Keycloak volume and let Aspire recreate it (which also deletes every local account, so the domain `users` rows will point at ids that no longer exist — clear the database too).

## What The Realm File Configures, And Why

| Setting | Value | Why |
| --- | --- | --- |
| `registrationAllowed`, `resetPasswordAllowed`, `editUsernameAllowed`, `rememberMe` | `false` | The API owns every account flow; Keycloak's self-service pages stay closed. |
| `verifyEmail` | `false` | Otherwise Keycloak adds a `VERIFY_EMAIL` required action that breaks the password grant. The API decides what "confirmed" means. |
| `loginWithEmailAllowed` | `false` | Credentials are always checked by username, matching the old `FindByNameAsync` behaviour. |
| `duplicateEmailsAllowed` | `false` | Keeps the `DuplicateEmail` error working. |
| `passwordPolicy` | length 8–128, upper, lower, digit, special | Mirrors `SharedValidationExtensions.Password()` so Keycloak rejects what the validators reject. |
| `bruteForceProtected` | `true` | Temporary lockout after 30 failures. |
| user profile: `firstName` / `lastName` | not required | MediSearch keeps names in the domain `users` table and never sends them to Keycloak. Keycloak's default profile marks them required, which leaves every account "incomplete" and makes the password grant fail with *Account is not fully set up*, no matter how correct the password is. |
| `medisearch-api` client | confidential, direct access grants + service accounts on, standard flow off | The API needs exactly two things: a machine identity for the Admin API and the ability to check a password. It never performs a browser login. |
| service account roles | `manage-users`, `view-users` | Least privilege for the Admin REST API. |

## Production Checklist

The shipped realm is tuned for local development. Before running this anywhere real:

- generate a new client secret and store it in Key Vault or user secrets, never in `appsettings.Development.json`
- generate a strong `AccountTokens:SecretKey`
- change the Keycloak admin credentials from `admin` / `admin`
- run Keycloak behind HTTPS and set `sslRequired` to `all`
- give Keycloak a real database instead of the dev-mode file store, and back it up — it is now the system of record for credentials
- review the brute-force and session settings for your traffic

## Migrating Existing Accounts

Password hashes cannot be moved from ASP.NET Core Identity to Keycloak: the formats are different and hashes cannot be converted. For an existing database you have two realistic options:

1. **Re-create accounts and force a reset.** Create a Keycloak user per `AspNetUsers` row, update `users.external_id` to the new Keycloak id, and send everyone a password reset email.
2. **Hash-on-first-login.** Write a custom Keycloak credential provider that can verify the old Identity hash once and then re-hash with Keycloak's algorithm. More work, but invisible to users.

The migration in this repository does neither — it drops the tables, on the assumption that the local database is disposable. Do not run it against production data without picking one of the strategies above first.

## Troubleshooting

| Symptom | Likely cause |
| --- | --- |
| Startup throws `Configuration key 'Keycloak' is missing or empty` | The `Keycloak` section or one of its values is not set. |
| Startup throws `Configuration key 'AccountTokens:SecretKey' is missing` | Add the secret to user secrets. |
| `Keycloak rejected the credentials of client 'medisearch-api'` | `Keycloak:ClientSecret` does not match the secret in the realm. Check **Clients → medisearch-api → Credentials**. |
| Admin API calls fail with 403 | The service account lost its `realm-management` roles. Check **Service accounts roles**. |
| Every login fails with invalid credentials right after a reset | The realm was re-imported into a fresh volume while the database kept the old `external_id` values. |
| Login fails for a user who exists, with no obvious reason | Brute-force lockout. Check **Users → the user → the lockout state**, or **Realm settings → Security defenses**. |
| `unauthorized_client` when checking a password | *Direct access grants* got turned off on the client. |
| Login always fails, and Keycloak's `error_description` says `Account is not fully set up` | The realm's user profile marks an attribute required that MediSearch never sends (typically `firstName` / `lastName`). Clear `required` on it under **Realm settings → User profile**. |

## Related Docs

- [Authorization And Identity Flow](Authorization-And-Identity-Flow.md)
- [Configuration And Secrets Reference](Configuration-Secrets-Reference.md)
- [Compensations And External Consistency](Compensations-And-External-Consistency.md)
