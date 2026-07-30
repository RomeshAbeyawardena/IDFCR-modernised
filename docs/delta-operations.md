# Delta operations

A delta describes a *change* to a collection: which items to add and which to remove. IDFCR provides `StringListDelta` and `PerformDeltaAsync` to sync many-to-many relationships atomically in a single EF Core operation.

**Packages:** `IDFCR.Abstractions.Metadata` (types), `IDFCR.Persistence.EntityFrameworkCore.Extensions` (EF Core implementation)

---

## The problem deltas solve

When a client sends a "tags" update — `{ add: ["urgent", "billing"], remove: ["draft"] }` — you need to:

1. Look up which tags already exist.
2. Create tags that do not exist yet.
3. Add join-table rows for new relationships.
4. Remove join-table rows for removed relationships.
5. Do all of this without duplicating rows or orphaning entities.

Writing that logic by hand is error-prone and repetitive. `PerformDeltaAsync` handles all of it.

---

## IStringListDelta / StringListDelta

```csharp
public interface IStringListDelta
{
    IEnumerable<string> Add { get; }
    IEnumerable<string> Remove { get; }
}

public record StringListDelta : IStringListDelta
{
    public IEnumerable<string> Add { get; init; } = [];
    public IEnumerable<string> Remove { get; init; } = [];
}
```

Use `StringListDelta` in commands and DTOs wherever a collection change is expressed by name:

```csharp
public sealed record UpdateOrderTagsCommand(
    Guid OrderId,
    StringListDelta Tags) : IUnitResultRequest<OrderDto>, IUnitOfWorkRequest
{
    public bool CommitChanges => true;
}
```

---

## PerformDeltaAsync

`DeltaExtensions.PerformDeltaAsync` is an extension method on `IStringListDelta` that operates against a `DbContext`.

```csharp
Task<RelationshipDeltaResult> PerformDeltaAsync<TEntity, TJoinEntity, TParentKey, TChildKey>(
    this IStringListDelta delta,
    DbContext context,
    TParentKey parentId,
    NamedDeltaOptions<TEntity, TJoinEntity, TParentKey, TChildKey> options,
    CancellationToken cancellationToken = default)
```

| Parameter | Purpose |
|---|---|
| `delta` | The `IStringListDelta` containing additions and removals |
| `context` | The `DbContext` to operate against |
| `parentId` | The ID of the parent entity (e.g., the order that owns the tags) |
| `options` | Configuration describing entity lookup, creation, and join-table mapping |

### NamedDeltaOptions

`NamedDeltaOptions<TEntity, TJoinEntity, TParentKey, TChildKey>` configures how `PerformDeltaAsync` resolves and creates entities and join rows:

| Property / Delegate | Purpose |
|---|---|
| `GetEntityName` | Extract the name from an entity (e.g., `tag => tag.Name`) |
| `NormalizeName` | Normalise names before comparison (e.g., `s => s.Trim().ToLowerInvariant()`) |
| `NameComparer` | String comparer for deduplication (e.g., `StringComparer.OrdinalIgnoreCase`) |
| `FilterEntitiesByNames` | Query the entity set by a list of names |
| `CreateNewEntity` | Factory for creating a new entity by name |
| `GetEntityId` | Get the child entity's key (e.g., `tag => tag.Id`) |
| `FilterExistingJoins` | Query the join set for existing rows by parent and child IDs |
| `CreateNewJoin` | Factory for creating a new join row |
| `GetJoinChildId` | Get the child ID from a join row |
| `IsLocalJoin` | Determines whether a locally tracked join row matches a parent + entity pair |

### RelationshipDeltaResult

The return value reports what happened:

```csharp
public record RelationshipDeltaResult(
    int EntitiesCreated,
    int RelationshipsAdded,
    int RelationshipsRemoved);
```

---

## Full example

```csharp
// Entities
public sealed class Order { public Guid Id { get; set; } /* ... */ }
public sealed class Tag   { public Guid Id { get; set; } public string Name { get; set; } = ""; }
public sealed class OrderTag { public Guid OrderId { get; set; } public Guid TagId { get; set; } }

// Handler
public sealed class UpdateOrderTagsCommandHandler(AppDbContext db)
    : IUnitResultRequestHandler<UpdateOrderTagsCommand, OrderDto>
{
    public async Task<IUnitResult<OrderDto>> Handle(
        UpdateOrderTagsCommand request,
        CancellationToken cancellationToken)
    {
        var options = new NamedDeltaOptions<Tag, OrderTag, Guid, Guid>
        {
            GetEntityName    = tag => tag.Name,
            NormalizeName    = s => s.Trim().ToLowerInvariant(),
            NameComparer     = StringComparer.OrdinalIgnoreCase,
            FilterEntitiesByNames = (set, names) => set.Where(t => names.Contains(t.Name.ToLower())),
            CreateNewEntity  = name => new Tag { Id = Guid.NewGuid(), Name = name },
            GetEntityId      = tag => tag.Id,
            FilterExistingJoins = (set, parentId, childIds) =>
                set.Where(ot => ot.OrderId == parentId && childIds.Contains(ot.TagId)),
            CreateNewJoin    = (parentId, tag) => new OrderTag { OrderId = parentId, TagId = tag.Id },
            GetJoinChildId   = ot => ot.TagId,
            IsLocalJoin      = (ot, parentId, tag) => ot.OrderId == parentId && ot.TagId == tag.Id,
        };

        var deltaResult = await request.Tags.PerformDeltaAsync(
            db, request.OrderId, options, cancellationToken);

        // SaveChangesAsync is called by UnitOfWorkPostPipelineProcessor after this returns
        var order = await db.Orders.FindAsync(request.OrderId, cancellationToken);
        return UnitResult.FromResult(order!.ToDto(), UnitAction.Update);
    }
}
```

---

## Semantics

`PerformDeltaAsync` uses **add-wins** semantics: if the same name appears in both `Add` and `Remove`, the add takes precedence. This prevents accidental removal during concurrent updates.

Normalisation is applied consistently to both the delta values and database-loaded entity names, ensuring that `"Urgent"` and `"urgent"` are treated as the same tag regardless of the stored capitalisation.

---

## Further reading

- [Persistence and unit of work](persistence-and-unit-of-work.md) — `DbContext` and `IUnitOfWork`.
- [Handlers and pipelines](handlers-and-pipelines.md) — `IUnitOfWorkRequest` and when `SaveChangesAsync` is called.
