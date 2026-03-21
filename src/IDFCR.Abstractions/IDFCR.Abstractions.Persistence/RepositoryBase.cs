using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Persistence.Extensions;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Exceptions;

namespace IDFCR.Abstractions.Persistence
{
    public abstract class RepositoryBase<TCommon, TDb, T, TKey>(IEntityInterceptorFactory entityInterceptorFactory) : IRepository<T, TKey>
        where TKey : struct
        where TDb: class, IMapper<TCommon>, TCommon, IIdentifiable<TKey>
        where T: class, IMapper<TCommon>, TCommon
    {
        private async ValueTask<IUnitResult<T>> WrapFindResult(Func<CancellationToken, Task<TDb?>> onFindAsync, object key, CancellationToken cancellationToken)
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
            await entityInterceptorFactory.InvokeAsync(await entityInterceptorFactory.
                GetEntityInterceptorsAsync(context, cancellationToken), context, cancellationToken);

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
        protected abstract Task<TDb?> OnFindAsync(TKey key, bool trackChanges, CancellationToken cancellationToken);
        protected abstract Task<TDb?> OnFindAsync(object[] keys, bool trackChanges, CancellationToken cancellationToken);
        protected abstract Task<bool> OnDeleteAsync(TKey key, CancellationToken cancellationToken);

        protected abstract bool IsHandled(Exception exception);

        public async ValueTask<IUnitResult> DeleteAsync(TKey key, CancellationToken cancellationToken)
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
                    
                    var context = await InvokeInterceptorsAsync(EntityContextBehaviorStage.Pre,
                        EntityContextBehavior.Update, dbValue, cancellationToken);

                    if (context.BypassOperation)
                    {
                        return UnitResult.FromResult(dbValue.Id, UnitAction.None)
                            .AddMeta("bypassed", true).As<TKey>();
                    }

                    foundEntry.Apply(dbValue);
                    var id = await OnUpdateAsync(foundEntry, entry, cancellationToken);
                    await InvokeInterceptorsAsync(EntityContextBehaviorStage.Post,
                        EntityContextBehavior.Update, dbValue, cancellationToken);

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
