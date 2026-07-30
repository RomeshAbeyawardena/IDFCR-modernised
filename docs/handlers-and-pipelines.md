# Handlers and pipelines

This page explains the MediatR request/handler interfaces that IDFCR provides and the built-in pipeline behaviors that sit between a `Send` call and your handler code.

**Packages:** `IDFCR.Abstractions.Mediator`, `IDFCR.Abstractions.Mediator.Extensions`

---

## Request interfaces

IDFCR wraps MediatR's `IRequest<T>` with typed result variants.

| Interface | Use when |
|---|---|
| `IUnitResultRequest<T>` | Command or query that returns a single typed result |
| `IUnitResultRequest` | Command that returns an untyped success/failure result |
| `IUnitResultCollectionRequest<T>` | Query that returns a non-paged collection |
| `IPagedUnitResultRequest<T>` | Query that returns a paged collection |

```csharp
// Single result
public sealed record GetOrderQuery(Guid OrderId) : IUnitResultRequest<OrderDto>;

// Collection (non-paged)
public sealed record GetOrdersByStatusQuery(string Status) : IUnitResultCollectionRequest<OrderDto>;

// Paged
public sealed record GetOrdersQuery : PagedUnitResultRequestBase<OrderDto>, IUnitOfWorkRequest;
```

`PagedUnitResultRequestBase<T>` provides `PageSize` and `PageIndex` properties. Implement `IPagedQuery` directly if you need a different shape.

---

## Handler interfaces

| Interface | Paired with |
|---|---|
| `IUnitResultRequestHandler<TReq, TResp>` | `IUnitResultRequest<TResp>` |
| `IUnitResultRequestHandler<TReq>` | `IUnitResultRequest` |
| `IUnitResultCollectionRequestHandler<TReq, TResp>` | `IUnitResultCollectionRequest<TResp>` |
| `IPagedUnitResultCollectionRequestHandler<TReq, TResp>` | `IPagedUnitResultRequest<TResp>` |

All of these are thin wrappers over MediatR's `IRequestHandler<TRequest, TResponse>`. MediatR resolves them from the DI container in the normal way.

```csharp
public sealed class GetOrderQueryHandler(IOrderRepository orders)
    : IUnitResultRequestHandler<GetOrderQuery, OrderDto>
{
    public async Task<IUnitResult<OrderDto>> Handle(
        GetOrderQuery request,
        CancellationToken cancellationToken)
    {
        var result = await orders.FindAsync(request.OrderId, cancellationToken);

        if (!result.IsSuccess)
            return result;  // propagate failure

        return UnitResult.FromResult(result.Result, UnitAction.Get);
    }
}
```

---

## Built-in pipeline behaviors

### 1. GenericDefaultExceptionPipeline

`GenericDefaultExceptionPipeline<TRequest, TResponse, TException>` is a MediatR `IRequestExceptionHandler`. It catches any exception that escapes a handler and converts it into a properly typed `IUnitResult*` response.

- For `IUnitResult<T>` responses: calls `UnitResult.Failed<T>(exception, ...)`.
- For `IUnitResultCollection<T>` responses: calls `UnitResultCollection.Failed<T>(exception, ...)`.
- For `IPagedUnitResult<T>` responses: calls `PagedUnitResult.FromResult<T>(empty, ...)`.

The `IExceptionBehaviourManager` controls which `UnitAction` and `FailureReason` are associated with each exception type. The `GenericDefaultExceptionPipeline` is registered automatically by `AddMediatorServicesAndPipelines`.

```csharp
services.ConfigureExceptionBehaviourManager(b =>
{
    b.SetFluentValidationBehaviours();   // ValidationException → FailureReason.ValidationError
    b.SetDefault(new ExceptionBehaviour(UnitAction.None, FailureReason.InternalError));
});
```

If an `ISaferExceptionProvider` is registered, the pipeline consults it to convert raw exceptions into safer representations before embedding them in the result (useful for hiding sensitive stack traces from clients).

### 2. ValidationPipeline

`ValidationPipeline<TRequest, TResponse>` runs all registered `IValidator<TRequest>` instances before the handler. If any validator fails, it throws a `FluentValidation.ValidationException`, which `GenericDefaultExceptionPipeline` then catches and converts to a `FailureReason.ValidationError` result.

Enable it via `AddMediatorServicesAndPipelines`:

```csharp
services.AddMediatorServicesAndPipelines(
    configuration,
    configureOptions: o => o.UseFluentValidation(),
    assemblies: typeof(Program).Assembly);
```

Register your validators with the DI container explicitly. The IDFCR pipeline consumes registered `IValidator<T>` instances; it does not scan for them automatically. Use FluentValidation's own helper:

```csharp
services.AddValidatorsFromAssembly(typeof(Program).Assembly);
```

### 3. UnitOfWorkPostPipelineProcessor

`UnitOfWorkPostPipelineProcessor<TRequest, TResponse>` is a MediatR `IRequestPostProcessor`. After the handler returns, it:

1. Checks whether the request implements `IUnitOfWorkRequest` and `CommitChanges == true`.
2. Checks whether the response is a successful `IUnitResult`.
3. If both conditions hold, calls `IUnitOfWork.SaveChangesAsync`.
4. If `IOutboxEntityNotificationHandler` and `IScopedResources` are registered, it processes staged outbox messages as part of the same commit cycle.

> **Important:** Do not call `SaveChangesAsync` inside your handler. The post-processor is responsible for the commit. Calling `SaveChangesAsync` early can leave the system in a partially committed state and make rollback impossible.

Enable it via `AddMediatorServicesAndPipelines`:

```csharp
services.AddMediatorServicesAndPipelines(
    configuration,
    configureOptions: o => o.UseUnitOfWorkPostPipeline(),
    assemblies: typeof(Program).Assembly);
```

---

## Registering services

`AddMediatorServicesAndPipelines` is the single registration call for all MediatR-related services.

```csharp
services
    .ConfigureExceptionBehaviourManager(b => b.SetFluentValidationBehaviours())
    .AddMediatorServicesAndPipelines(
        configuration,
        configureMediatr: cfg =>
        {
            // Any standard MediatR configuration goes here
        },
        configureOptions: o =>
        {
            o.UseFluentValidation();
            o.UseUnitOfWorkPostPipeline();
        },
        assemblies: typeof(Program).Assembly);
```

`ConfigureExceptionBehaviourManager` must be called before `AddMediatorServicesAndPipelines`; an `InvalidOperationException` is thrown if the exception behaviour manager is not registered first.

---

## IUnitOfWorkRequest

Implement `IUnitOfWorkRequest` on any request that should trigger a commit. Set `CommitChanges = true` (it defaults to `true` on most base implementations) to indicate that the handler's side effects should be saved.

```csharp
public sealed record CreateOrderCommand(string Reference)
    : IUnitResultRequest<OrderDto>, IUnitOfWorkRequest
{
    public bool CommitChanges => true;
}
```

---

## Further reading

- [Validation](validation.md) — FluentValidation integration details.
- [Persistence and unit of work](persistence-and-unit-of-work.md) — `IUnitOfWork` and `IRepository`.
- [Results and flow](results-and-flow.md) — `IUnitResult` types and factory methods.
