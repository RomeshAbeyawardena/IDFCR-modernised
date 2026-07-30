# Migration and adoption

IDFCR is designed to be adopted incrementally. You do not need to rewrite an existing application. This page describes three practical adoption paths and guidance for coexisting with existing patterns.

---

## Adoption paths

### Path 1 — Results and handlers only

**Who should use this:** Any application that uses MediatR and wants consistent result contracts without touching persistence or infrastructure.

**What to install:**

```xml
<PackageReference Include="IDFCR.Abstractions.Results" />
<PackageReference Include="IDFCR.Abstractions.Mediator" />
<PackageReference Include="IDFCR.Abstractions.Mediator.Extensions" />
<!-- Optional: HTTP result mapping -->
<PackageReference Include="IDFCR.Results.Http" />
```

**Steps:**

1. Register the exception behaviour manager and MediatR:

   ```csharp
   services
       .ConfigureExceptionBehaviourManager(b => b.SetFluentValidationBehaviours())
       .AddMediatorServicesAndPipelines(configuration, assemblies: typeof(Program).Assembly);
   ```

2. Change existing `IRequest<T>` commands/queries to `IUnitResultRequest<T>`.
3. Change handler return types from `T` or `ActionResult<T>` to `IUnitResult<T>`.
4. Call `.AsHttp()` at API boundaries instead of returning raw values.

**Effort:** Low. Mainly a matter of changing interfaces and return types. Existing business logic is unchanged.

---

### Path 2 — Results, handlers, validation, and persistence

**Who should use this:** Applications that also use EF Core and want clean repository/filter patterns.

**Additional packages:**

```xml
<PackageReference Include="IDFCR.Abstractions.Persistence" />
<PackageReference Include="IDFCR.Abstractions.Filters" />
<PackageReference Include="IDFCR.Abstractions.Interceptors" />
<PackageReference Include="IDFCR.Abstractions.Interceptors.DependencyInjection" />
<PackageReference Include="IDFCR.Persistence.EntityFrameworkCore" />
<PackageReference Include="IDFCR.Persistence.EntityFrameworkCore.Extensions" />
```

**Steps (after path 1):**

1. Implement `IRepository<T, TKey>` on your repositories, extending `RepositoryBase<TCommon, TDb, T, TKey>` (four type parameters: shared abstraction, DB entity, domain model, key type).
2. Implement `IUnitOfWork` on or alongside your `DbContext`.
3. Enable the UoW post-processor:

   ```csharp
   services.AddMediatorServicesAndPipelines(
       configuration,
       configureOptions: o => o.UseUnitOfWorkPostPipeline(),
       assemblies: typeof(Program).Assembly);
   ```

4. Mark commands that should commit with `IUnitOfWorkRequest`:

   ```csharp
   public sealed record CreateOrderCommand(string Reference)
       : IUnitResultRequest<OrderDto>, IUnitOfWorkRequest
   {
       public bool CommitChanges => true;
   }
   ```

5. Register interceptors for audit timestamps:

   ```csharp
   services.AddInterceptors(typeof(Program).Assembly);
   ```

6. Write filters for your queries and register them via `ScanFilters`.

**Effort:** Medium. Requires changing repository interfaces and moving `SaveChangesAsync` calls from handler bodies to the framework post-processor.

---

### Path 3 — Full application flow

**Who should use this:** Applications that need reliable background delivery (outbox), distributed caching, or transport integrations (HTTP result mapping, gRPC).

**Additional packages (choose what you need):**

```xml
<!-- Outbox -->
<PackageReference Include="IDFCR.Abstractions.Outbox" />
<PackageReference Include="IDFCR.Abstractions.Outbox.Interceptors" />
<PackageReference Include="IDFCR.Outbox.EntityFramework" />
<PackageReference Include="IDFCR.Outbox.Extensions" />

<!-- Distributed caching -->
<PackageReference Include="IDFCR.Caching.Http" />
<PackageReference Include="IDFCR.Caching.Serialisation" />

<!-- gRPC -->
<PackageReference Include="IDFCR.Abstractions.GRPC.HostExtensions" />
<PackageReference Include="IDFCR.Abstractions.GRPC.Extensions" />

<!-- CLI / database updater -->
<PackageReference Include="IDFCR.DatabaseUpdater" />
```

**Steps (after path 2):**

1. Register outbox services and implement `IOutboxPublisher`, `IOutboxReader`.
2. Add a background service to poll and dispatch outbox records.
3. Add `AddGroupedDistributedCache` and implement cache invalidation on writes.
4. Register gRPC services with `app.DiscoverGRPCServices(configuration, assembly)` if needed.
5. Replace manual migration scripts with `ConfigureDatabaseUpdaterHost`.

---

## Coexisting with existing patterns

### Existing repositories

You do not need to replace all repositories at once. Wrap an existing repository with `IRepository<T, TKey>` incrementally — one entity at a time. Existing code using the old repository interface continues to work.

### Existing exception handling

`GenericDefaultExceptionPipeline` only intercepts exceptions that escape MediatR handlers. Exceptions in existing code outside MediatR are unaffected.

### Existing HTTP response conventions

If your API currently returns raw objects (`T`) instead of `IUnitResult<T>`, you can migrate endpoints one by one. Add `.AsHttp()` only to handlers that have been converted to return `IUnitResult<T>`.

---

## Common migration pitfalls

| Pitfall | Fix |
|---|---|
| Calling `SaveChangesAsync` inside the handler | Remove it. The `UnitOfWorkPostPipelineProcessor` handles commits. |
| Returning exceptions instead of `FailureReason.NotFound` | Return `UnitResult.NotFound<T>(id)` instead of throwing. |
| Registering filters but not the filter factory | Call `ScanFilters(assemblies)` — this also registers `DefaultFilterFactory`. |
| Registering `IExceptionBehaviourManager` after `AddMediatorServicesAndPipelines` | `ConfigureExceptionBehaviourManager` must come first. |
| Missing `IScopedResources` registration when using interceptors | `AddInterceptors` registers it. Do not register it manually unless you replace the default. |

---

## Further reading

- [Getting started](getting-started.md) — step-by-step for each adoption path.
- [Package map](package-map.md) — full package list to plan your dependency graph.
- [Architecture overview](architecture-overview.md) — how the pieces connect.
