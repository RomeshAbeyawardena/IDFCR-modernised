# Getting started with IDFCR

This page walks through adding IDFCR to a new or existing .NET application.

**Prerequisites:** .NET 10 SDK, basic familiarity with C#, dependency injection, and ASP.NET Core.

---

## Adoption paths

IDFCR is modular. You do not need to install every package. Three common starting points:

1. **Results and handlers only** — add explicit outcomes to any MediatR application.
2. **Results, handlers, validation, and persistence** — add EF Core repositories and filter/paging support.
3. **Full application flow** — add interceptors, outbox dispatch, caching, transport, and optional AI/gRPC.

Start with path 1. Add capabilities as you need them.

---

## Path 1 — Results and handlers only

### Install packages

```xml
<PackageReference Include="IDFCR.Abstractions.Results" />
<PackageReference Include="IDFCR.Abstractions.Mediator" />
<PackageReference Include="IDFCR.Abstractions.Mediator.Extensions" />
```

Add the HTTP bridge if you are building an API:

```xml
<PackageReference Include="IDFCR.Results.Http" />
```

### Register services

Call `ConfigureExceptionBehaviourManager` before `AddMediatorServicesAndPipelines`. The exception behaviour manager controls how unexpected exceptions are translated into clean result responses.

```csharp
using IDFCR.Abstractions.Mediator.Extensions;

services
    .ConfigureExceptionBehaviourManager(b => b.SetFluentValidationBehaviours())
    .AddMediatorServicesAndPipelines(
        configuration,
        assemblies: typeof(Program).Assembly);
```

`SetFluentValidationBehaviours()` maps `ValidationException` to `FailureReason.ValidationError`. If you do not use FluentValidation yet, omit that call and configure a default behaviour instead:

```csharp
services.ConfigureExceptionBehaviourManager(b =>
    b.SetDefault(new ExceptionBehaviour(UnitAction.None, FailureReason.InternalError)));
```

### Write a command and handler

```csharp
using IDFCR.Abstractions.Mediator;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Results;

// The command is a plain record implementing IUnitResultRequest<T>
public sealed record CreateOrderCommand(string Reference) : IUnitResultRequest<OrderDto>;

// The handler returns IUnitResult<OrderDto>
public sealed class CreateOrderCommandHandler : IUnitResultRequestHandler<CreateOrderCommand, OrderDto>
{
    public Task<IUnitResult<OrderDto>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Reference))
            return Task.FromResult(
                UnitResult.Failed<OrderDto>(
                    new ArgumentException("Reference is required"),
                    failureReason: FailureReason.ValidationError));

        var order = new OrderDto(Guid.NewGuid(), request.Reference);
        return Task.FromResult(UnitResult.FromResult(order, UnitAction.Add));
    }
}
```

### Map the result to HTTP

```csharp
using IDFCR.Results.Http.Extensions;

app.MapPost("/orders", async (CreateOrderCommand cmd, IMediator mediator, CancellationToken ct) =>
{
    var result = await mediator.Send(cmd, ct);
    return result.AsHttp();
    // FailureReason.None (success)    → 200 OK  (body: the OrderDto)
    // FailureReason.ValidationError  → 400 Bad Request
    // FailureReason.NotFound         → 404 Not Found
});
```

See [Results and flow](results-and-flow.md) for the full mapping table and all result types.

---

## Path 2 — Adding validation and persistence

### Additional packages

```xml
<PackageReference Include="FluentValidation" />
<PackageReference Include="IDFCR.Abstractions.Persistence" />
<PackageReference Include="IDFCR.Abstractions.Filters" />
<PackageReference Include="IDFCR.Abstractions.Interceptors" />
<PackageReference Include="IDFCR.Abstractions.Interceptors.DependencyInjection" />
```

If you use EF Core:

```xml
<PackageReference Include="IDFCR.Persistence.EntityFrameworkCore" />
<PackageReference Include="IDFCR.Persistence.EntityFrameworkCore.Extensions" />
```

### Enable the validation pipeline

Pass `configureOptions` to `AddMediatorServicesAndPipelines` and enable the FluentValidation processor:

```csharp
services
    .ConfigureExceptionBehaviourManager(b => b.SetFluentValidationBehaviours())
    .AddMediatorServicesAndPipelines(
        configuration,
        configureOptions: o => o.UseFluentValidation(),
        assemblies: typeof(Program).Assembly);
```

Register validators alongside handlers in the same assembly. The FluentValidation pipeline will consume any `IValidator<T>` that is registered with the DI container. Register them explicitly:

```csharp
// Using FluentValidation's own registration helper:
services.AddValidatorsFromAssembly(typeof(Program).Assembly);
```

### Write a validator

```csharp
using FluentValidation;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Reference).NotEmpty().MaximumLength(100);
    }
}
```

### Register interceptors (for audit timestamps)

```csharp
using IDFCR.Abstractions.Interceptors.DependencyInjection.Extensions;

services.AddInterceptors(typeof(Program).Assembly);
```

`AddInterceptors` scans the supplied assemblies for `IEntityInterceptor` and `IAuditProcessor` implementations and registers them as transient services, along with `IScopedResources` (scoped) and `IEntityInterceptorFactory` (transient).

See [Interceptors](interceptors.md) and [Persistence and unit of work](persistence-and-unit-of-work.md) for further detail.

---

## Path 3 — Full application flow

See the individual pages for each capability:

- [Interceptors](interceptors.md) — entity-lifecycle processing and auditing.
- [Outbox and dispatch](outbox-and-dispatch.md) — reliable background message delivery.
- [Caching](caching.md) — grouped distributed cache with group-level invalidation.
- [HTTP and gRPC](http-and-grpc.md) — transport bridges and gRPC service hosting.
- [CLI and database updater](cli-and-database-updater.md) — running EF Core migrations.

For guidance on adding IDFCR to a codebase that already uses its own patterns, see [Migration and adoption](migration-and-adoption.md).

---

## Next steps

- [Architecture overview](architecture-overview.md) — understand how the layers compose.
- [Results and flow](results-and-flow.md) — learn the full result model.
- [Handlers and pipelines](handlers-and-pipelines.md) — understand what happens between a `Send` call and your handler.
