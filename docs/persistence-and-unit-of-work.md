# Persistence and unit of work

IDFCR provides a thin set of persistence abstractions over which you build repositories and unit-of-work patterns. The EF Core packages add a default implementation.

**Packages:** `IDFCR.Abstractions.Persistence`, `IDFCR.Persistence.EntityFrameworkCore`, `IDFCR.Persistence.EntityFrameworkCore.Extensions`

---

## The unit of work

`IUnitOfWork` expresses the single responsibility of committing a set of changes:

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
```

**Do not call `SaveChangesAsync` from inside a handler.** The `UnitOfWorkPostPipelineProcessor` calls it after the handler returns successfully, ensuring that the interceptor and outbox pipeline have completed first. See [Handlers and pipelines](handlers-and-pipelines.md) for the full flow.

`ITransactionalUnitOfWork` extends `IUnitOfWork` with `IDbTransaction` support when you need explicit transaction control across multiple operations.

---

## The repository

```csharp
public interface IRepository<T, TKey> : IUnitOfWork
    where TKey : struct
{
    Task<IUnitResult<T>> FindAsync(TKey key, CancellationToken cancellationToken);
    Task<IUnitResult<T>> FindAsync(object[] keys, CancellationToken cancellationToken);
    Task<IUnitResult<TKey>> UpsertAsync(T entry, CancellationToken cancellationToken);
    Task<IUnitResult> DeleteAsync(TKey key, CancellationToken cancellationToken);
    Task<IPagedUnitResult<T>> GetPagedAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IPagedQuery;
}
```

Every repository method returns an `IUnitResult*`. Successful operations carry the value; failures carry the reason and (optionally) the exception. There are no raw exceptions escaping the repository boundary for expected scenarios.

### Implementing a repository

Extend `RepositoryBase<TCommon, TDb, T, TKey>` to get the interceptor wiring for free, then add domain-specific query methods.

The four type parameters are:
- `TCommon` — shared abstraction implemented by both the DB model and the domain model (typically an interface).
- `TDb` — the EF Core / persistence entity (must implement `TCommon`, `IMapper<TCommon>`, and `IIdentifiable<TKey>`).
- `T` — the domain model returned to callers (must implement `TCommon` and `IMapper<TCommon>`).
- `TKey` — the key type (must be a value type, e.g., `Guid`).

```csharp
// Shared abstraction
public interface IOrderCommon
{
    string Reference { get; }
    string Status { get; }
}

// Domain model
public sealed class Order : IOrderCommon, IMapper<IOrderCommon> { ... }

// EF Core entity
public sealed class OrderEntity : IOrderCommon, IMapper<IOrderCommon>, IIdentifiable<Guid> { ... }

public sealed class OrderRepository(AppDbContext db, IEntityInterceptorFactory interceptors)
    : RepositoryBase<IOrderCommon, OrderEntity, Order, Guid>(interceptors), IOrderRepository
{
    // Implement the required OnAddAsync, OnUpdateAsync, OnFindAsync, OnDeleteAsync,
    // OnGetPagedAsync, OnUpdate, OnReloadEntityAsync, and IsHandled members.
}
```

Register the repository in DI as scoped:

```csharp
services.AddScoped<IOrderRepository, OrderRepository>();
```

---

## Paged queries

`GetPagedAsync<TRequest>` on the repository applies your registered filters and returns a paged result. Pass any request that implements `IPagedQuery` (which `PagedUnitResultRequestBase<T>` already does).

```csharp
var pagedResult = await orders.GetPagedAsync(request, cancellationToken);
// pagedResult.Items   — the page of entities
// pagedResult.PagedQuery — applied PageSize / PageIndex
```

See [Filters and paging](filters-and-paging.md) for how filters are applied.

---

## IScopedResources

`IScopedResources` is a type-keyed bag registered as a scoped DI service. It lets interceptors and the post-processor share execution-scoped objects — most commonly the `DbContext` instance and staged outbox messages — without constructor-injecting them into every child component.

```csharp
public interface IScopedResources
{
    void AddOrUpdate<T>(T value);
    bool TryGetScopedResource<T>(out T? value);
    T? GetScopedResource<T>();
    bool Contains<T>();
    bool TryRemove<T>(out T? oldValue);
    IReadOnlyDictionary<Type, object?> Items { get; }
}
```

> **Warning:** `IScopedResources` is not a service locator. Store only objects that have already been resolved through normal DI. Do not use it to pull services on demand.

`AddInterceptors(assembly)` registers `IScopedResources` (as `DefaultScopedResources`) automatically.

---

## DatabaseConfiguration

`DatabaseConfiguration` is a configuration helper for binding connection strings from `IConfiguration`. Use it with the EF Core setup:

```csharp
var config = builder.Configuration
    .GetSection("Database")
    .Get<DatabaseConfiguration>()!;

services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(config.ConnectionString));
```

---

## EF Core-specific helpers

`IDFCR.Abstractions.Persistence.Interceptors` contains `SoftDeletionEntityInterceptor`, which marks entities with a `DeletedAt` timestamp instead of removing them from the database. Register it with `AddInterceptors`.

`MaximumLengthStringExpressionBuilder` helps construct EF Core LINQ predicates that filter by maximum string length — useful in filters that enforce column constraints.

---

## IHasRowVersion

Implement `IHasRowVersion` on your entities to get optimistic concurrency checking via EF Core's `[Timestamp]` / `rowversion` column. The repository base will pass the row version to the context entry, triggering a `DbUpdateConcurrencyException` if the record has changed since it was read.

---

## Further reading

- [Filters and paging](filters-and-paging.md) — how `GetPagedAsync` applies filters.
- [Interceptors](interceptors.md) — entity-lifecycle interception registered alongside the repository.
- [Delta operations](delta-operations.md) — syncing many-to-many relationships in a single operation.
- [Outbox and dispatch](outbox-and-dispatch.md) — persisting messages as part of the same commit.
