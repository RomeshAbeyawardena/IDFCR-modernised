# IDFCR Developer Documentation

This is the full documentation index for IDFCR — Intent-Driven Flow Composition Runtime.

Return to the [repository root](../README.md) for a quick introduction and quick-start example.

---

## Learning path

### Start here

| Page | What you will learn |
|---|---|
| [Getting started](getting-started.md) | How to install IDFCR, wire up MediatR, write your first handler, and return a result from an HTTP endpoint |
| [Architecture overview](architecture-overview.md) | How the package layers fit together and how a request flows through the framework |

### Core concepts

| Page | What you will learn |
|---|---|
| [Results and flow](results-and-flow.md) | The `IUnitResult` family, `FailureReason`, `UnitAction`, collection and paged results, chained results |
| [Handlers and pipelines](handlers-and-pipelines.md) | MediatR request interfaces, handler interfaces, the exception pipeline, and the UoW post-processor |
| [Validation](validation.md) | FluentValidation pipeline integration and the `AbstractPagedQueryValidator` base class |

### Data and persistence

| Page | What you will learn |
|---|---|
| [Persistence and unit of work](persistence-and-unit-of-work.md) | `IRepository<T, TKey>`, `IUnitOfWork`, EF Core implementation, and `IScopedResources` |
| [Filters and paging](filters-and-paging.md) | `FilterBase`, `IFilterFactory`, `DefaultPagedFilter`, and how assembly scanning wires filters |
| [Interceptors](interceptors.md) | `IEntityInterceptor`, built-in audit interceptors, `OutboxInterceptor`, and `AddInterceptors` registration |
| [Delta operations](delta-operations.md) | `StringListDelta`, `PerformDeltaAsync`, and how to sync many-to-many relationships atomically |

### Background processing

| Page | What you will learn |
|---|---|
| [Outbox and dispatch](outbox-and-dispatch.md) | `IOutboxEntity`, `IOutboxPublisher`, `IOutboxReader`, `IOutboxDispatcher`, and delivery semantics |

### Integration and transport

| Page | What you will learn |
|---|---|
| [Caching](caching.md) | `IDistributedGroupCache`, group-keyed invalidation, MessagePack serialisation, and auditing |
| [HTTP and gRPC](http-and-grpc.md) | Mapping results to HTTP via `.AsHttp()`, gRPC result extensions, and assembly-scanned service hosting |
| [CLI and database updater](cli-and-database-updater.md) | `ICommandOperation`, interactive CLI hosting, and `ConfigureDatabaseUpdaterHost` for EF Core migrations |
| [AI integrations](ai-integrations.md) | `IAIService`, `IOpenAIService`, HTTP and OpenAI providers |

### Testing and operations

| Page | What you will learn |
|---|---|
| [Testing](testing.md) | `IDFCR.TestUtilities`, testing handlers, results, repositories, and interceptors |
| [Migration and adoption](migration-and-adoption.md) | Incremental adoption paths and guidance for adding IDFCR to an existing codebase |

### Reference

| Page | What you will learn |
|---|---|
| [Package map](package-map.md) | Every published package, its purpose, and its dependencies |
| [Glossary](glossary.md) | Framework-specific and architectural terms defined in plain language |
