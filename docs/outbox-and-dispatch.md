# Outbox and dispatch

The outbox pattern ensures that messages are delivered reliably even when the application restarts or a downstream service is temporarily unavailable. IDFCR provides abstractions for persisting, reading, dispatching, and publishing outbox messages.

**Packages:** `IDFCR.Abstractions.Outbox`, `IDFCR.Abstractions.Outbox.Extensions`, `IDFCR.Abstractions.Outbox.Interceptors`, `IDFCR.Outbox.EntityFramework`, `IDFCR.Outbox.Extensions`

---

## Why an outbox?

Without an outbox, a handler that writes to a database *and* sends a message faces a race: the database commit succeeds but the message send fails (or vice versa), leaving the two systems out of sync.

With an outbox:

1. The handler writes its side effects *and* inserts an outbox record in the same transaction.
2. A background worker reads pending outbox records and delivers them to the external system.
3. Delivery is at-least-once: if the worker restarts, it re-reads pending records.

---

## Outbox flow

```mermaid
flowchart LR
    Handler --> Repository
    Repository --> OutboxInterceptor
    OutboxInterceptor --> ScopedResources
    UoWPostProcessor --> OutboxNotificationHandler
    OutboxNotificationHandler --> OutboxTable[(Outbox table)]
    BackgroundWorker --> OutboxReader
    OutboxReader --> OutboxDispatcher
    OutboxDispatcher --> OutboxPublisher
    OutboxPublisher --> ExternalSystem
```

---

## IOutboxEntity

`IOutboxEntity` is the core contract for an outbox record. Implement it on your EF Core entity:

```csharp
public interface IOutboxEntity : IAuditCreatedTimestamp, IAuditModifiedTimestamp, IIdentifiable
{
    bool IsUpdate { get; set; }
    string EntityType { get; set; }
    string? Data { get; set; }
    DateTimeOffset? CompletedTimestampUtc { get; set; }
    DateTimeOffset? FailedTimestampUtc { get; set; }
    DateTimeOffset? ProcessedTimestampUtc { get; set; }
}
```

`IOutboxEntity<TKey>` adds a typed identifier property.

### DefaultOutboxEntity

`DefaultOutboxEntity` is a ready-made implementation of `IOutboxEntity<Guid>` that you can use directly or extend:

```csharp
public sealed class MyOutboxMessage : DefaultOutboxEntity
{
    // All IOutboxEntity properties inherited
    // Add any domain-specific properties here
}
```

---

## IOutboxPublisher

`IOutboxPublisher` receives a batch of outbox records and delivers them to the external system. Implement `OutboxPublisherBase<TMessage>` to get the type-safe variant:

```csharp
public sealed class OrderEventPublisher : OutboxPublisherBase<MyOutboxMessage>
{
    private readonly IEventBus _eventBus;

    public OrderEventPublisher(IEventBus eventBus) => _eventBus = eventBus;

    public override async Task HandleAsync(
        IEnumerable<MyOutboxMessage> messages,
        CancellationToken cancellationToken)
    {
        foreach (var message in messages)
        {
            await _eventBus.PublishAsync(message.EntityType, message.Data, cancellationToken);
        }
    }
}
```

---

## IOutboxReader

`IOutboxReader` pages pending outbox records so the dispatcher can process them in batches. Implement `OutboxReaderBase<TMessage, TPagedQuery>`:

```csharp
public sealed class OrderOutboxReader(AppDbContext db)
    : OutboxReaderBase<MyOutboxMessage, GetPendingOutboxQuery>("orders")
{
    public override async Task<IPagedUnitResult<MyOutboxMessage>> GetMessagesAsync(
        GetPendingOutboxQuery request,
        CancellationToken cancellationToken)
    {
        var messages = await db.OutboxMessages
            .Where(m => m.CompletedTimestampUtc == null && m.FailedTimestampUtc == null)
            .OrderBy(m => m.CreatedTimestampUtc)
            .Skip((request.PageIndex ?? 0) * (request.PageSize ?? 10))
            .Take(request.PageSize ?? 10)
            .ToListAsync(cancellationToken);

        return PagedUnitResult.FromResult(messages, messages.Count, request);
    }

    public override async Task<IUnitResult> HasPagesAsync(
        GetPendingOutboxQuery request,
        CancellationToken cancellationToken)
    {
        var hasAny = await db.OutboxMessages
            .AnyAsync(m => m.CompletedTimestampUtc == null && m.FailedTimestampUtc == null,
                cancellationToken);

        return UnitResult.Success(hasAny ? UnitAction.Get : UnitAction.None);
    }
}
```

---

## IOutboxDispatcher

`IOutboxDispatcher` orchestrates the read → publish cycle. It calls `IOutboxReader.GetMessagesAsync`, then passes the paged result to `IOutboxPublisher.HandleAsync`.

`DefaultOutboxReaderFactory` discovers all registered `IOutboxReader` implementations and routes dispatch calls by name.

---

## Background dispatch with a hosted service

Run a background service that periodically polls the outbox:

```csharp
public sealed class OutboxDispatchWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxDispatchWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IOutboxReaderFactory>();
            var dispatcher = scope.ServiceProvider.GetRequiredService<IOutboxDispatcher>();

            // IOutboxReaderFactory resolves the reader by name
            foreach (var reader in factory.GetAll())
            {
                var query = new GetPendingOutboxQuery { PageSize = 50, PageIndex = 0 };
                var messages = await reader.GetMessagesAsync(query, stoppingToken);
                await dispatcher.PushAsync(messages, stoppingToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
```

---

## Delivery semantics

IDFCR's outbox provides **at-least-once** delivery. Your publisher should be idempotent where possible, or use message identifiers to deduplicate on the receiving side.

Key fields for tracking delivery:

| Field | Meaning |
|---|---|
| `ProcessedTimestampUtc` | Set when the dispatcher reads the message |
| `CompletedTimestampUtc` | Set by `UnitOfWorkPostPipelineProcessor` when the commit succeeds |
| `FailedTimestampUtc` | Set by `UnitOfWorkPostPipelineProcessor` when the commit fails |

---

## Registering outbox services

```csharp
using IDFCR.Abstractions.Outbox.Extensions;

services
    .AddScoped<IOutboxReader, OrderOutboxReader>()
    .AddScoped<IOutboxPublisher<MyOutboxMessage>, OrderEventPublisher>()
    .AddSingleton<IOutboxReaderFactory, DefaultOutboxReaderFactory>();
```

If you use the EF Core outbox package:

```csharp
services.AddOutboxEntityFramework<AppDbContext, MyOutboxMessage>();
```

---

## Further reading

- [Interceptors](interceptors.md) — `OutboxInterceptor` for staging messages automatically.
- [Handlers and pipelines](handlers-and-pipelines.md) — `UnitOfWorkPostPipelineProcessor` and commit lifecycle.
- [Persistence and unit of work](persistence-and-unit-of-work.md) — `IUnitOfWork` and `SaveChangesAsync`.
