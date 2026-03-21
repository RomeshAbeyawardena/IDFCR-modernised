using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Persistence;

public interface IRepository<T, TKey>
    where TKey : struct
{
    ValueTask<IUnitResult<T>> FindAsync(object[] keys, CancellationToken cancellationToken);
    ValueTask<IUnitResult<T>> FindAsync(TKey key, CancellationToken cancellationToken);
    ValueTask<IUnitResult<TKey>> UpsertAsync(T entry, CancellationToken cancellationToken);
    ValueTask<IUnitResult> DeleteAsync(TKey key, CancellationToken cancellationToken);
}
