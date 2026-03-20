using IDCR.Abstractions.Mapper;
using IDCR.Abstractions.Persistence.Extensions;
using IDCR.Abstractions.Results;
using IDCR.Abstractions.Results.Exceptions;

namespace IDCR.Abstractions.Persistence
{
    public interface IRepository<T, TKey>
        where TKey : struct
    {
        ValueTask<IUnitResult<T>> FindAsync(object[] keys, CancellationToken cancellationToken);
        ValueTask<IUnitResult<T>> FindAsync(TKey key, CancellationToken cancellationToken);
        ValueTask<IUnitResult<TKey>> UpsertAsync(T entry, CancellationToken cancellationToken);
        ValueTask<IUnitResult> DeleteAsync(TKey key, CancellationToken cancellationToken);
    }
    //TODO: Inteceptors are coming soon!
    public abstract class RepositoryBase<TDbContext, TCommon, TDb, T, TKey> : IRepository<T, TKey>
        where TKey : struct
        where TDb: class, IMapper<TCommon>, TCommon, IIdentifiable<TKey>
        where T: class, IMapper<TCommon>, TCommon
    {
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
                success = await OnDeleteAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            return UnitResult.FromResult(key, UnitAction.Delete, success, caughtException);
        }

        public async ValueTask<IUnitResult<T>> FindAsync(object[] keys, CancellationToken cancellationToken)
        {
            Exception? caughtException = null;
            bool success = false;
            T? value = null;
            try
            {
                var result = await OnFindAsync(keys, false, cancellationToken);

                if (result is not null)
                {
                    value = Map(result);
                    success = value is not null;
                    caughtException = value is null ? new EntityNotFoundException(typeof(T), keys) : null;
                }
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            return UnitResult.FromResult(value, UnitAction.Get, success, caughtException);
        }

        public async ValueTask<IUnitResult<T>> FindAsync(TKey key, CancellationToken cancellationToken)
        {
            Exception? caughtException = null;
            bool success = false;
            T? value = null;
            try
            {
                var result = await OnFindAsync(key, false, cancellationToken);

                if (result is not null)
                {
                    value = Map(result);
                    success = value is not null;
                    caughtException = value is null ? new EntityNotFoundException(typeof(T), key) : null;
                }
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            return UnitResult.FromResult(value, UnitAction.Get, success, caughtException);
        }

        public async ValueTask<IUnitResult<TKey>> UpsertAsync(T entry, CancellationToken cancellationToken)
        {
            try
            {
                var dbValue = Map(entry) ?? throw new NullReferenceException($"Mapping from {typeof(T)} to {typeof(TDb)} failed");

                if (EqualityComparer<TKey>.Default.Equals(dbValue.Id, default))
                {
                    var id = await OnAddAsync(dbValue, entry, cancellationToken);
                    
                    return UnitResult.FromResult(id, UnitAction.Add);
                }
                else
                {
                    var foundEntry = await OnFindAsync(dbValue.Id, true, cancellationToken);

                    if (foundEntry is null)
                    {
                        return UnitResult.NotFound<TKey>(dbValue.Id, new EntityNotFoundException(typeof(T), dbValue.Id));
                    }

                    foundEntry.Apply(dbValue);
                    var id = await OnUpdateAsync(foundEntry, entry, cancellationToken);
                    
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
