using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Metadata.Lookups;
using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Lookups;

/// <summary>
/// Represents an asynchronous lookup service for entities of type <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IAsyncLookup<TEntity>
{
    /// <summary>
    /// Determines whether the provided filter can be used for lookup.
    /// </summary>
    /// <param name="filter">The filter criteria to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the filter can be used for lookup; otherwise, false.</returns>
    Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken);
    /// <summary>
    /// Looks up an entity of type <typeparamref name="TEntity"/> based on the provided filter.
    /// </summary>
    /// <param name="filter">The filter criteria to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The entity that matches the filter criteria.</returns>
    Task<IUnitResult<TEntity>> LookupAsync(object? filter, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether an entity of type <typeparamref name="TEntity"/> exists based on the provided filter.
    /// </summary>
    /// <param name="filter">The filter criteria to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if an entity exists that matches the filter criteria; otherwise, false.</returns>
    Task<IUnitResult<bool>> HasAsync(object? filter, CancellationToken cancellationToken);
}

/// <summary>
/// Represents an asynchronous lookup service for entities of type <typeparamref name="TEntity"/> with a specific filter of type <typeparamref name="TFilter"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entities to look up.</typeparam>
/// <typeparam name="TFilter">The type of the filter to apply for the lookup.</typeparam>
public interface IAsyncLookup<TEntity, TFilter> : IAsyncLookup<TEntity>
    where TEntity : class
    where TFilter : IFilter
{
    /// <summary>
    /// Looks up an entity of type <typeparamref name="TEntity"/> based on the provided filter of type <typeparamref name="TFilter"/>.
    /// </summary>
    /// <param name="filter">The filter criteria to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The entity that matches the filter criteria.</returns>
    Task<IUnitResult<TEntity>> LookupAsync(TFilter filter, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether an entity of type <typeparamref name="TEntity"/> exists based on the provided filter of type <typeparamref name="TFilter"/>.
    /// </summary>
    /// <param name="filter">The filter criteria to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if an entity exists that matches the filter criteria; otherwise, false.</returns>
    Task<IUnitResult<bool>> HasAsync(TFilter filter, CancellationToken cancellationToken);
}
