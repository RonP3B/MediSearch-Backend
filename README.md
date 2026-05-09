# MediSearch

> 🏥 A healthcare marketplace API where clients, pharmacies, and laboratories can discover, connect, chat, and manage business activity in one place.

MediSearch powers the backend side of a platform built around real business relationships in the medical supply space. Clients browse pharmacies, pharmacies interact with clients and laboratories, laboratories supply pharmacies, and company users can operate on behalf of their company in key features like chat and favorites.

This is not just a generic CRUD API with medical names on top. A lot of the project is shaped by business rules, role-based behavior, company representation, and the need to keep external side effects consistent when things fail.

## ✨ What MediSearch Covers

- 🛍️ Product catalog, classifications, categories, and company product listings
- 💬 Realtime chat between the actors that are actually allowed to interact
- ⭐ Favorites for products and companies
- 👥 Client users, company users, owners, managers, and members
- 🔐 Authentication, permissions, and company-aware authorization
- 📬 Notifications, messaging, and background event handling
- 🧾 Comments, replies, and user-facing validation/error flows

## 🧠 Business Flavor

Some of the most important project decisions come from the business model itself:

- Company users do not always act as individuals. In features like chat and favorites, they often act as the company they belong to.
- Clients, pharmacies, and laboratories do not all interact the same way.
- Validation is split carefully between application rules and domain invariants.
- External services are pulled into the app's consistency model through compensations instead of ad hoc cleanup.

That is why the docs folder matters in this project: the tricky parts are not only technical, they are also conceptual.

## 🛠️ Tech Stack

- `ASP.NET Core` Minimal API + `SignalR`
- `MediatR` request pipeline
- `FluentValidation`
- `EF Core` + `PostgreSQL`
- `Dapper` for read-side query services
- `MassTransit` + `RabbitMQ` + EF Core outbox
- `Redis`
- `JWT` authentication
- `.NET Aspire` for local orchestration
- `MailPit` in development, `Resend` by default outside development

## 🗂️ Solution Map

- `src/Core/MediSearch.Core.Domain`
  Domain entities, value objects, smart enums, domain events, and invariants.

- `src/Core/MediSearch.Core.Application`
  Commands, queries, handlers, validators, behaviors, ports, compensations, and notifications.

- `src/Infrastructure/*`
  Persistence, security, localization, communication, templating, caching, messaging, and file storage.

- `src/Presentation/MediSearch.Presentation.WebApi`
  Endpoints, SignalR hub, OpenAPI, exception handling, and current-user HTTP integration.

- `src/Hosting/*`
  Aspire AppHost and shared service defaults.

- `docs`
  The non-obvious rules and conventions that are worth preserving for future contributors.

## 📚 Documentation

If you are onboarding, these are the best docs to start with:

- [Architecture Overview](docs/Architecture-Overview.md)
- [Business Context And Agent Model](docs/Business-Context-And-Agent-Model.md)
- [Validation And Value Objects](docs/Validation-And-Value-Objects.md)
- [Authorization And Identity Flow](docs/Authorization-And-Identity-Flow.md)
- [Compensations And External Consistency](docs/Compensations-And-External-Consistency.md)

For the full set:

- [Exceptions And Error Responses](docs/Exceptions-And-Error-Responses.md)
- [Localization And Error Codes](docs/Localization-And-Error-Codes.md)
- [Repositories And Query Services](docs/Repositories-And-Query-Services.md)
- [Configuration And Secrets Reference](docs/Configuration-Secrets-Reference.md)
- [MassTransit Messaging Architecture](docs/MassTransit-Messaging-Architecture.md)

## 🚀 Run Locally

1. Install the .NET SDK version pinned in `global.json`.
2. Make sure Docker is running.
3. Fill the required settings and user secrets described in [Configuration And Secrets Reference](docs/Configuration-Secrets-Reference.md).
4. Start the Aspire host:

```powershell
dotnet run --project src/Hosting/MediSearch.Hosting.AppHost
```

5. Open the Aspire dashboard and launch the Web API link that points to Scalar.

## 🧪 Local Dev Notes

- Aspire starts PostgreSQL, RabbitMQ, Redis, and MailPit for you.
- In development, the Web API applies migrations automatically.
- In development, the app also ensures the system administrator account exists.
- The seeded administrator username is `administrator`.
- Its password comes from the `AdminPassword` user secret.
- When running through Aspire, connection strings are injected automatically.
- The values in `appsettings.Development.json` are fallback values for running the Web API directly without the AppHost.

## 🌶️ A Few Conventions Worth Knowing

- Domain invariants belong in value objects and entities, not duplicated in FluentValidation validators.
- Application validators make requests safe to execute and attach proper error codes for client responses.
- Commands and queries own authorization through application request attributes and MediatR behaviors.
- Methods named `OrDefault` explicitly mean the resource may be missing and `null` is part of the contract.
- External side effects that must behave transactionally with the database go through the compensation workflow.

## 📌 In Short

MediSearch is a business-driven backend with a lot of attention on rules, consistency, and the difference between user identity and company identity. If you want the short version: marketplace + chat + favorites + permissions + messaging, all shaped around how pharmacies, laboratories, and clients actually interact.
