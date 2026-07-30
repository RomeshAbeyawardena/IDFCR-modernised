# HTTP and gRPC

IDFCR provides thin adapters that convert `IUnitResult*` values into HTTP responses (for Minimal APIs and MVC) and gRPC status codes.

**Packages:** `IDFCR.Results.Http`, `IDFCR.Results.Http.Grpc`, `IDFCR.GRPC.Client.Extensions`, `IDFCR.Abstractions.GRPC`, `IDFCR.Abstractions.GRPC.Extensions`, `IDFCR.Abstractions.GRPC.HostExtensions`

---

## HTTP result mapping

### IUnitHttpResult

`IUnitHttpResult` implements ASP.NET Core's `IResult`, so it is a natural return type for Minimal API endpoints. MVC controllers can also return it as `IResult`, though many MVC codebases prefer wrapping responses as `IActionResult`.

```csharp
public interface IUnitHttpResult : IResult
{
    // Delegates to IResult.ExecuteAsync, selecting the HTTP status code
    // based on IUnitResult.IsSuccess, FailureReason, and UnitAction.
}
```

### AsHttp extension methods

Call `.AsHttp()` on any `IUnitResult*` to get an `IUnitHttpResult`:

```csharp
using IDFCR.Results.Http.Extensions;

// Single result
IUnitHttpResult http = result.AsHttp();

// Typed result
IUnitHttpResult http = result.AsHttp<OrderDto>();

// Collection result
IUnitHttpResult http = collectionResult.AsHttp<OrderDto>();

// Chained result
IUnitHttpResult http = chainedResult.AsChainedHttp();

// Chained typed result
IUnitHttpResult http = chainedResult.AsChainedHttp<OrderDto>();
```

### Status code mapping

`UnitHttpResult` maps `FailureReason` to HTTP status codes as follows. `UnitAction` does **not** influence the status code — the body JSON always contains the action value for the caller to inspect.

| `FailureReason` | HTTP status |
|---|---|
| `None` (success) | 200 OK |
| `ValidationError` | 400 Bad Request |
| `AuthorizationError` / `Unauthorized` | 401 Unauthorized |
| `Forbidden` | 403 Forbidden |
| `NotFound` | 404 Not Found |
| `Conflict` | 409 Conflict |
| `ExternalDependencyError` | 424 Failed Dependency |
| `InternalError` | 500 Internal Server Error |
| `Unknown` | 503 Service Unavailable |
| `None` (default) | 200 OK |

### Minimal API example

```csharp
using IDFCR.Results.Http.Extensions;

app.MapPost("/orders", async (
    CreateOrderCommand cmd,
    IMediator mediator,
    CancellationToken ct) =>
{
    var result = await mediator.Send(cmd, ct);
    return result.AsHttp();
});

app.MapGet("/orders/{id:guid}", async (
    Guid id,
    IMediator mediator,
    CancellationToken ct) =>
{
    var result = await mediator.Send(new GetOrderQuery(id), ct);
    return result.AsHttp();
});
```

### MVC example

```csharp
[ApiController]
[Route("api/orders")]
public sealed class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IResult> Create(CreateOrderCommand cmd, CancellationToken ct)
        => (await mediator.Send(cmd, ct)).AsHttp();
}
```

---

## gRPC integration

### Result extensions

`IDFCR.Abstractions.GRPC.Extensions` provides `UnitResultExtensions` for mapping `IUnitResult*` to gRPC `Status` and for converting `StringListDelta` values across the gRPC boundary.

```csharp
using IDFCR.Abstractions.GRPC.Extensions;

// Map a unit result to a gRPC status
var status = result.ToGrpcStatus();

// Map a StringListDelta proto message to the domain type
IStringListDelta delta = protoMessage.ToStringListDelta();
```

`IDFCR.Results.Http.Grpc` extends the HTTP result mapping for gRPC-transcoded endpoints.

### Assembly-scanned gRPC service hosting

`IDFCR.Abstractions.GRPC.HostExtensions` provides `DiscoverGRPCServices` on `WebApplication`, which discovers all classes decorated with `[RegisteredGRPCServiceImplementation]` and maps them as gRPC endpoints.

```csharp
// Attribute marks a gRPC service implementation for discovery
[RegisteredGRPCServiceImplementation]
public sealed class OrderGrpcService : Orders.OrdersBase
{
    // ...
}

// In app setup (after builder.Build())
app.DiscoverGRPCServices(builder.Configuration, typeof(OrderGrpcService).Assembly);
```

`IRegisteredGRPCServiceImplementationTypeDiscoveryService` is the interface that `DiscoverGRPCServices` uses internally; you can inject it to discover registered service types programmatically.

### gRPC client factory extensions

`IDFCR.GRPC.Client.Extensions` extends `IHttpClientBuilder` with helpers for configuring gRPC channels with service discovery:

```csharp
services
    .AddGrpcClient<Orders.OrdersClient>(o =>
    {
        o.Address = new Uri("https://order-service");
    })
    .AddServiceDiscovery();
```

### Protobuf contracts

Shared `.proto` definitions in `IDFCR.Abstractions.GRPC.Contracts` include common message types (`UnitResult`, `UnitAction`, `FailureReason`, `StringListDelta`) so cross-service contracts stay consistent. `IDFCR.Abstractions.GRPC.Generated` contains the pre-generated C# code.

---

## SortOrder mapping

`SortOrderExtensions` (in `IDFCR.Abstractions.GRPC.Extensions`) maps between the framework's `OrderDirection` enum and the gRPC `SortOrder` enum, keeping sort direction consistent across the HTTP and gRPC boundaries.

---

## Further reading

- [Results and flow](results-and-flow.md) — the full `IUnitResult*` model and `FailureReason`.
- [Handlers and pipelines](handlers-and-pipelines.md) — where results come from before mapping.
