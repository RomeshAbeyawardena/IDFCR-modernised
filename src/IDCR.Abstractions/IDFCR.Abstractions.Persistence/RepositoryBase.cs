using IDCR.Abstractions.Interceptors;
using IDCR.Abstractions.Mapper;
using IDCR.Abstractions.Persistence.Extensions;
using IDCR.Abstractions.Results;
using IDCR.Abstractions.Results.Exceptions;

namespace IDCR.Abstractions.Persistence
{
    internal class RepositoryInterceptorContext
        : IEntityInterceptorContext
    {
        private RepositoryInterceptorContext() { }

        public static RepositoryInterceptorContext Create(EntityContextBehaviorStage Stage,
            EntityContextBehavior Behavior,
            object Model)
        {
            return new RepositoryInterceptorContext 
            { 
                Stage = Stage,
                Behavior = Behavior, 
                Model = Model 
            };
        }

        public EntityContextBehaviorStage Stage { get; init; }
        public EntityContextBehavior Behavior { get; init; }
        public object? Model { get; init; }
        /// <summary>
        /// Gets or sets a value that will cause the behaviour operation to be bypassed, where supported.
        /// </summary>
        public bool BypassOperation { get; set; }
    }

    public abstract class RepositoryBase<TDbContext, TCommon, TDb, T, TKey>(IEntityInterceptorFactory entityInterceptorFactory) : IRepository<T, TKey>
        where TKey : struct
        where TDb: class, IMapper<TCommon>, TCommon, IIdentifiable<TKey>
        where T: class, IMapper<TCommon>, TCommon
    {
        private async ValueTask<IUnitResult<T>> WrapFindResult(Func<CancellationToken, Task<TDb>> onFindAsync, object key, CancellationToken cancellationToken)
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
            object model)
        {
            var context = RepositoryInterceptorContext.Create(stage, behavior, model);
            var interceptors = entityInterceptorFactory
                .GetEntityInterceptors(context);

            foreach (var interceptor in interceptors.OrderBy(x => x.OrderIndex))
            {
                await interceptor.InterceptAsync(context);
            }

            return context;
        }

        protected virtual TDb? Map(T value)
        {
            return value.Map<TDb>(value);
        }

        protected virtual T? Map(TDb value)
        {
            return value.Map<T>(value);
        }

        protected abstract Task<TKey> OnAddAsync(TDb entry, T rawEntry, CancellationToken cancellationToken);
        protected abstract Task<TKey> OnUpdateAsync(TDb entry, T rawEntry, CancellationToken cancellationToken);
        protected abstract Task<TDb> OnFindAsync(TKey key, bool trackChanges, CancellationToken cancellationToken);
        protected abstract Task<TDb> OnFindAsync(object[] keys, bool trackChanges, CancellationToken cancellationToken);
        protected abstract Task<bool> OnDeleteAsync(TKey key, CancellationToken cancellationToken);

        protected abstract bool IsHandled(Exception exception);

        public async ValueTask<IUnitResult> DeleteAsync(TKey key, CancellationToken cancellationToken)
        {
            Exception? caughtException = null;
            bool success = false;
            try
            {
                var dbValue = await OnFindAsync(key, true, cancellationToken);
                var context = await InvokeInterceptorsAsync(EntityContextBehaviorStage.Pre,
                        EntityContextBehavior.Delete, dbValue);
                if (!context.BypassOperation)
                {
                    success = await OnDeleteAsync(key, cancellationToken);
                }
                await InvokeInterceptorsAsync(EntityContextBehaviorStage.Post,
                        EntityContextBehavior.Delete, dbValue);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            return UnitResult.FromResult(key, UnitAction.Delete, success, caughtException);
        }

        public ValueTask<IUnitResult<T>> FindAsync(object[] keys, CancellationToken cancellationToken)
        {
            return WrapFindResult(ct => OnFindAsync(keys, false, ct), keys, cancellationToken);
        }

        public ValueTask<IUnitResult<T>> FindAsync(TKey key, CancellationToken cancellationToken)
        {
            return WrapFindResult(ct => OnFindAsync(key, false, ct), key, cancellationToken);
        }

        public async ValueTask<IUnitResult<TKey>> UpsertAsync(T entry, CancellationToken cancellationToken)
        {
            try
            {
                var dbValue = Map(entry) ?? throw new InvalidOperationException($"Mapping from {typeof(T)} to {typeof(TDb)} failed");

                if (EqualityComparer<TKey>.Default.Equals(dbValue.Id, default))
                {
                    await InvokeInterceptorsAsync(EntityContextBehaviorStage.Pre, 
                        EntityContextBehavior.Insert, dbValue);

                    var id = await OnAddAsync(dbValue, entry, cancellationToken);

                    await InvokeInterceptorsAsync(EntityContextBehaviorStage.Post,
                        EntityContextBehavior.Insert, dbValue);

                    return UnitResult.FromResult(id, UnitAction.Add);
                }
                else
                {
                    var foundEntry = await OnFindAsync(dbValue.Id, true, cancellationToken);

                    if (foundEntry is null)
                    {
                        return UnitResult.NotFound<TKey>(dbValue.Id, new EntityNotFoundException(typeof(T), dbValue.Id));
                    }
                    await InvokeInterceptorsAsync(EntityContextBehaviorStage.Pre,
                        EntityContextBehavior.Update, dbValue);
                    foundEntry.Apply(dbValue);
                    var id = await OnUpdateAsync(foundEntry, entry, cancellationToken);
                    await InvokeInterceptorsAsync(EntityContextBehaviorStage.Post,
                        EntityContextBehavior.Update, dbValue);

                    return UnitResult.FromResult(id, UnitAction.Update);
                }
            }
            catch (Exception exception)
            {
                if (IsHandled(exception))
                {
                    return UnitResult.Failed<TKey>(exception);
                }
                //alert the consumer that the exception was not expected
                return UnitResult.Failed<TKey>(exception).AddMeta("unexpected", "true").As<TKey>();
            }
        }
    }
}
