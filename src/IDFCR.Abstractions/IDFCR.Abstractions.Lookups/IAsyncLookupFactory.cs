using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Metadata.Lookups;

namespace IDFCR.Abstractions.Lookups;

/// <summary>
/// Represents a factory for creating instances of <see cref="IAsyncLookup{TEntity}"/> and <see cref="IAsyncLookup{TEntity, TFilter}"/>.
/// </summary>
public interface IAsyncLookupFactory
{
    /// <summary>
    /// Determines whether an entity exists based on the provided filter.
    /// </summary>
    /// <param name="filter">The filter criteria to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if an entity exists that matches the filter criteria; otherwise, false.</returns>
    Task<bool> HasAsync<TEntity>(object? filter, CancellationToken cancellationToken);
    
    /// <summary>
    /// Determines whether an entity exists based on the provided filter of type <typeparamref name="TFilter"/>.
    /// </summary>
    /// <param name="filter">The filter criteria to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if an entity exists that matches the filter criteria; otherwise, false.</returns>
    Task<bool> HasAsync<TEntity, TFilter>(TFilter filter, CancellationToken cancellationToken)
        where TEntity : class
        where TFilter : IFilter;

    /// <summary>
    /// Looks up entities of type <typeparamref name="TEntity"/> based on the provided filter.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities to look up.</typeparam>
    /// <param name="filter">The filter criteria to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The results of the lookup operation, which may be empty if no results were found.</returns>
    Task<ILookupResults<TEntity>> LookupAsync<TEntity>(object filter, CancellationToken cancellationToken)
        where TEntity : class;

    /// <summary>
    /// Looks up entities of type <typeparamref name="TEntity"/> based on the provided filter of type <typeparamref name="TFilter"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities to look up.</typeparam>
    /// <typeparam name="TFilter">The type of the filter to apply for the lookup.</typeparam>
    /// <param name="filter">The filter criteria to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The results of the lookup operation, which may be empty if no results were found.</returns>
    Task<ILookupResults<TEntity>> LookupAsync<TEntity, TFilter>(TFilter filter, CancellationToken cancellationToken)
        where TEntity : class
        where TFilter : IFilter;
}
