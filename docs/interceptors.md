# Interceptors

Interceptors let you hook into entity-lifecycle events — insert, update, delete — without adding logic to your repositories or handlers. IDFCR provides built-in interceptors for audit timestamps, and you can write your own for any cross-cutting concern.

**Packages:** `IDFCR.Abstractions.Interceptors`, `IDFCR.Abstractions.Interceptors.DependencyInjection`, `IDFCR.Abstractions.Interceptors.Extensions`, `IDFCR.Abstractions.Outbox.Interceptors`

---

## How interceptors work

When the repository performs an operation (insert, update, delete), it calls `IEntityInterceptorFactory.Intercept(context)` after staging the change. The factory resolves all registered `IEntityInterceptor` implementations and calls each one in `OrderIndex` order. Each interceptor decides whether it applies via `CanIntercept(context)`.

Interceptors share execution-scoped state through `IScopedResources`. This lets the `OutboxInterceptor`, for example, stage a message that the `UnitOfWorkPostPipelineProcessor` later finalises after `SaveChangesAsync`.

---

## IEntityInterceptor

```csharp
public interface IEntityInterceptor
{
    IEntityInterceptorContext? State { get; }
    IEntityInterceptorFactory? Context { get; set; }
    int? OrderIndex { get; }

    bool CanIntercept(IEntityInterceptorContext context);
    void Intercept(IEntityInterceptorContext context);
    Task<bool> CanInterceptAsync(IEntityInterceptorContext context, CancellationToken cancellationToken);
    Task InterceptAsync(IEntityInterceptorContext context, CancellationToken cancellationToken);
}
```

Extend `EntityInterceptorBase` to get sensible defaults for the synchronous overloads and focus on the async variants in most cases.

---

## IEntityInterceptorContext

The context passed to each interceptor describes:

- **Stage** (`EntityContextBehaviorStage`): Before or after the operation.
- **Behavior** (`EntityContextBehavior`): Insert, Update, or Delete.
- **Entity instance**: The object being acted on.

```csharp
public interface IEntityInterceptorContext
{
    EntityContextBehaviorStage Stage { get; }
    EntityContextBehavior Behavior { get; }
    object Entity { get; }
}
```

Use `context.Entity as MyEntityType` to determine applicability inside `CanIntercept`.

---

## Built-in interceptors

| Interceptor | What it does |
|---|---|
| `AuditCreatedTimestampEntityInterceptor` | Sets `CreatedAt` on new entities implementing `IAuditCreatedTimestamp` |
| `AuditModifiedTimestampEntityInterceptor` | Updates `ModifiedAt` on every change for entities implementing `IAuditModifiedTimestamp` |
| `AuditEntityChangesInterceptor` | Records property-level changes via `IAuditProcessor` for auditing |
| `SoftDeletionEntityInterceptor` | Sets a `DeletedAt` timestamp instead of removing the row (from `IDFCR.Abstractions.Persistence.Interceptors`) |
| `OutboxInterceptor` | Stages an `IOutboxEntity` in `IScopedResources` when an entity change occurs (from `IDFCR.Abstractions.Outbox.Interceptors`) |

---

## Registering interceptors

```csharp
using IDFCR.Abstractions.Interceptors.DependencyInjection.Extensions;

services.AddInterceptors(typeof(Program).Assembly);
```

`AddInterceptors` scans the supplied assemblies (plus the IDFCR interceptors assembly) for:

- `IEntityInterceptor` implementations → registered as transient.
- `IAuditProcessor` implementations → registered as transient.

It also registers:

- `IEntityInterceptorFactory` → `DefaultEntityInterceptorFactory` (transient).
- `IAuditProcessorProvider` → `DefaultAuditProcessorProvider` (transient).
- `IScopedResources` → `DefaultScopedResources` (scoped).

---

## Writing a custom interceptor

```csharp
using IDFCR.Abstractions.Interceptors.Interceptors;

public sealed class OrderApprovedInterceptor : EntityInterceptorBase
{
    public override int? OrderIndex => 100;

    public override bool CanIntercept(IEntityInterceptorContext context)
        => context.Behavior == EntityContextBehavior.Update
        && context.Entity is Order order
        && order.Status == OrderStatus.Approved;

    public override async Task InterceptAsync(
        IEntityInterceptorContext context,
        CancellationToken cancellationToken)
    {
        var order = (Order)context.Entity;
        // Example: stage a notification via scoped resources
        // Access IScopedResources through the Context factory if needed
        await Task.CompletedTask;
    }
}
```

Place the interceptor in an assembly passed to `AddInterceptors(...)` and it will be discovered automatically.

---

## Audit processors

`IAuditProcessor` lets you respond to property-level change tracking. Implement `AuditProcessorBase<TEntity>` to receive before/after snapshots of an entity's auditable properties.

```csharp
public sealed class OrderAuditProcessor : AuditProcessorBase<Order>
{
    protected override Task ProcessAsync(
        Order entity,
        IReadOnlyList<AuditEntry> changes,
        CancellationToken cancellationToken)
    {
        foreach (var change in changes)
        {
            Console.WriteLine($"{change.PropertyName}: {change.OldValue} → {change.NewValue}");
        }
        return Task.CompletedTask;
    }
}
```

Audit processors are also discovered by `AddInterceptors`.

---

## IScopedResources and interceptor sharing

`IScopedResources` is the shared bag for execution-scoped data within a single interceptor pipeline run. Use it to pass objects between interceptors without constructor-injecting each downstream component.

```csharp
// Inside an interceptor: stage data for a downstream consumer
scopedResources.AddOrUpdate(new MyPipelineContext(entity.Id));

// Inside another interceptor or the post-processor: read it
if (scopedResources.TryGetScopedResource<MyPipelineContext>(out var ctx))
{
    // use ctx
}
```

`IScopedResources` is **not** a service locator. Store only objects that you have already resolved elsewhere. Do not call it to resolve arbitrary services.

---

## OutboxInterceptor

`OutboxInterceptor` (in `IDFCR.Abstractions.Outbox.Interceptors`) is a built-in interceptor that:

1. Detects entity changes that should produce outbox messages.
2. Maps the entity to an `IOutboxEntity` via a registered `IOutboxEntityNotificationHandler`.
3. Stages the message in `IScopedResources`.
4. The `UnitOfWorkPostPipelineProcessor` reads from `IScopedResources` after a successful commit and finalises the outbox record.

Register it via `AddInterceptors` or explicitly via `IDFCR.Abstractions.Outbox.Extensions`.

---

## Further reading

- [Outbox and dispatch](outbox-and-dispatch.md) — the full outbox pattern.
- [Persistence and unit of work](persistence-and-unit-of-work.md) — repositories and `IScopedResources`.
- [Architecture overview](architecture-overview.md) — how interceptors fit into the persistence flow.
