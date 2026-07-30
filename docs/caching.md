# Caching

IDFCR's caching layer provides a group-keyed distributed cache with optional auditing and MessagePack serialisation. It sits on top of `IDistributedCache` and introduces the concept of a *group*: a named set of related cache entries that you can invalidate all at once.

**Packages:** `IDFCR.Caching.Http`, `IDFCR.Caching.Serialisation`, `IDFCR.Caching` (core group abstractions), `IDFCR.Abstractions.Caching`

---

## Why grouped caching?

A standard `IDistributedCache` stores key-value pairs. When source data changes, you need to know — and invalidate — every derived cache key. That becomes brittle when keys are computed from multiple inputs (tenant ID, filter parameters, etc.).

Grouped caching inverts this: you assign cache entries to a named group (e.g., `"orders"`) and invalidate the whole group when the data changes. You do not need to track individual keys.

---

## IDistributedGroupCache

```csharp
public interface IDistributedGroupCache
{
    Task<byte[]?> GetAsync(string groupKey, string compositeKey, CancellationToken cancellationToken);
    Task<byte[]?> GetAsync(string groupKey, string compositeKey,
        Func<string, string, string>? format, CancellationToken cancellationToken);

    Task SetAsync(string groupKey, string compositeKey, byte[] data, CancellationToken cancellationToken);
    Task SetAsync(string groupKey, string compositeKey,
        Func<string, string, string>? format, byte[] data, CancellationToken cancellationToken);

    Task<bool> RemoveAsync(string group, CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetCacheKeysAsync(string groupKey, CancellationToken cancellationToken);
}
```

| Method | Purpose |
|---|---|
| `GetAsync` | Read a single entry by group + composite key |
| `SetAsync` | Write a single entry under a group + composite key |
| `RemoveAsync` | Invalidate the entire group (all composite keys) |
| `GetCacheKeysAsync` | List all composite keys currently tracked in a group |

---

## Registration

### Without auditing

```csharp
services.AddGroupedDistributedCache();
// or, with in-memory options:
services.AddGroupedDistributedCache(opts => opts.SizeLimit = 1024 * 1024);
```

If no `IDistributedCache` implementation is already registered, an in-memory distributed cache is added automatically. When deploying to production, register your preferred `IDistributedCache` (e.g., Redis) before calling `AddGroupedDistributedCache`.

### With logger-based auditing

```csharp
services.AddGroupedDistributedCacheWithLogAuditing();
```

### With a custom audit sink

```csharp
services.AddGroupedDistributedCache<MyAuditSink>();
```

Where `MyAuditSink : IDistributedGroupCacheAuditSink`.

---

## Basic usage pattern

```csharp
public sealed class OrderCacheService(IDistributedGroupCache cache)
{
    private const string Group = "orders";

    public async Task<byte[]?> GetAsync(string tenantId, string status, CancellationToken ct)
    {
        var compositeKey = $"{tenantId}:{status}";
        return await cache.GetAsync(Group, compositeKey, ct);
    }

    public async Task SetAsync(string tenantId, string status, byte[] data, CancellationToken ct)
    {
        var compositeKey = $"{tenantId}:{status}";
        await cache.SetAsync(Group, compositeKey, data, ct);
    }

    public Task InvalidateAsync(CancellationToken ct)
        => cache.RemoveAsync(Group, ct);
}
```

---

## Serialisation with MessagePack

`IDFCR.Caching.Serialisation` provides `DeserialiseAsync<T>` on `byte[]` via an extension method:

```csharp
using IDFCR.Caching.Serialisation.Extensions;
using MessagePack;

var serializerOptions = MessagePackSerializerOptions.Standard
    .WithCompression(MessagePackCompression.Lz4BlockArray);

// Deserialise
var cached = await cache.GetAsync(Group, compositeKey, ct);
if (cached is not null)
{
    var orders = await cached.DeserialiseAsync<List<OrderDto>>(serializerOptions, ct);
    return orders;
}

// Serialise before writing
var bytes = MessagePackSerializer.Serialize(orders, serializerOptions);
await cache.SetAsync(Group, compositeKey, bytes, ct);
```

Register `MessagePackSerializerOptions` as a singleton so it is shared across the application:

```csharp
services.AddSingleton(
    MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray));
```

---

## Auditing

When auditing is enabled (via `AddGroupedDistributedCacheWithLogAuditing` or a custom sink), every `GetAsync`, `SetAsync`, and `RemoveAsync` call emits an `IDistributedCacheEvent` with:

- The operation type (`Get`, `Set`, `Remove`).
- The group key and composite key.
- Whether the operation resulted in a cache hit or miss.
- A timestamp.

The default `LoggerDistributedGroupCacheAuditSink` writes these events to `ILogger`. Implement `IDistributedGroupCacheAuditSink` to route them elsewhere (e.g., a metrics sink).

---

## IDistributedCacheGroups

`IDistributedCacheGroups` (from `IDFCR.Abstractions.Caching`) manages the group-to-key index stored in `IDistributedCache`. You do not need to use this interface directly; `IDistributedGroupCache` delegates to it internally.

---

## Tips

- Use short, stable group keys (`"orders"`, `"clients"`). Group keys should correspond to a data entity, not a query shape.
- Use composite keys to differentiate entries within a group (`"tenantId:status"`). Include all dimensions that affect the result.
- Invalidate at write time, not read time. When a handler modifies orders, call `InvalidateAsync("orders")` before or after the commit.
- In production, register a real distributed cache (Redis, SQL Server) before calling `AddGroupedDistributedCache` so the cache is shared across instances.

---

## Further reading

- [Persistence and unit of work](persistence-and-unit-of-work.md) — where cache invalidation fits in the write flow.
- [Handlers and pipelines](handlers-and-pipelines.md) — handler patterns that complement caching.
