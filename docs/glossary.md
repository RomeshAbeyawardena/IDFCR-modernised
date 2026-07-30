# Glossary

Definitions for terms used throughout IDFCR documentation, in alphabetical order.

---

**Action** (`UnitAction`)  
A flags enum describing what kind of operation produced a result: `Add`, `Get`, `Update`, `Delete`, `Pending`, or `Conflict`. Used by HTTP bridges to choose the correct status code (e.g., `Add` → 201 Created, `Delete` → 204 No Content).

---

**Assembly scanning**  
A registration pattern where a single extension method call discovers and registers all types implementing a given interface from one or more assemblies. IDFCR uses it for interceptors (`AddInterceptors`), filters (`ScanFilters`), CLI commands (`AddInjectableCommandServices`), and MediatR handlers (`AddMediatorServicesAndPipelines`).

---

**Audit processor** (`IAuditProcessor`)  
A component that receives before/after snapshots of an entity's properties when the entity changes, allowing property-level change history to be recorded. Discovered and registered by `AddInterceptors`.

---

**Chained result** (`IChainedUnitResult`)  
A result type that groups multiple related sub-results into a single traceable response. The chain is considered successful only when all recorded sub-results are successful.

---

**Command**  
In the context of IDFCR, a command is a MediatR request that expresses the *intent to change state*. Commands implement `IUnitResultRequest<T>` (typed) or `IUnitResultRequest` (untyped). They are handled by `IUnitResultRequestHandler<TReq, TResp>`.

---

**Composite key** (caching)  
The secondary key within a cache group that identifies a specific entry. Typically constructed from the dimensions that determine the result: tenant ID, filter values, page index, and so on. Example: `"tenant-42:active:page-1"`.

---

**Delta** (`IStringListDelta`, `StringListDelta`)  
A value object describing a change to a collection of strings: which items to `Add` and which to `Remove`. Used with `PerformDeltaAsync` to sync many-to-many relationships atomically.

---

**Dispatcher** (`IOutboxDispatcher`)  
The component that orchestrates reading pending outbox records and passing them to a publisher. Reads pages of messages from `IOutboxReader` and calls `IOutboxPublisher.HandleAsync`.

---

**Exception behaviour manager** (`IExceptionBehaviourManager`)  
Configures how the `GenericDefaultExceptionPipeline` maps specific exception types to `UnitAction` and `FailureReason` values. Registered via `ConfigureExceptionBehaviourManager`.

---

**Failure origin** (`FailureOrigin`)  
Describes where a failure occurred: `Internal` (within the application), `Caller` (caused by invalid input), or `Unknown`. Used to decide whether exception detail should be exposed to callers.

---

**Failure reason** (`FailureReason`)  
An enum describing why an operation failed: `NotFound`, `ValidationError`, `Conflict`, `Unauthorized`, `Forbidden`, `InternalError`, `ExternalDependencyError`, `AuthorizationError`, `NotSupported`, `Unknown`. HTTP bridges map failure reasons to status codes.

---

**Filter** (`IFilter<TRequest, TDb>`, `FilterBase<TRequest, TDb>`)  
A class that applies a composable predicate to an `IQueryable<TDb>` based on a request. Filters are discovered by assembly scanning and applied in sequence by `IFilterFactory.Apply`.

---

**Filter factory** (`IFilterFactory`)  
Resolves applicable filters for a given request type and applies them to a queryable. The `DefaultFilterFactory` implementation reads all registered `IFilter<TDb>` services from DI.

---

**Group key** (caching)  
The primary key that identifies a cache group. All entries in a group share the same group key and can be invalidated together with a single `RemoveAsync` call. Example: `"orders"`.

---

**Handler**  
A class that processes a specific MediatR request and returns a result. In IDFCR, handlers implement `IUnitResultRequestHandler<TRequest, TResponse>` (or one of the collection/paged variants).

---

**Intent**  
The goal expressed by a command or query: what the caller wants to happen, expressed independently of how the infrastructure achieves it.

---

**Interceptor** (`IEntityInterceptor`)  
A class that runs when an entity is inserted, updated, or deleted. Interceptors handle cross-cutting concerns (audit timestamps, outbox staging, soft deletion) without modifying repositories or handlers. Registered and discovered via `AddInterceptors`.

---

**Outbox**  
A durability pattern in which messages to be delivered to external systems are persisted in the same database transaction as the business data that triggers them. A background worker reads pending messages and delivers them, providing at-least-once delivery semantics.

---

**Outbox entity** (`IOutboxEntity`)  
A persistent record representing a message in the outbox. Tracks delivery status via `CompletedTimestampUtc`, `FailedTimestampUtc`, and `ProcessedTimestampUtc`.

---

**Outbox publisher** (`IOutboxPublisher`)  
The component that receives a batch of outbox records and delivers them to the external system (e.g., a message bus, a webhook, another API).

---

**Outbox reader** (`IOutboxReader`)  
The component that pages pending outbox records from the persistence store for the dispatcher to process.

---

**Pipeline**  
In MediatR terms, a set of `IPipelineBehavior` and `IRequestExceptionHandler` implementations that run around a handler. IDFCR's built-in pipelines are the `ValidationPipeline`, `GenericDefaultExceptionPipeline`, and `UnitOfWorkPostPipelineProcessor`.

---

**Publisher** — see **Outbox publisher**.

---

**Query**  
A MediatR request that expresses the *intent to read state*. Queries implement `IUnitResultRequest<T>`, `IUnitResultCollectionRequest<T>`, or `IPagedUnitResultRequest<T>`.

---

**Repository** (`IRepository<T, TKey>`)  
A data-access abstraction that provides `FindAsync`, `UpsertAsync`, `DeleteAsync`, and `GetPagedAsync`. Every method returns an `IUnitResult*` type.

---

**Result** (`IUnitResult`, `IUnitResult<T>`)  
The value returned by any operation. Carries a success flag, optional result value, `FailureReason`, `UnitAction`, and optional exception. Results travel across handler, HTTP, and outbox boundaries without translation ceremony.

---

**Scoped resources** (`IScopedResources`)  
A type-keyed bag registered as a scoped DI service. Lets interceptors and post-processors share already-resolved, execution-scoped objects within a single pipeline run without using a service locator.

---

**Transport bridge**  
A component that translates an `IUnitResult*` into a transport-specific response. IDFCR provides bridges for HTTP (`.AsHttp()`) and gRPC (`UnitResultExtensions.ToGrpcStatus()`).

---

**Unit of work** (`IUnitOfWork`)  
Coordinates writing a set of changes to the data store as a single operation. `SaveChangesAsync` is called by the `UnitOfWorkPostPipelineProcessor` after a successful handler response; do not call it from inside a handler.

---

**UnitAction** — see **Action**.
