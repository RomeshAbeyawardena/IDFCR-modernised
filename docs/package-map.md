# Package map

This page lists every package in the IDFCR repository, grouped by concern. Install only the packages you need.

> All packages target **.NET 10** and share version **3.1.x** (see `Directory.Build.props`).

---

## Results and handler contracts

These packages form the minimum viable dependency for any IDFCR-aware application.

| Package | Purpose |
|---|---|
| `IDFCR.Abstractions.Results` | `IUnitResult`, `IUnitResult<T>`, `IUnitResultCollection<T>`, `IPagedUnitResult<T>`, `IChainedUnitResult`, `UnitResult` factory, `FailureReason`, `UnitAction`, `FailureOrigin` |
| `IDFCR.Abstractions.Mediator` | `IUnitResultRequest<T>`, `IUnitResultCollectionRequest<T>`, `IPagedUnitResultRequest<T>`, `PagedUnitResultRequestBase<T>` |
| `IDFCR.Abstractions.Mediator.Extensions` | `IUnitResultRequestHandler<TReq, TResp>`, `ValidationPipeline`, `GenericDefaultExceptionPipeline`, `UnitOfWorkPostPipelineProcessor`, `AbstractPagedQueryValidator<T>`, `AddMediatorServicesAndPipelines`, `ConfigureExceptionBehaviourManager` |
| `IDFCR.Results.Http` | `.AsHttp()`, `.AsChainedHttp()`, `IUnitHttpResult`, HTTP status code mapping |
| `IDFCR.Results.Http.Grpc` | gRPC-transcoded HTTP result support |

---

## Metadata and common contracts

| Package | Purpose |
|---|---|
| `IDFCR.Abstractions.Metadata` | `IIdentifiable<TKey>`, `IAuditCreatedTimestamp`, `IAuditModifiedTimestamp`, `IAuditable`, `INamed`, `IStringListDelta`, `StringListDelta`, `NamedDeltaOptions`, `RelationshipDeltaResult`, `IOrderedRequest`, `IStructuredOrderedRequest`, `EntityState` |
| `IDFCR.Abstractions.Builders` | `IDictionaryBuilder<T>`, `DictionaryBuilder` |
| `IDFCR.Abstractions.Mapper` | `IMapper<TSource>`, `IMapper<TSource, TSource1>`, `MapperBase`, `IRecordMapper` |
| `IDFCR.Abstractions.Mapper.Extensions` | Mapper scanning and registration extensions |

---

## Dependency injection

| Package | Purpose |
|---|---|
| `IDFCR.Abstractions.DependencyInjection` | `IScopedResources`, `DefaultScopedResources`, `ScanGenericServices` extension |

---

## Persistence

| Package | Purpose |
|---|---|
| `IDFCR.Abstractions.Persistence` | `IRepository<T, TKey>`, `IUnitOfWork`, `ITransactionalUnitOfWork`, `IDbTransaction`, `RepositoryBase<T, TKey>`, `RepositoryInterceptorContext`, `IHasRowVersion`, `DatabaseConfiguration`, `MaximumLengthStringExpressionBuilder` |
| `IDFCR.Abstractions.Persistence.Interceptors` | `SoftDeletionEntityInterceptor` (marks entities as deleted rather than removing rows) |
| `IDFCR.Abstractions.Persistence.StorageQueues` | `IQueueProducer`, `IQueueConsumer`, `IQueueMessageItem`, `IQueuePullResponse` (storage queue abstractions) |
| `IDFCR.Persistence.EntityFrameworkCore` | EF Core implementation of `IRepository<T, TKey>` and `IUnitOfWork` |
| `IDFCR.Persistence.EntityFrameworkCore.Extensions` | `DeltaExtensions.PerformDeltaAsync` and EF Core-specific helpers |
| `IDFCR.Persistence.CloudFlare` | Cloudflare D1 / R2 persistence helpers |

---

## Filters

| Package | Purpose |
|---|---|
| `IDFCR.Abstractions.Filters` | `FilterBase<TRequest, TDb>`, `IFilterFactory`, `DefaultFilterFactory`, `DefaultPagedFilter`, `GlobalFilterAttribute`, `AddFilterFactory` |

---

## Interceptors

| Package | Purpose |
|---|---|
| `IDFCR.Abstractions.Interceptors` | `IEntityInterceptor`, `EntityInterceptorBase`, `IEntityInterceptorContext`, `IEntityInterceptorFactory`, `DefaultEntityInterceptorFactory`, `EntityContextBehavior`, `EntityContextBehaviorStage`, `AuditCreatedTimestampEntityInterceptor`, `AuditModifiedTimestampEntityInterceptor`, `AuditEntityChangesInterceptor`, `IAuditProcessor`, `AuditProcessorBase`, `IScopedResources` (re-exported) |
| `IDFCR.Abstractions.Interceptors.DependencyInjection` | `AddInterceptors(assemblies)` |
| `IDFCR.Abstractions.Interceptors.Extensions` | `AuditProcessorExtensions` for processor helpers |

---

## Outbox

| Package | Purpose |
|---|---|
| `IDFCR.Abstractions.Outbox` | `IOutboxEntity`, `IOutboxEntity<TKey>`, `DefaultOutboxEntity`, `IOutboxPublisher`, `IOutboxDispatcher`, `IOutboxReader`, `IOutboxReaderFactory`, `IOutboxPipeline`, `IOutboxEntityNotificationHandler`, `OutboxEntityNotificationHandlerBase` |
| `IDFCR.Abstractions.Outbox.Extensions` | Service registration helpers for outbox components |
| `IDFCR.Abstractions.Outbox.Interceptors` | `OutboxInterceptor` — stages messages in `IScopedResources` during entity saves |
| `IDFCR.Outbox.EntityFramework` | EF Core-backed `RepositoryOutboxReaderBase` and `EntityFrameworkOutboxEntityNotificationHandlerBase` |
| `IDFCR.Outbox.Extensions` | `OutboxPublisherBase<TMessage>`, `OutboxReaderBase<TMessage, TPagedQuery>`, `DefaultOutboxReaderFactory` |

---

## Caching

| Package | Purpose |
|---|---|
| `IDFCR.Abstractions.Caching` | `ICacheGroup`, `ICacheGroups`, `IDistributedCacheGroups`, `CachedStringService` |
| `IDFCR.Caching` | `DefaultCacheGroup`, `DefaultCacheGroups`, `DefaultDistributedCacheGroups` |
| `IDFCR.Caching.Http` | `IDistributedGroupCache`, `DefaultDistributedGroupCache`, `DefaultDistributedGroupCacheWithAuditing`, `IDistributedGroupCacheAuditSink`, `LoggerDistributedGroupCacheAuditSink`, `AddGroupedDistributedCache`, `AddGroupedDistributedCacheWithLogAuditing` |
| `IDFCR.Caching.Serialisation` | `DeserialiseAsync<T>` (MessagePack extension on `byte[]`) |

---

## gRPC

| Package | Purpose |
|---|---|
| `IDFCR.Abstractions.GRPC` | `RegisteredGRPCServiceImplementationAttribute`, `IRegisteredGRPCServiceImplementationTypeDiscoveryService`, `RegisteredGRPCServiceImplementationTypeDiscoveryService` |
| `IDFCR.Abstractions.GRPC.Contracts` | Shared `.proto` definitions for `UnitResult`, `UnitAction`, `FailureReason`, `StringListDelta` |
| `IDFCR.Abstractions.GRPC.Generated` | Pre-generated C# from the shared proto contracts |
| `IDFCR.Abstractions.GRPC.Extensions` | `UnitResultExtensions` (result → gRPC status mapping), `SortOrderExtensions`, `StringListDelta` proto conversion |
| `IDFCR.Abstractions.GRPC.HostExtensions` | `AddGrpcServices(assemblies)` — assembly-scanned gRPC service registration |
| `IDFCR.GRPC.Client.Extensions` | gRPC client factory helpers with service discovery |

---

## CLI

| Package | Purpose |
|---|---|
| `IDFCR.Abstractions.Cli` | `ICommandOperation`, `IInjectableCommandOperation`, `InjectableCommandOperationBase`, `ICommandRouteDispatcher`, `DefaultCommandRouteDispatcher`, `IArgumentParameters`, `ArgumentParameters`, `IManagedStream`, `IIOReadableStream`, `IIOWriteableStream`, `ConsoleStream`, `IPromptGreeter`, `FeatureCommandAttribute`, `ReturnResult` |
| `IDFCR.Abstractions.Cli.Extensions` | `AddInjectableCommandServices(assemblies)`, `RunCommandsAsync`, `ScrutorExtensions` |

---

## Database updater

| Package | Purpose |
|---|---|
| `IDFCR.Abstractions.DatabaseUpdater` | `IDatabaseFascade`, `DefaultDatabaseFascade`, `ITargetDatabaseConfiguration`, `DatabaseRootCommand`, `ApplyDatabaseMigrationsCommand`, `ListDatabaseMigrationsCommand` |
| `IDFCR.DatabaseUpdater` | `ConfigureDatabaseUpdaterHost`, `ConfigureDatabaseUpdater`, `TargetDatabaseConfiguration`, `IConfiguredDatabaseUpdaterHost` |

---

## AI

| Package | Purpose |
|---|---|
| `IDFCR.AI.Abstractions` | `IAIService`, `IAIServiceConfiguration`, `ITextGeneration`, `AIServiceRequest`, `AIServiceResponse`, `VerifiedConnectionResult` |
| `IDFCR.AI.Http` | `HttpAIService`, `HttpAIServiceConfiguration`, `AddHttpAIService` |
| `IDFCR.AI.OpenAI` | `OpenAIService`, `IOpenAIService`, `OpenAIConfiguration`, `AddOpenAIService` |

---

## Cryptography

| Package | Purpose |
|---|---|
| `IDFCR.Abstractions.Cryptography` | `IPasswordDerivedKeyGenerator`, `ITokenPayloadProtector` |
| `IDFCR.Cryptography` | Default implementations of the cryptography abstractions |

---

## Utilities

| Package | Purpose |
|---|---|
| `IDFCR.Utilities` | Shared utility extensions used internally (logging helpers, etc.) |

---

## Testing

| Package | Purpose |
|---|---|
| `IDFCR.TestUtilities` | Reusable in-memory test infrastructure, stream test doubles, shared test helpers |

---

## Typical package combinations

### Results-only application

```
IDFCR.Abstractions.Results
```

### MediatR handler application

```
IDFCR.Abstractions.Results
IDFCR.Abstractions.Mediator
IDFCR.Abstractions.Mediator.Extensions
```

### Web API with HTTP result mapping

Add:
```
IDFCR.Results.Http
```

### EF Core persistence

Add:
```
IDFCR.Abstractions.Persistence
IDFCR.Abstractions.Filters
IDFCR.Abstractions.Interceptors
IDFCR.Abstractions.Interceptors.DependencyInjection
IDFCR.Persistence.EntityFrameworkCore
IDFCR.Persistence.EntityFrameworkCore.Extensions
```

### Grouped distributed caching

Add:
```
IDFCR.Caching.Http
IDFCR.Caching.Serialisation
```

### Outbox pattern

Add:
```
IDFCR.Abstractions.Outbox
IDFCR.Abstractions.Outbox.Extensions
IDFCR.Abstractions.Outbox.Interceptors
IDFCR.Outbox.EntityFramework
IDFCR.Outbox.Extensions
```

### gRPC hosting

Add:
```
IDFCR.Abstractions.GRPC
IDFCR.Abstractions.GRPC.Extensions
IDFCR.Abstractions.GRPC.HostExtensions
```

### AI integration

Add:
```
IDFCR.AI.Abstractions
IDFCR.AI.Http           (generic HTTP provider)
IDFCR.AI.OpenAI         (OpenAI-specific provider)
```

---

## Further reading

- [Getting started](getting-started.md) — how to combine these packages in a real application.
- [Migration and adoption](migration-and-adoption.md) — adding packages incrementally.
