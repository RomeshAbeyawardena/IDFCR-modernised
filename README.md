# IDFCR: Intent-Driven Flow Composition Runtime

IDFCR is a modular .NET application framework/toolkit for **result-driven, handler-oriented, infrastructure-light** application development.

It helps teams (and coding agents) build predictable application flows by standardising result contracts, MediatR handler conventions, and infrastructure registration patterns—without forcing you to adopt every package.

> Install only what you need. IDFCR is intentionally opt-in.

---

## What is IDFCR?

IDFCR (Intent-Driven Flow Composition Runtime) is a set of composable packages for building clean .NET applications around explicit intent and explicit outcomes.

Core value:
- **Consistent results** (`IUnitResult*`) across handlers, HTTP, and infrastructure boundaries.
- **Handler conventions** for commands/queries using MediatR-compatible abstractions.
- **Clean registration** through extension methods that hide repetitive plumbing.
- **Optional capabilities** (persistence, caching, AI, gRPC, CLI, database updater) you can add only when needed.

This is not “magic”. It is architecture-guided acceleration: boring correctness under wraps, interesting business logic on top.

---

## The IDFCR philosophy

- **Make the common thing easy**: standard result contracts and registration extensions remove boilerplate.
- **Make the complex thing possible**: compose interceptors, persistence, caching, gRPC, and AI without rewriting foundations.
- **Keep consumer applications clean**: app code should mostly express business intent, not infrastructure ceremony.
- **Prefer explicit opt-in**: use packages by concern; do not pull the whole solution by default.
- **Prefer result-driven flow**: expected failures should travel as result states, not exception/control-flow spaghetti.

---

## Package map / choose what you need

Use package groups as building blocks.

### Results + flow contracts
- **[Abstractions]** `IDFCR.Abstractions.Results`, `IDFCR.Abstractions.Mediator`
- **[Implementation bridge]** `IDFCR.Abstractions.Mediator.Extensions`
- **[HTTP bridge]** `IDFCR.Results.Http`

Use when you want consistent operation outcomes and MediatR request/handler conventions.

### Persistence
- **[Abstractions]** `IDFCR.Abstractions.Persistence`, `IDFCR.Abstractions.Filters`, `IDFCR.Abstractions.Interceptors`
- **[Implementations]** `IDFCR.Persistence.EntityFrameworkCore`, `IDFCR.Persistence.EntityFrameworkCore.Extensions`
- **[Tooling]** `IDFCR.DatabaseUpdater`, `IDFCR.Abstractions.DatabaseUpdater`

Use when you want repository/unit-of-work patterns with interceptor/filter support and migration automation.

### Caching
- **[Abstractions]** `IDFCR.Abstractions.Caching`
- **[Implementations]** `IDFCR.Caching`, `IDFCR.Caching.Http`, `IDFCR.Caching.Serialisation`

Use when you need grouped distributed cache invalidation and MessagePack-backed cache payload handling.

### AI
- **[Abstractions]** `IDFCR.AI.Abstractions`
- **[Implementations]** `IDFCR.AI.Http`, `IDFCR.AI.OpenAI`

Use when you want provider-backed AI integration behind stable contracts.

### gRPC
- **[Abstractions/contracts]** `IDFCR.Abstractions.gRPC`, `IDFCR.Abstractions.gRPC.Contracts`
- **[Generated/bridge/host]** `IDFCR.Abstractions.gRPC.Generated`, `IDFCR.Abstractions.gRPC.Extensions`, `IDFCR.Abstractions.gRPC.HostExtensions`

Use when you want shared result contracts plus assembly-driven gRPC service hosting.

### CLI + build/runtime tooling
- `IDFCR.Abstractions.Cli`, `IDFCR.Abstractions.Cli.Extensions`
- `BuildTools.*` projects for packaged CLI + backend orchestration workflows.

### Test support
- `IDFCR.TestUtilities`

Use for reusable in-memory test helpers and shared test infrastructure primitives.

### Common package selection examples

- **Results only**  
  `IDFCR.Abstractions.Results`

- **MediatR handler applications**  
  `IDFCR.Abstractions.Results` + `IDFCR.Abstractions.Mediator` + `IDFCR.Abstractions.Mediator.Extensions`

- **ASP.NET Core HTTP results**  
  add `IDFCR.Results.Http`

- **EF Core persistence**  
  add `IDFCR.Abstractions.Persistence` + `IDFCR.Persistence.EntityFrameworkCore`

- **Grouped distributed caching**  
  add `IDFCR.Caching.Http` + `IDFCR.Caching.Serialisation`

- **AI integrations**  
  add `IDFCR.AI.Abstractions` + `IDFCR.AI.Http` (+ `IDFCR.AI.OpenAI` when using OpenAI)

- **gRPC hosting**  
  add `IDFCR.Abstractions.gRPC.HostExtensions` + required gRPC contracts/bridges

- **CLI tooling**  
  add `IDFCR.Abstractions.Cli` (+ `.Cli.Extensions` for host wiring)

---

## Golden path: minimal application setup

Use extension methods to keep `Program.cs` thin and intention-revealing.

```csharp
using IDFCR.Caching.Http.Extensions;
using MessagePack;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services
    .AddSingleton(MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray))
    .AddGroupedDistributedCacheWithLogAuditing()
    .AddHandlers(typeof(Program).Assembly)
    .AddDatabaseInfrastructure(builder.Configuration)
    .AddRazorPages();
```

`AddHandlers(...)` and `AddDatabaseInfrastructure(...)` are consumer-facing composition extensions (defined in your application layer) that encapsulate infrastructure plumbing; cache extensions from IDFCR do the same.

---

## Golden path: handlers and results

Write handlers around `IUnitResult*` outcomes.

```csharp
using IDFCR.Abstractions.Mediator;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Results;

public sealed record CreateClientCommand(string Name) : IUnitResultRequest<ClientDto>;
public sealed record GetClientsQuery(IEnumerable<string> Names) : IUnitResultCollectionRequest<ClientDto>;
public sealed record GetPagedClientsQuery : PagedUnitResultRequestBase<ClientDto>;

public sealed class CreateClientCommandHandler(IClientRepository clients)
    : IUnitResultRequestHandler<CreateClientCommand, ClientDto>
{
    public async Task<IUnitResult<ClientDto>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return UnitResult.Failed<ClientDto>(new ArgumentException("Name is required"), failureReason: FailureReason.ValidationError);
        }

        // call your repository/service here
        var created = new ClientDto(Guid.NewGuid(), request.Name);
        return UnitResult.FromResult(created, UnitAction.Add);
    }
}
```

When to use which result type:
- **`IUnitResult<T>`**: single item/value outcome for a command or query.
- **`IUnitResultCollection<T>`**: non-paged list/collection outcomes.
- **`IUnitPagedResult<T>`**: paged list outcomes including paging metadata.
- **`ChainedResult`**: combine related operation results into one traceable chain.

Expected domain/application failures should be represented as result states (`FailureReason`, success flag, optional exception payload), not thrown as raw control-flow exceptions.
Use `GenericDefaultExceptionPipeline` as the fallback exception pipeline so unexpected exceptions are translated into clean `IUnitResult*` responses.

---

## Golden path: grouped distributed caching

Grouped distributed caching uses:
- **group key** (e.g., `clients`)
- **composite key** (e.g., `tenant-42:active`)
- **payload bytes** (usually MessagePack)

You usually invalidate by **group** when source data changes, instead of trying to track every derived cache key manually.

```csharp
using IDFCR.Caching.Http;
using IDFCR.Caching.Serialisation.Extensions;
using MessagePack;

public sealed class ClientCacheService(
    IDistributedGroupCache groupCache,
    MessagePackSerializerOptions serializerOptions)
{
    public async Task<IReadOnlyList<ClientDto>> GetAsync(string tenantId, CancellationToken cancellationToken)
    {
        const string groupKey = "clients";
        var compositeKey = $"{tenantId}:active";

        var cachedValue = await groupCache.GetAsync(groupKey, compositeKey, cancellationToken);
        if (cachedValue is not null)
        {
            var results = await cachedValue.DeserialiseAsync<List<ClientDto>>(serializerOptions, cancellationToken);
            return results;
        }

        // ...query source, then cache with SetAsync(groupKey, compositeKey, bytes, cancellationToken)
        return [];
    }

    public Task InvalidateClientsAsync(CancellationToken cancellationToken)
        => groupCache.RemoveAsync("clients", cancellationToken);
}
```

---

## Anti-patterns to avoid

- Installing every IDFCR package “just in case”.
- Throwing exceptions for expected validation/not-found paths instead of returning failed `IUnitResult` states.
- Returning ad-hoc response shapes per handler instead of using shared result contracts.
- Hand-writing infrastructure registration in `Program.cs` when existing extension methods already provide it.
- Invalidating individual derived cache keys when a group-level invalidation is the safer intent.

---

## Build and test

```bash
dotnet build IDFCR.slnx
dotnet test IDFCR.slnx --no-build
```
