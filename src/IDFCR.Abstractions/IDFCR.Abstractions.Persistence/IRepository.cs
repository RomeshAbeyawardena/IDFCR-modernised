using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Persistence;

/// <summary>
/// Describes the repository contract used by the abstractions layer.
/// </summary>
/// <typeparam name="T">The domain model type.</typeparam>
/// <typeparam name="TKey">The identifier type.</typeparam>
public interface IRepository<T, TKey> : IUnitOfWork
    where TKey : struct
{
    /// <summary>
    /// Finds an entity by composite keys.
    /// </summary>
    Task<IUnitResult<T>> FindAsync(object[] keys, CancellationToken cancellationToken);

    /// <summary>
    /// Finds an entity by its identifier.
    /// </summary>
    Task<IUnitResult<T>> FindAsync(TKey key, CancellationToken cancellationToken);

    /// <summary>
    /// Inserts or updates an entity.
    /// </summary>
    Task<IUnitResult<TKey>> UpsertAsync(T entry, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an entity by its identifier.
    /// </summary>
    Task<IUnitResult> DeleteAsync(TKey key, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a paged result for the supplied request.
    /// </summary>
    Task<IPagedUnitResult<T>> GetPagedAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IPagedQuery;
}
