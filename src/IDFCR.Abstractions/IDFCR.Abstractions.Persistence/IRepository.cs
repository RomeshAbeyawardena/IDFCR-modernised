using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Persistence;

public interface IRepository<T, TKey>
    where TKey : struct
{
    Task<IUnitResult<T>> FindAsync(object[] keys, CancellationToken cancellationToken);
    Task<IUnitResult<T>> FindAsync(TKey key, CancellationToken cancellationToken);
    Task<IUnitResult<TKey>> UpsertAsync(T entry, CancellationToken cancellationToken);
    Task<IUnitResult> DeleteAsync(TKey key, CancellationToken cancellationToken);

    Task<IUnitPagedResult<T>> GetPagedAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IPagedQuery;
}
