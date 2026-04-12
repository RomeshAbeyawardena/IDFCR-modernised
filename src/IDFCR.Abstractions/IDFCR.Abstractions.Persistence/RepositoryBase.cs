using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Persistence.Extensions;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Exceptions;

namespace IDFCR.Abstractions.Persistence
{
    /// <summary>
    /// Base class for repositories that map between a common abstraction, a database model, and a domain model.
    /// </summary>
    /// <typeparam name="TCommon">The shared abstraction implemented by both models.</typeparam>
    /// <typeparam name="TDb">The persistence model type.</typeparam>
    /// <typeparam name="T">The domain model type.</typeparam>
    /// <typeparam name="TKey">The identifier type.</typeparam>
    /// <param name="entityInterceptorFactory">The interceptor factory used to resolve repository interceptors.</param>
    public abstract class RepositoryBase<TCommon, TDb, T, TKey>(IEntityInterceptorFactory entityInterceptorFactory) : IRepository<T, TKey>
        where TKey : struct
        where TDb: class, IMapper<TCommon>, TCommon, IIdentifiable<TKey>
        where T: class, IMapper<TCommon>, TCommon
    {
        private async Task<IUnitResult<T>> WrapFindResult(Func<CancellationToken, Task<TDb?>> onFindAsync, object key, CancellationToken cancellationToken)
        {
            Exception? caughtException;
            bool success = false;
            T? value = null;
            try
            {
                var result = await onFindAsync(cancellationToken);

                if (result is not null)
                {
                    value = Map(result);
                    success = value is not null;
                    caughtException = value is null ? new EntityNotFoundException(typeof(T), key) : null;
                }
                else
                {
                    caughtException = new EntityNotFoundException(typeof(T), key);
                }
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            return UnitResult.FromResult(value, UnitAction.Get, success, caughtException);
        }
        
        private async Task<RepositoryInterceptorContext> InvokeInterceptorsAsync(EntityContextBehaviorStage stage,
            EntityContextBehavior behavior,
            object model, CancellationToken cancellationToken)
        {
            var context = RepositoryInterceptorContext.Create(stage, behavior, model);
            var interceptors = await entityInterceptorFactory.GetEntityInterceptorsAsync(context, cancellationToken);
            await entityInterceptorFactory.InvokeAsync(interceptors, context, cancellationToken);

            return context;
        }

        /// <summary>
        /// Maps the domain model to the database model.
        /// </summary>
        protected virtual TDb? Map(T value)
        {
            return value.Map<TDb>();
        }

        /// <summary>
        /// Maps the database model to the domain model.
        /// </summary>
        protected virtual T? Map(TDb value)
        {
            return value.Map<T>();
        }

        /// <summary>
        /// Persists a new record.
        /// </summary>
        protected abstract Task<TKey> OnAddAsync(TDb entry, T rawEntry, CancellationToken cancellationToken);
        /// <summary>
        /// Persists an updated record.
        /// </summary>
        protected abstract Task<TKey> OnUpdateAsync(TDb entry, T rawEntry, CancellationToken cancellationToken);
        /// <summary>
        /// Finds a record by primary key.
        /// </summary>
        protected abstract Task<TDb?> OnFindAsync(TKey key, bool trackChanges, CancellationToken cancellationToken);
        /// <summary>
        /// Finds a record by composite keys.
        /// </summary>
        protected abstract Task<TDb?> OnFindAsync(object[] keys, bool trackChanges, CancellationToken cancellationToken);
        /// <summary>
        /// Deletes a record by its primary key.
        /// </summary>
        protected abstract Task<bool> OnDeleteAsync(TKey key, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the data and total row count for a paged query.
        /// </summary>
        protected abstract Task<(IEnumerable<TDb> data,int totalRows)> OnGetPagedAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IPagedQuery;

        /// <summary>
        /// Determines whether an exception is handled by the repository.
        /// </summary>
        protected abstract bool IsHandled(Exception exception);

        /// <inheritdoc />
        public async Task<IUnitResult> DeleteAsync(TKey key, CancellationToken cancellationToken)
        {
            Exception? caughtException = null;
            bool success = false;
            try
            {
                var dbValue = await OnFindAsync(key, true, cancellationToken);

                if (dbValue is null)
                {
                    return UnitResult.NotFound<TKey>(key, caughtException);
                }

                var context = await InvokeInterceptorsAsync(EntityContextBehaviorStage.Pre,
                        EntityContextBehavior.Delete, dbValue, cancellationToken);

                if (context.BypassOperation)
                {
                    return UnitResult.FromResult(key, UnitAction.None)
                            .AddMeta("bypassed", true).As<TKey>();
                }

                success = await OnDeleteAsync(key, cancellationToken);

                await InvokeInterceptorsAsync(EntityContextBehaviorStage.Post,
                        EntityContextBehavior.Delete, dbValue, cancellationToken);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            return UnitResult.FromResult(key, UnitAction.Delete, success, caughtException);
        }

        /// <inheritdoc />
        public Task<IUnitResult<T>> FindAsync(object[] keys, CancellationToken cancellationToken)
        {
            return WrapFindResult(ct => OnFindAsync(keys, false, ct), keys, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IUnitResult<T>> FindAsync(TKey key, CancellationToken cancellationToken)
        {
            return WrapFindResult(ct => OnFindAsync(key, false, ct), key, cancellationToken);
        }

        /// <summary>
        /// On add hook for performing actions before or after the add operation. The raw entry is provided for reference but should not be modified.
        /// </summary>
        /// <param name="db">The database entity.</param>
        /// <param name="dto">The data transfer object.</param>
        protected abstract void OnUpdate(TDb db, T dto);

        /// <inheritdoc />
        public async Task<IUnitResult<TKey>> UpsertAsync(T entry, CancellationToken cancellationToken)
        {
            try
            {
                var dbValue = Map(entry) ?? throw new InvalidOperationException($"Mapping from {typeof(T)} to {typeof(TDb)} failed");

                if (EqualityComparer<TKey>.Default.Equals(dbValue.Id, default))
                {
                    var context = await InvokeInterceptorsAsync(EntityContextBehaviorStage.Pre, 
                        EntityContextBehavior.Insert, dbValue, cancellationToken);

                    if (context.BypassOperation)
                    {
                        return UnitResult.FromResult(default(TKey), UnitAction.None)
                            .AddMeta("bypassed", true).As<TKey>();
                    }

                    var id = await OnAddAsync(dbValue, entry, cancellationToken);

                    await InvokeInterceptorsAsync(EntityContextBehaviorStage.Post,
                        EntityContextBehavior.Insert, dbValue, cancellationToken);

                    return UnitResult.FromResult(id, UnitAction.Add);
                }
                else
                {
                    var foundEntry = await OnFindAsync(dbValue.Id, true, cancellationToken);

                    if (foundEntry is null)
                    {
                        return UnitResult.NotFound<TKey>(dbValue.Id, new EntityNotFoundException(typeof(T), dbValue.Id));
                    }

                    OnUpdate(foundEntry, entry);

                    foundEntry.Apply(dbValue);
                    var context = await InvokeInterceptorsAsync(EntityContextBehaviorStage.Pre,
                        EntityContextBehavior.Update, foundEntry, cancellationToken);

                    if (context.BypassOperation)
                    {
                        return UnitResult.FromResult(dbValue.Id, UnitAction.None)
                            .AddMeta("bypassed", true).As<TKey>();
                    }
                    var id = await OnUpdateAsync(foundEntry, entry, cancellationToken);
                    await InvokeInterceptorsAsync(EntityContextBehaviorStage.Post,
                        EntityContextBehavior.Update, foundEntry, cancellationToken);

                    return UnitResult.FromResult(id, UnitAction.Update);
                }
            }
            catch (Exception exception)
            {
                if (IsHandled(exception))
                {
                    return UnitResult.Failed<TKey>(exception);
                }
                return UnitResult.Failed<TKey>(exception).AddMeta("unexpected", "true").As<TKey>();
            }
        }

        /// <inheritdoc />
        public async virtual Task<IUnitPagedResult<T>> GetPagedAsync<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IPagedQuery
        {
            var (data, totalRows) = await OnGetPagedAsync(request, cancellationToken);

            var mappedData = data.Select(Map) ?? throw new InvalidOperationException($"Mapping from {typeof(T)} to {typeof(TDb)} failed");

            return UnitPagedResult.FromResult<T>(mappedData!, 
                totalRows, request, UnitAction.Get);
        }

        /// <inheritdoc />
        public abstract Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
