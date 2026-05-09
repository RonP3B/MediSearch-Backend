# Architecture Overview

This document is the best starting point for new contributors. It explains how the solution is split, how a request moves through the system, and which follow-up docs explain the non-obvious conventions in more detail.

## Solution Shape

The solution is organized into clear layers:

- `Core/Domain`
  - The business model. Entities, value objects, smart enums, domain events, and domain exceptions live here.
- `Core/Application`
  - Use cases. Commands, queries, handlers, validators, MediatR pipeline behaviors, ports, notifications, and compensations live here.
- `Infrastructure`
  - Implementations for persistence, messaging, auth, localization, email, file storage, caching, and templating.
- `Presentation/WebApi`
  - Minimal API endpoints, SignalR hub, exception handling, OpenAPI, and the `ICurrentUser` HTTP adapter.
- `Hosting`
  - Aspire AppHost plus shared service defaults for local orchestration, health checks, and telemetry.
- `Shared`
  - Solution-level constants shared across projects.

## High-Level Runtime Flow

For a typical HTTP request:

1. A minimal API endpoint receives the request and maps it into a command or query.
2. The request is sent through MediatR.
3. MediatR behaviors run in order:
   - request logging pre-processor
   - exception logging
   - performance logging
   - authorization
   - validation
   - unit of work
4. The handler performs the use case by using repositories, query services, and infrastructure ports.
5. Domain entities/value objects enforce invariants.
6. For commands, `UnitOfWorkBehavior` saves changes and publishes domain events through the EF outbox path.
7. Domain event handlers can fan out into notifications, cache invalidations, realtime pushes, or compensations.

## Domain Model Style

The domain is intentionally strict:

- Value objects are used for most entity properties.
- Value objects expose `TryFrom(...)` and `From(...)`.
- Entities throw `BusinessRuleException` when invariants are violated.
- Domain events are raised from entities, then published at unit-of-work save time.

The goal is to keep business rules close to the model instead of leaking them into controllers, validators, or SQL.

## Data Access Split

The solution uses a write/read split inside the same service:

- Write side:
  - EF Core repositories return aggregates and track changes through `AppDbContext`.
- Read side:
  - Dapper query services return DTOs optimized for reads.

One important convention is that nullable lookups are explicitly named with `OrDefault`. See [Repositories And Query Services](Repositories-And-Query-Services.md).

## Key Internal Docs Worth Knowing

- Validation strategy:
  - [Validation And Value Objects](Validation-And-Value-Objects.md)
- Templating and rendered content flow:
  - [Templating-And-Application-Models.md](Templating-And-Application-Models.md)
- Exception and HTTP error strategy:
  - [Exceptions And Error Responses](Exceptions-And-Error-Responses.md)
- Localized client-facing errors:
  - [Localization And Error Codes](Localization-And-Error-Codes.md)
- Compensating external side effects:
  - [Compensations And External Consistency](Compensations-And-External-Consistency.md)
- Business identity model and interaction rules:
  - [Business Context And Agent Model](Business-Context-And-Agent-Model.md)
- Auth and permissions:
  - [Authorization And Identity Flow](Authorization-And-Identity-Flow.md)
- Messaging internals:
  - [MassTransit Messaging Architecture](MassTransit-Messaging-Architecture.md)

## Infrastructure Snapshot

The current default infrastructure stack is:

- PostgreSQL for primary persistence
- RabbitMQ + MassTransit for messaging
- EF Core outbox for transactional domain-event delivery
- Redis for caching
- MailPit in development for email inspection
- Resend in non-development by default for real email sending
- Local file storage in development/runtime by default, with commented alternatives for cloud storage
- SignalR for realtime pushes
- Aspire AppHost for local service orchestration

## When to Read Which Doc

- If you are touching validators or request DTOs, start with [Validation And Value Objects](Validation-And-Value-Objects.md).
- If you are adding or changing email/template content, read [Templating-And-Application-Models.md](Templating-And-Application-Models.md).
- If you are adding a new client-facing rule or exception, read [Localization And Error Codes](Localization-And-Error-Codes.md) and [Exceptions And Error Responses](Exceptions-And-Error-Responses.md).
- If you are adding a repository/query method, read [Repositories And Query Services](Repositories-And-Query-Services.md).
- If you are calling an external service inside a command handler, read [Compensations And External Consistency](Compensations-And-External-Consistency.md).
- If you are working on chat, favorites, comments, or company-user behavior, read [Business Context And Agent Model](Business-Context-And-Agent-Model.md).
- If you are adding a secured request, read [Authorization And Identity Flow](Authorization-And-Identity-Flow.md).
