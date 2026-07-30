# HTTP and gRPC

IDFCR provides thin adapters that convert `IUnitResult*` values into HTTP responses (for Minimal APIs and MVC) and gRPC status codes.

**Packages:** `IDFCR.Results.Http`, `IDFCR.Results.Http.Grpc`, `IDFCR.GRPC.Client.Extensions`, `IDFCR.Abstractions.GRPC`, `IDFCR.Abstractions.GRPC.Extensions`, `IDFCR.Abstractions.GRPC.HostExtensions`

---

## HTTP result mapping

### IUnitHttpResult

`IUnitHttpResult` implements ASP.NET Core's `IResult`, so it can be returned directly from Minimal API endpoints and MVC action methods.

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

| Condition | HTTP status |
|---|---|
| `IsSuccess && Action == Add` | 201 Created |
| `IsSuccess && Action == Delete` | 204 No Content |
| `IsSuccess` (other actions) | 200 OK |
| `FailureReason.NotFound` | 404 Not Found |
| `FailureReason.ValidationError` | 422 Unprocessable Entity |
| `FailureReason.Conflict` | 409 Conflict |
| `FailureReason.Unauthorized` | 401 Unauthorized |
| `FailureReason.Forbidden` / `AuthorizationError` | 403 Forbidden |
| `FailureReason.NotSupported` | 415 / 501 (context-dependent) |
| `FailureReason.InternalError` | 500 Internal Server Error |
| `FailureReason.ExternalDependencyError` | 502 Bad Gateway |
| Other failures | 500 Internal Server Error |

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
app.DiscoverGRPCServices(configuration, typeof(OrderGrpcService).Assembly);
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
