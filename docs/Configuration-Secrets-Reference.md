# Configuration And User Secrets Reference

This document summarizes the configuration currently expected by `MediSearch.Presentation.WebApi` and the infrastructure projects it wires together.

## Appsettings

These values are expected in:

- `src/Presentation/MediSearch.Presentation.WebApi/appsettings.json`
- `src/Presentation/MediSearch.Presentation.WebApi/appsettings.Development.json`

```json
{
  "AccountTokens": {
    "SecretKey": "",
    "EmailConfirmationTokenLifetimeHours": 24,
    "PasswordResetTokenLifetimeHours": 2
  },
  "AllowedHosts": "*",
  "AppUrls": {
    "ApiBaseUrl": "",
    "ApiEmailConfirmationPath": "accounts/email-confirmation?externalUserId={0}&confirmationToken={1}",
    "AssetBaseUrl": "",
    "ClientBaseUrl": "",
    "ClientLoginPath": "login",
    "ClientResetPasswordPath": "auth/reset-password?externalUserId={0}&token={1}",
    "ClientEmailConfirmationResultPath": "auth/email-confirmation-result?status={0}"
  },
  "ConnectionStrings": {
    "MediSearchDb": "",
    "RabbitMQ": "",
    "Redis": ""
  },
  "Cors": {
    "AllowedOrigins": []
  },
  "EmailService": {
    "FromName": "MediSearch",
    "FromEmail": ""
  },
  "Keycloak": {
    "Url": "",
    "Realm": "medisearch",
    "ClientId": "medisearch-api",
    "ClientSecret": ""
  },
  "MessageBus": {
    "PrefetchCount": 16,
    "ConcurrentMessageLimit": 8,
    "RetryCount": 4,
    "RetryInitialIntervalSeconds": 1,
    "RetryMaxIntervalSeconds": 30,
    "RetryIntervalDeltaSeconds": 3,
    "OutboxQueryDelaySeconds": 1,
    "DuplicateDetectionWindowMinutes": 30
  },
  "MediSearchApiFileStorage": {
    "StoragePath": "wwwroot/uploads"
  }
}
```

## User Secrets

These values are expected in user secrets for the Web API project:

```json
{
  "AdminPassword": "",
  "JWT": {
    "AccessTokenSecretKey": "",
    "RefreshTokenSecretKey": "",
    "Issuer": "",
    "Audience": "",
    "AccessTokenExpirationMinutes": 0,
    "RefreshTokenExpirationDays": 0
  },
  "AccountTokens": {
    "SecretKey": ""
  },
  "EmailService": {
    "ApiKey": ""
  }
}
```

`AccountTokens:SecretKey` signs the email confirmation and password reset tokens. It is deliberately separate from the two `JWT` secrets, and it must be at least 32 characters because it is used with HMAC-SHA256.

## What Is Actually Needed for Local Development

When running the solution through the Aspire AppHost in development:

- `AdminPassword` is required
- the full `JWT` section is required
- `AccountTokens:SecretKey` is required
- the `Keycloak` section is required, but development values are already in place (see below)
- `EmailService.ApiKey` is not used because development email goes through MailPit over SMTP
- connection strings are injected automatically by Aspire

You still need sensible `AppUrls`, `Cors`, and `EmailService.FromEmail` values in appsettings because email templates and client-facing links depend on them.

## Appsettings Development Fallbacks

`appsettings.Development.json` contains fallback connection strings for running the Web API directly without the AppHost.

When you run through Aspire:

- PostgreSQL
- RabbitMQ
- Redis
- MailPit
- Keycloak

are provisioned and injected automatically, so those fallback values are ignored.

## Keycloak Settings

The `Keycloak` section tells the Web API which realm holds the accounts and how to authenticate to it.

| Key | Where it comes from in development |
| --- | --- |
| `Keycloak:Url` | Injected by the Aspire AppHost as the `Keycloak__Url` environment variable. `appsettings.Development.json` holds `http://localhost:8080` as the fallback for running the Web API alone. |
| `Keycloak:Realm` | `appsettings.json` - `medisearch`, created by the realm import file. |
| `Keycloak:ClientId` | `appsettings.json` - `medisearch-api`, created by the realm import file. |
| `Keycloak:ClientSecret` | `appsettings.Development.json` - a development-only value that must match `src/Hosting/MediSearch.Hosting.AppHost/Realms/medisearch-realm.json`. Outside development, put it in user secrets or Key Vault. |

The AppHost itself reads two parameters for the Keycloak admin console, defaulted in `src/Hosting/MediSearch.Hosting.AppHost/appsettings.Development.json`:

```json
{
  "Parameters": {
    "keycloak-admin-username": "admin",
    "keycloak-admin-password": "admin"
  }
}
```

Full background is in [Keycloak Identity Provider](Keycloak-Identity-Provider.md).

## Database Initialization Notes

In development, the Web API:

- applies EF Core migrations on startup
- ensures the system administrator exists, both as a Keycloak account and as a domain user row

The seeded administrator username is:

- `administrator`

Its password comes from:

- `AdminPassword`

## Optional Configuration

- `AZURE_KEY_VAULT_ENDPOINT`
  - optional
  - when present, the Web API adds Azure Key Vault configuration on startup

## Non-Development Email

Outside development, the default email implementation is Resend.

That means:

- `EmailService.ApiKey` becomes required
- the sender name/email settings must be valid for the configured provider/domain
