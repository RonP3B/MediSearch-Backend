# MassTransit Messaging Architecture

> Note: this solution intentionally stays on MassTransit `8.3.6`. MassTransit v8 remains open-source, while the current v9 line is the commercial/licensed line, including development use. We keep the older version partly for that reason.

## Overview

This document describes the messaging architecture built on top of MassTransit, RabbitMQ, and the EF Core Outbox pattern. The system handles three distinct message types — **Domain Events**, **Notifications**, and **Compensation Events** — each flowing through the same infrastructure but serving different conceptual purposes.

---

## Message Types

### Domain Events (`IDomainEvent`)
Represent something that happened in the domain. They are raised by entities, collected from the `AppDbContext` change tracker inside the unit of work, and published immediately before `SaveChangesAsync`. They are always tied to a transaction.

```csharp
public abstract class DomainEvent : IDomainEvent
{
    public Guid Id { get; init; }
    public DateTime OccurredAtUtc { get; init; }

    protected DomainEvent()
    {
        Id = Guid.CreateVersion7();
        OccurredAtUtc = DateTime.UtcNow;
    }
}
```

### Notifications (`INotification`)
Application-level events published explicitly via `IEventBus`. Used for cross-cutting concerns like sending emails or SMS. They are not tied to a transaction. They carry an `IdempotencyKey` to support provider-level deduplication.

```csharp
public abstract record Notification : INotification
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
    public string IdempotencyKey { get; }

    // Pass an explicit key for deterministic operations (e.g. one-time per user).
    // If omitted, Id is used — safe for repeatable operations where each invocation
    // is intentionally distinct.
    protected Notification(string? idempotencyKey = null)
    {
        IdempotencyKey = idempotencyKey ?? Id.ToString();
    }
}
```

### Compensation Events (`ICompensationEvent`)
Used to undo side effects when a distributed operation fails. For example, deleting a file from storage or rolling back an external user account creation. They must be naturally idempotent — see [Idempotency](#idempotency) below.

---

## How Messages Flow

### Domain Events

1. An entity raises a domain event and stores it in memory via `entity.AddDomainEvent(...)`.
2. `UnitOfWork.SaveChangesAsync` collects all pending domain events from the current `AppDbContext` and publishes them via `IPublishEndpoint` immediately before calling `SaveChangesAsync`.
3. MassTransit's EF Core Outbox captures those publishes and stores them in the outbox table within the same transaction.
4. If the transaction succeeded the outbox poller picks up the message and delivers it to RabbitMQ.
5. The corresponding `DomainEventHandlerConsumer<THandler, TEvent>` receives it and delegates to the handler.

```
Entity.AddDomainEvent(event)
  └── UnitOfWork.SaveChangesAsync (pre-SaveChanges)
        └── EF Core Outbox (same transaction)
              └── RabbitMQ
                    └── DomainEventHandlerConsumer<THandler, TEvent>
                          └── YourDomainEventHandler.Handle(event)
```

### Notifications

Notifications are published explicitly from handlers (domain event handlers or command handlers) via `IEventBus`:

```csharp
await _eventBus.PublishAsync(new UserRegisteredNotification(domainEvent.Id), cancellationToken);
```

They follow the same delivery path through RabbitMQ into a `NotificationHandlerConsumer<THandler, TNotification>`.

### Compensation Events

Published via `IEventBus` in response to failures. Same delivery path as notifications.

---

## EventBus and IBus

`IEventBus` is implemented using MassTransit's `IBus` directly, not `IPublishEndpoint`:

```csharp
internal sealed class EventBus(IBus bus) : IEventBus
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent => bus.Publish(@event, cancellationToken);

    public Task PublishBatchAsync<TEvent>(
        IEnumerable<TEvent> events,
        CancellationToken cancellationToken = default
    )
        where TEvent : class, IEvent => bus.PublishBatch(events, cancellationToken);
}
```

**Why `IBus` and not `IPublishEndpoint`?**

`UseBusOutbox()` intercepts all `IPublishEndpoint` calls and writes messages to the outbox table using the scoped `AppDbContext` before forwarding them to RabbitMQ. This works correctly for domain events because they are published inside `UnitOfWork.SaveChangesAsync` while the DbContext is healthy.

However, `IEventBus` is also used during our custom compensation rollback for external services — which runs after an exception has been thrown through the same scoped `AppDbContext`. At that point the DbContext is in a faulted state. The outbox write appears to succeed (no exception propagates back) but the message is silently discarded, never reaching RabbitMQ.

`IBus` bypasses the outbox entirely and publishes directly to the broker, which is the correct behavior for both notifications and compensation events since neither requires transactional delivery guarantees.

**Rule of thumb:**
- `IPublishEndpoint` (via `UnitOfWork`) → domain events only, must commit with the DB transaction
- `IBus` (via `IEventBus`) → notifications and compensation events, always go directly to RabbitMQ

---

## Consumer Registration

Each handler type gets its own dedicated MassTransit consumer via reflection. This is done automatically at startup — no manual consumer registration is needed.

```csharp
internal static class HandlerConsumerMappings
{
    public static readonly HandlerConsumerMapping[] All =
    [
        new(typeof(IDomainEventHandler<>),    typeof(DomainEventHandlerConsumer<,>),    ApplicationAssemblyReference.Assembly),
        new(typeof(INotificationHandler<>),   typeof(NotificationHandlerConsumer<,>),   ApplicationAssemblyReference.Assembly),
        new(typeof(ICompensationEventHandler<>), typeof(CompensationEventHandlerConsumer<,>), ApplicationAssemblyReference.Assembly),
    ];
}
```

For each concrete handler found in the assembly (e.g. `UserRegisteredDomainEventHandler`), a closed generic consumer is created (`DomainEventHandlerConsumer<UserRegisteredDomainEventHandler, UserRegisteredDomainEvent>`) and registered with its own endpoint.

**Key: each handler gets its own independent queue.** If `HandlerA` and `HandlerB` both handle the same event, they receive and process it independently. A failure in `HandlerA` does not cause `HandlerB` to retry, and vice versa.

### Consumer Implementation

```csharp
internal sealed class DomainEventHandlerConsumer<THandler, TEvent>(THandler handler)
    : IConsumer<TEvent>
    where THandler : class, IDomainEventHandler<TEvent>
    where TEvent : class, IDomainEvent
{
    public Task Consume(ConsumeContext<TEvent> context) =>
        handler.Handle(context.Message, context.CancellationToken);
}
```

---

## Outbox Pattern

The EF Core outbox ensures that messages are only delivered if the database transaction commits successfully. This prevents the dual-write problem (writing to the DB and publishing a message independently).

Short warning: do not move domain event publishing back into an EF Core `SaveChangesInterceptor` while using MassTransit's EF outbox on the same `AppDbContext`. Resolving `IPublishEndpoint` from that interceptor can create a circular startup/runtime dependency with the DbContext and hang application startup.

```csharp
configurator.AddEntityFrameworkOutbox<AppDbContext>(outbox =>
{
    outbox.UsePostgres();
    outbox.UseBusOutbox();
    outbox.QueryDelay = TimeSpan.FromSeconds(options.OutboxQueryDelaySeconds); // 1s recommended
    outbox.DuplicateDetectionWindow = TimeSpan.FromMinutes(options.DuplicateDetectionWindowMinutes); // 30min recommended
});
```

The `AppDbContext` includes the required outbox tables:

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    builder.AddInboxStateEntity();    // tracks processed messages per consumer
    builder.AddOutboxMessageEntity(); // stores pending outbound messages
    builder.AddOutboxStateEntity();   // tracks outbox poller state
}
```

`InboxState` is the MassTransit inbox — when a consumer successfully processes a message, a record is saved in the same EF transaction. On redelivery, MassTransit checks this table and skips processing if a record already exists. `DuplicateDetectionWindow` controls how long these records are retained.

---

## Retry Policy

All endpoints share a global exponential retry policy applied via `AddConfigureEndpointsCallback`:

```csharp
endpointCfg.UseMessageRetry(r =>
{
    r.Exponential(
        options.RetryCount,
        TimeSpan.FromSeconds(options.RetryInitialIntervalSeconds),
        TimeSpan.FromSeconds(options.RetryMaxIntervalSeconds),
        TimeSpan.FromSeconds(options.RetryIntervalDeltaSeconds)
    );

    r.Ignore<ExpectedResultNotFoundException>();
    r.Ignore<CorruptedInvariantException>();
});
```

`ExpectedResultNotFoundException` is thrown by query service methods that guarantee a result exists. If the result is missing, retrying will never fix it — the message goes straight to the error queue for inspection.

`CorruptedInvariantException` represents impossible application states caused by broken domain or persistence invariants (for example, a company without an owner, or a chat room without exactly two distinct participants). These failures are non-transient and are also sent directly to the error queue without retries.

Recommended values:

```json
"MassTransit": {
  "PrefetchCount": 16,
  "ConcurrentMessageLimit": 8,
  "RetryCount": 4,
  "RetryInitialIntervalSeconds": 1,
  "RetryMaxIntervalSeconds": 30,
  "RetryIntervalDeltaSeconds": 3,
  "OutboxQueryDelaySeconds": 1,
  "DuplicateDetectionWindowMinutes": 30
}
```

- `PrefetchCount` — messages RabbitMQ delivers before waiting for acks. Rule of thumb: `2 × ConcurrentMessageLimit`.
- `ConcurrentMessageLimit` — parallel messages processed per endpoint. Keep conservative to protect the DB connection pool.
- `OutboxQueryDelaySeconds` — how often the outbox poller checks for unsent messages. 1s is the MassTransit default and adds negligible DB load.
- `DuplicateDetectionWindowMinutes` — how long `InboxState` records are retained. 30 minutes covers realistic crash-recovery scenarios. **Do not lower this to 5 minutes** — it leaves a too-small window for infrastructure-level duplicate detection.

---

## Idempotency

### Domain Event Handlers

MassTransit's inbox (`InboxStateEntity`) provides infrastructure-level deduplication. Since each handler has its own queue, a retry of `HandlerA` never re-runs `HandlerB`.

Business logic idempotency is still your responsibility. Ask for each handler: _what happens if this runs twice with the same event?_

- **DB inserts** → check existence first, or use `ON CONFLICT DO NOTHING`
- **Read model projections** → `UPDATE ... SET field = value` is naturally idempotent; bare `INSERT` is not
- **Cache invalidation** → naturally idempotent
- **Email / external service calls** → see below

### Notifications and Email Idempotency

Since `IEmailService.SendAsync` and `SendBulkAsync` require a non-nullable `idempotencyKey`, the notification handler always passes `notification.IdempotencyKey`:

```csharp
public class UserWelcomeEmailNotificationHandler(IEmailService emailService)
    : NotificationHandler<UserWelcomeEmailNotification>
{
    public override Task Handle(UserWelcomeEmailNotification notification, CancellationToken ct) =>
        emailService.SendAsync(BuildEmail(notification), notification.IdempotencyKey, ct);
}
```

**Resend** supports native idempotency keys on both `EmailSendAsync` and `EmailBatchAsync` — the SDK accepts the key as the first parameter and deduplicates on their end within 24 hours.

**SendGrid** does not support idempotency keys natively. Our implementation keeps chunk-level idempotency in app code and sends each chunk as a single API request with one personalization per recipient. This preserves strict chunk retries only when every email in the chunk shares the same subject and body.

#### Choosing the Right IdempotencyKey

The `IdempotencyKey` on a `Notification` is either auto-generated or explicit:

```csharp
// Repeatable operation — each invocation is intentionally distinct.
// A fresh Id is fine because MassTransit retries deliver the same message instance,
// so the key stays stable across retries of the same invocation.
new ResendActivationEmailNotification(userId);

// One-time per user operation — use a deterministic key so two concurrent
// requests for the same user are deduplicated at the provider level.
new AccountConfirmedEmailNotification(
    userId,
    idempotencyKey: $"{nameof(ConfirmAccountEmailCommand)}-{userId}"
);
```

#### Publishing Notifications from Domain Event Handlers

Always pass `domainEvent.Id` as the idempotency key. If the domain event handler retries, it re-publishes the notification — without a stable key, the notification gets a new random ID on every retry and the idempotency protection is lost:

```csharp
public class UserRegisteredDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<UserRegisteredDomainEvent>
{
    public override Task Handle(UserRegisteredDomainEvent domainEvent, CancellationToken ct) =>
        eventBus.PublishAsync(
            new UserWelcomeEmailNotification(domainEvent.UserId, idempotencyKey: domainEvent.Id.ToString()),
            ct
        );
}
```

### Compensation Event Handlers

Compensation handlers must be **naturally idempotent** — "already in the desired state" must be treated as success, never as a failure that triggers a retry.

```csharp
// ✅ Correct — treat already-deleted as success
public override async Task Handle(
    ExternalUserDeletionCompensationEvent compensationEvent,
    CancellationToken cancellationToken
)
{
    var result = await _accountManager.DeleteUserAsync(
        compensationEvent.ExternalUserId,
        cancellationToken
    );

    if (!result.Succeeded && !result.HasError(AccountErrorCodes.AccountNoLongerExists))
    {
        throw new InvalidOperationException(
            $"Failed to delete external user {compensationEvent.ExternalUserId} during compensation."
        );
    }
}

// ✅ Correct — IFileStorageService.DeleteFileAsync must treat "not found" as success
public override Task Handle(
    AssetDeletionCompensationEvent compensationEvent,
    CancellationToken cancellationToken
) => _fileStorageService.DeleteFileAsync(compensationEvent.AssetKey, cancellationToken);
```

Only throw on genuine failures. Never throw when the system is already in the desired end state.

---

## Bulk Email and Batch Limits

When a domain event affects many users, chunk in the domain event handler and publish multiple notifications — do not pass a large collection through the bus.

Use `IBatchSizeProvider` to get the chunk size for the target transport, then publish all notifications in one call via `PublishBatchAsync`:
```csharp
// ✅ Correct — chunk by transport batch size, publish all at once
public override async Task Handle(ProductDiscountedDomainEvent domainEvent, CancellationToken ct)
{
    var subscribers = await _repository.GetSubscribersAsync(domainEvent.ProductId, ct);
    var batchSize = _batchSizeProvider.GetBatchSize(TransportType.Email);

    var notifications = subscribers
        .Chunk(batchSize)
        .Select(batch => new NotifyDiscountSubscribersNotification(
            batch,
            idempotencyKey: $"{domainEvent.Id}-{Guid.CreateVersion7()}"
        ));

    await _eventBus.PublishBatchAsync(notifications, ct);
}
```

Each notification chunk is an independent message with its own retry scope. If one batch fails, only that batch retries.

Note the idempotency key uses `domainEvent.Id` as a stable prefix combined with a unique suffix per chunk — using `domainEvent.Id` alone across all chunks would cause Resend to deduplicate them into a single send.

Provider batch limits enforced by the service implementations:
- **Resend** — 100 messages per batch request
- **SendGrid** — 1,000 recipients per request

Each implementation throws `InvalidOperationException` if the limit is exceeded.

---

## Adding a New Handler

No wiring or registration is needed. Create a class that extends the appropriate base:

```csharp
// Domain event handler
public sealed class UserRegisteredDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<UserRegisteredDomainEvent>
{
    public override Task Handle(UserRegisteredDomainEvent domainEvent, CancellationToken ct) =>
        eventBus.PublishAsync(
            new UserWelcomeEmailNotification(domainEvent.UserId, domainEvent.Id.ToString()),
            ct
        );
}

// Notification handler
public sealed class UserWelcomeEmailNotificationHandler(IEmailService emailService)
    : NotificationHandler<UserWelcomeEmailNotification>
{
    public override Task Handle(UserWelcomeEmailNotification notification, CancellationToken ct) =>
        emailService.SendAsync(BuildEmail(notification), notification.IdempotencyKey, ct);
}

// Compensation event handler
public sealed class AssetDeletionCompensationEventHandler(IFileStorageService fileStorageService)
    : CompensationEventHandler<AssetDeletionCompensationEvent>
{
    public override Task Handle(AssetDeletionCompensationEvent compensationEvent, CancellationToken ct) =>
        fileStorageService.DeleteFileAsync(compensationEvent.AssetKey, ct);
}
```

`HandlerConsumerRegistry` scans the application assembly at startup, finds all concrete handler implementations, and registers the corresponding consumers automatically.
