# Filters and paging

Filters provide a composable, assembly-scanned way to narrow repository queries. They work alongside `IRepository.GetPagedAsync` to apply request-specific predicates without polluting repository implementations with conditional logic.

**Packages:** `IDFCR.Abstractions.Filters`, `IDFCR.Abstractions.Persistence`

---

## The problem filters solve

Without a filter abstraction, every paged query requires branching inside the repository:

```csharp
// Without filters — hard to maintain at scale
if (request.Status is not null)
    query = query.Where(o => o.Status == request.Status);
if (request.CustomerId.HasValue)
    query = query.Where(o => o.CustomerId == request.CustomerId);
```

With filters, each concern lives in its own class and the repository remains clean.

---

## IFilter

The low-level interfaces in `IDFCR.Abstractions.Metadata`:

```csharp
public interface IFilter<TDb>
{
    bool CanFilter(object? request);
    IQueryable<TDb> Apply(IQueryable<TDb> queryable, object? request);
}

public interface IFilter<TRequest, TDb> : IFilter<TDb>
{
    bool CanFilter(TRequest request);
    IQueryable<TDb> Apply(IQueryable<TDb> queryable, TRequest request);
}
```

`CanFilter` lets a filter opt out for a specific request (for example, when an optional field is not supplied).

---

## FilterBase

`FilterBase<TRequest, TDb>` in `IDFCR.Abstractions.Filters` is the recommended base class. It:

- Provides a `StarterExpression` (`LinqKit.ExpressionStarter<TDb>`) for composing predicates without null-guarding.
- Defaults `CanFilter` to `true` (opt in by default; override to opt out).

```csharp
using IDFCR.Abstractions.Filters;

public sealed class OrderStatusFilter : FilterBase<GetOrdersQuery, Order>
{
    public override bool CanFilter(GetOrdersQuery request)
        => !string.IsNullOrWhiteSpace(request.Status);

    public override IQueryable<Order> Apply(IQueryable<Order> queryable, GetOrdersQuery request)
        => queryable.Where(o => o.Status == request.Status);
}
```

---

## DefaultPagedFilter

`DefaultPagedFilter` applies standard `Skip` / `Take` paging to a queryable based on the `PageSize` and `PageIndex` properties of the request. The repository base uses it automatically when calling `GetPagedAsync`.

You do not need to write paging logic in your own filters. `DefaultPagedFilter` is registered alongside your custom filters.

---

## IFilterFactory

`IFilterFactory` resolves the applicable filters for a given request and applies them to a queryable in sequence.

```csharp
public interface IFilterFactory
{
    IQueryable<TDb> Apply<TRequest, TDb>(IQueryable<TDb> queryable, TRequest request);
}
```

`DefaultFilterFactory` discovers registered `IFilter<TDb>` services from the DI container, calls `CanFilter`, and applies matching filters in registration order.

---

## Registering filters

### Manual registration

```csharp
services.AddTransient<IFilter<Order>, OrderStatusFilter>();
services.AddTransient<IFilter<Order>, OrderCustomerFilter>();
```

### Assembly scanning

Use the `ScanFilters` extension method from `IDFCR.Abstractions.Filters.Extensions`:

```csharp
using IDFCR.Abstractions.Filters.Extensions;

services.ScanFilters(registerGlobalPagingFilter: true, typeof(Program).Assembly);
```

This scans the supplied assemblies for non-global `IFilter` implementations, registers `DefaultFilterFactory`, and optionally registers the built-in `DefaultPagedFilter` for paging.

---

## GlobalFilterAttribute

Mark a filter class with `[GlobalFilter]` to make it apply to all requests of the matching `TDb` type, regardless of `TRequest`. This is useful for cross-cutting concerns such as tenant isolation or soft-deletion exclusion.

```csharp
[GlobalFilter]
public sealed class ExcludeSoftDeletedOrdersFilter : FilterBase<object, Order>
{
    public override IQueryable<Order> Apply(IQueryable<Order> queryable, object request)
        => queryable.Where(o => o.DeletedAt == null);
}
```

---

## Paged queries

`IPagedQuery` is implemented by `PagedUnitResultRequestBase<T>` and any custom class that carries paging information:

```csharp
public interface IPagedQuery
{
    int? PageSize { get; }
    int? PageIndex { get; }
}
```

When the handler passes the request to `GetPagedAsync`, `DefaultPagedFilter` translates these values into `Skip(PageIndex * PageSize).Take(PageSize)`.

```csharp
public sealed record GetOrdersQuery(string? Status, int? PageSize, int? PageIndex)
    : IPagedUnitResultRequest<OrderDto>, IPagedQuery;
```

---

## Sorted queries

Implement `IOrderedRequest` or `IStructuredOrderedRequest` on your request to carry sort fields. The `DefaultSort` and `StructuredOrderedRequestBase` types provide a ready-made implementation. Filters can read the sort instructions and apply `OrderBy`/`OrderByDescending` accordingly.

---

## Example: composing multiple filters

```csharp
// Filter 1 — by status
public sealed class OrderStatusFilter : FilterBase<GetOrdersQuery, Order>
{
    public override bool CanFilter(GetOrdersQuery r) => r.Status is not null;
    public override IQueryable<Order> Apply(IQueryable<Order> q, GetOrdersQuery r)
        => q.Where(o => o.Status == r.Status);
}

// Filter 2 — by date range
public sealed class OrderDateFilter : FilterBase<GetOrdersQuery, Order>
{
    public override bool CanFilter(GetOrdersQuery r) => r.FromDate.HasValue || r.ToDate.HasValue;
    public override IQueryable<Order> Apply(IQueryable<Order> q, GetOrdersQuery r)
    {
        var predicate = StarterExpression;
        if (r.FromDate.HasValue) predicate = predicate.And(o => o.CreatedAt >= r.FromDate.Value);
        if (r.ToDate.HasValue)   predicate = predicate.And(o => o.CreatedAt <= r.ToDate.Value);
        return q.Where(predicate);
    }
}
```

Both filters are registered by assembly scanning and applied automatically by `DefaultFilterFactory` when `GetPagedAsync` is called.

---

## Further reading

- [Persistence and unit of work](persistence-and-unit-of-work.md) — `GetPagedAsync` and how filters are applied.
- [Validation](validation.md) — `AbstractPagedQueryValidator` for validating paging parameters.
