# IDFCR — Intent-Driven Flow Composition Runtime

IDFCR is a composable .NET toolkit for building handler-oriented applications with explicit, typed operation outcomes.

> **Install only what you need.** Every capability is an independent, opt-in package.

---

## What problem does IDFCR solve?

Most .NET applications share the same boilerplate: handler plumbing, result shapes that differ per endpoint, exception-driven control flow for expected failures, repetitive DI registration, and no standard way to compose infrastructure concerns such as auditing, outbox persistence, or distributed caching.

IDFCR provides a set of focused, composable packages that replace that boilerplate with consistent contracts and conventions, while keeping the framework out of your business logic.

---

## Key capabilities

| Capability | What you get |
|---|---|
| **Results** | `IUnitResult<T>`, `IUnitResultCollection<T>`, `IPagedUnitResult<T>`, `FailureReason` — a single result shape across all layers |
| **Handlers** | MediatR-compatible request/handler interfaces with typed results and an automatic exception pipeline |
| **Validation** | FluentValidation pipeline integration, paged-query base validators |
| **Persistence** | `IRepository<T, TKey>` + `IUnitOfWork` abstractions with optional EF Core implementations |
| **Filters** | Composable, assembly-scanned query filters with LinqKit predicate builders |
| **Interceptors** | Entity-lifecycle interceptors (audit timestamps, custom processing) registered via assembly scanning |
| **Delta operations** | `StringListDelta` + `PerformDeltaAsync` for efficient many-to-many relationship sync |
| **Outbox** | `IOutboxEntity`, `IOutboxPublisher`, `IOutboxReader`, `OutboxInterceptor` for reliable background dispatch |
| **Caching** | `IDistributedGroupCache` for group-keyed distributed cache with optional auditing and MessagePack serialisation |
| **HTTP mapping** | `.AsHttp()` converts any `IUnitResult*` to an `IResult` for Minimal APIs or MVC |
| **gRPC** | Assembly-scanned gRPC service registration and result-to-status-code mapping |
| **CLI** | `ICommandOperation` and `ICommandRouteDispatcher` for interactive or batch command-line tools |
| **Database updater** | Self-contained CLI host for running EF Core migrations via `ConfigureDatabaseUpdaterHost` |
| **AI** | `IAIService` + `IOpenAIService` contracts backed by HTTP or OpenAI providers |

---

## Why IDFCR?

- **Consistent results everywhere.** One result shape travels through handlers, HTTP responses, gRPC status codes, and outbox messages without translation ceremony.
- **Explicit failures, not exceptions.** Expected failures (`NotFound`, `ValidationError`, `Conflict`) are first-class result states, not thrown exceptions.
- **Low-boilerplate registration.** One extension method call — `AddInterceptors(assembly)`, `AddGroupedDistributedCache()`, `AddMediatorServicesAndPipelines(...)` — replaces pages of manual DI wiring.
- **Gradual adoption.** Start with just the results package. Add persistence, caching, or outbox only when you need them.

---

## Quick start

### 1. Add the core packages

```xml
<PackageReference Include="IDFCR.Abstractions.Results" />
<PackageReference Include="IDFCR.Abstractions.Mediator" />
<PackageReference Include="IDFCR.Abstractions.Mediator.Extensions" />
```

### 2. Register MediatR with IDFCR pipelines

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigureExceptionBehaviourManager(b => b.SetFluentValidationBehaviours())
    .AddMediatorServicesAndPipelines(builder.Configuration, assemblies: typeof(Program).Assembly);
```

### 3. Define a command and handler

```csharp
// Command (a MediatR request)
public sealed record CreateOrderCommand(string Reference) : IUnitResultRequest<OrderDto>;

// Handler
public sealed class CreateOrderCommandHandler : IUnitResultRequestHandler<CreateOrderCommand, OrderDto>
{
    public Task<IUnitResult<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
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

### 4. Map the result to HTTP (Minimal API)

```csharp
app.MapPost("/orders", async (CreateOrderCommand cmd, IMediator mediator, CancellationToken ct) =>
{
    var result = await mediator.Send(cmd, ct);
    return result.AsHttp();   // automatically maps FailureReason → HTTP status code
});
```

---

## Documentation

Full developer guide is in [`docs/`](docs/README.md).

| Section | Contents |
|---|---|
| [Getting started](docs/getting-started.md) | Installation, first handler, first HTTP endpoint |
| [Architecture overview](docs/architecture-overview.md) | How the packages fit together |
| [Results and flow](docs/results-and-flow.md) | `IUnitResult`, `FailureReason`, `UnitAction`, chained results |
| [Handlers and pipelines](docs/handlers-and-pipelines.md) | MediatR request types, exception pipeline, UoW post-processor |
| [Validation](docs/validation.md) | FluentValidation pipeline, `AbstractPagedQueryValidator` |
| [Persistence and unit of work](docs/persistence-and-unit-of-work.md) | `IRepository`, `IUnitOfWork`, EF Core |
| [Filters and paging](docs/filters-and-paging.md) | `FilterBase`, `IFilterFactory`, paged queries |
| [Interceptors](docs/interceptors.md) | `IEntityInterceptor`, audit interceptors, `AddInterceptors` |
| [Delta operations](docs/delta-operations.md) | `StringListDelta`, `PerformDeltaAsync`, many-to-many sync |
| [Outbox and dispatch](docs/outbox-and-dispatch.md) | `IOutboxEntity`, `IOutboxPublisher`, `OutboxInterceptor` |
| [Caching](docs/caching.md) | `IDistributedGroupCache`, group invalidation, serialisation |
| [HTTP and gRPC](docs/http-and-grpc.md) | `.AsHttp()`, gRPC result extensions, service hosting |
| [CLI and database updater](docs/cli-and-database-updater.md) | `ICommandOperation`, `ConfigureDatabaseUpdaterHost` |
| [AI integrations](docs/ai-integrations.md) | `IAIService`, HTTP and OpenAI providers |
| [Package map](docs/package-map.md) | Full list of packages by concern |
| [Testing](docs/testing.md) | `IDFCR.TestUtilities`, testing patterns |
| [Migration and adoption](docs/migration-and-adoption.md) | Incremental adoption paths |
| [Glossary](docs/glossary.md) | Key terms defined |

---

## Build and test

```bash
dotnet build IDFCR.slnx
dotnet test IDFCR.slnx
```

---

## Project status

Current version: **3.1.x** · Target framework: **.NET 10**

This repository is under active development. Public APIs within a major version are stable; breaking changes are introduced only on major version increments.
