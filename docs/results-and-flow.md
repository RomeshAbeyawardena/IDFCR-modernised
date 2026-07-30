# Results and flow

Results are the central contract of IDFCR. Every operation — whether it succeeds or fails — returns one of the `IUnitResult*` types. This page explains what those types are, when to use each, and how to construct and inspect them.

**Package:** `IDFCR.Abstractions.Results`

---

## Why explicit results?

When an operation fails, you have two options: throw an exception or return a value that encodes the failure. IDFCR takes the second approach for *expected* failures (validation errors, missing entities, conflicts). This makes failure handling predictable and avoids exception-driven control flow.

A caller can always inspect `result.IsSuccess`, `result.FailureReason`, and `result.Exception` without catching anything.

---

## Result interfaces

### `IUnitResult` — untyped result

The base interface. Every result implements it.

```csharp
public interface IUnitResult
{
    bool IsSuccess { get; }
    FailureReason? FailureReason { get; }
    FailureOrigin? FailureOrigin { get; }
    Exception? Exception { get; }
    UnitAction Action { get; }
    IReadOnlyDictionary<string, object?> Meta { get; }
    // ...
}
```

### `IUnitResult<T>` — typed result

Carries a value of type `T` alongside the success/failure state.

```csharp
public interface IUnitResult<TResult> : IUnitResult
{
    TResult? Result { get; }
    bool HasValue { get; }   // true when IsSuccess && Result is not null
    TResult? OriginalState { get; }
    TResult? ModifiedState { get; }
    string? NamedResult { get; }
}
```

### `IUnitResultCollection<T>` — collection result

A result that carries a collection of items.

### `IPagedUnitResult<T>` — paged result

A result that carries a paged collection along with a `IPagedQuery` describing the applied paging parameters.

### `IChainedUnitResult` — chained result

Groups multiple related operation results into a single traceable chain. Useful when a single logical operation touches multiple aggregates.

---

## `FailureReason`

`FailureReason` describes why an operation failed. IDFCR maps these to HTTP status codes via `.AsHttp()`.

| Value | Meaning | HTTP status (via `.AsHttp()`) |
|---|---|---|
| `None` | No failure | 200 OK |
| `ValidationError` | Input failed validation | 400 Bad Request |
| `AuthorizationError` / `Unauthorized` | Caller is not authenticated | 401 Unauthorized |
| `Forbidden` | Caller is authenticated but not permitted | 403 Forbidden |
| `NotFound` | Entity was not found | 404 Not Found |
| `Conflict` | Operation conflicted with current state | 409 Conflict |
| `ExternalDependencyError` | A dependency failed | 424 Failed Dependency |
| `InternalError` | Unexpected internal failure | 500 Internal Server Error |
| `Unknown` | Reason could not be determined | 503 Service Unavailable |

---

## `UnitAction`

`UnitAction` is a flags enum describing what kind of operation produced the result. It is included in the serialised response body so callers can determine the type of operation that ran. It does **not** affect the HTTP status code — all successful results return 200 OK regardless of action.

```csharp
[Flags]
public enum UnitAction
{
    None    = 0,
    Add     = 1,
    Get     = 2,
    Update  = 4,
    Delete  = 8,
    Pending = 16,
    Conflict = 32
}
```

---

## `FailureOrigin`

`FailureOrigin` tells you where a failure originated — useful for deciding whether to surface the exception detail to the caller.

| Value | Meaning |
|---|---|
| `Internal` | The failure originated inside IDFCR or the application itself |
| `Caller` | The failure was caused by invalid caller input |
| `Unknown` | Origin could not be determined |

Call `UnitResult.IsInternalFailure(result)` to check for internal failures. Call `UnitResult.ThrowIfInternalFailure(result)` to rethrow if you need to surface internal failures upstream.

---

## Creating results with `UnitResult`

`UnitResult` is a static factory class.

```csharp
// Success with a value
IUnitResult<OrderDto> ok = UnitResult.FromResult(order, UnitAction.Add);

// Explicit success without a value
IUnitResult ok = UnitResult.Success(UnitAction.Delete);

// Typed failure
IUnitResult<OrderDto> failed = UnitResult.Failed<OrderDto>(
    new InvalidOperationException("Order already shipped"),
    failureReason: FailureReason.Conflict);

// Not-found shortcut
IUnitResult<OrderDto> notFound = UnitResult.NotFound<OrderDto>(orderId);

// Untyped failure
IUnitResult failed = UnitResult.Failed(
    exception,
    UnitAction.None,
    FailureReason.InternalError,
    FailureOrigin.Internal);

// Full control
IUnitResult<OrderDto> custom = UnitResult.Create<OrderDto>(
    result: order,
    isSuccess: true,
    action: UnitAction.Add);
```

---

## Adding metadata

Results support an open metadata dictionary for cross-cutting concerns (e.g., correlation IDs, page counts, timing).

```csharp
var result = UnitResult.FromResult(order, UnitAction.Add)
    .AddMeta("correlationId", correlationId);

object? value = result.Meta["correlationId"];
```

---

## Chained results

Use `IChainedResultBuilder` when a single logical operation spans multiple sub-operations and you want to report all outcomes together.

```csharp
// Building a chained result is done via the builder registered in DI.
// Each Add call records a sub-result.
var chainedResult = builder
    .Add("createOrder", orderResult)
    .Add("createLineItem", lineItemResult)
    .Build();

// The chain is successful only when all recorded results are successful.
bool allOk = chainedResult.IsSuccess;
```

---

## Inspecting results safely

```csharp
var result = await mediator.Send(new GetOrderQuery(orderId), ct);

if (!result.IsSuccess)
{
    // result.FailureReason, result.Exception, result.FailureOrigin are available
    return result.AsHttp();   // maps to the right HTTP status code
}

// result.HasValue is true here
OrderDto order = result.Result!;
```

---

## Further reading

- [Handlers and pipelines](handlers-and-pipelines.md) — how results flow through MediatR pipelines.
- [HTTP and gRPC](http-and-grpc.md) — mapping results to transport responses.
- [Outbox and dispatch](outbox-and-dispatch.md) — encoding results in outbox messages.
