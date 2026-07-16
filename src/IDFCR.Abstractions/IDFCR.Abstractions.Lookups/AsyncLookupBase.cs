using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Lookups;

/// <summary>
/// Represents a base class for implementing asynchronous lookup services for entities of type <typeparamref name="TEntity"/> with a specific filter of type <typeparamref name="TFilter"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entities to look up.</typeparam>
/// <typeparam name="TFilter">The type of the filter to apply for the lookup.</typeparam>
public abstract class AsyncLookupBase<TEntity, TFilter> : IAsyncLookup<TEntity, TFilter>
    where TEntity : class
    where TFilter : IFilter
{
    Task<TEntity?> IAsyncLookup<TEntity>.LookupAsync(object? filter, CancellationToken cancellationToken)
    {
        if (filter is not TFilter typedFilter)
        {
            throw new ArgumentException($"Invalid filter type. Expected {typeof(TFilter).Name}.", nameof(filter));
        }

        return LookupAsync(typedFilter, cancellationToken);
    }

    /// <summary>
    /// Looks up an entity of type <typeparamref name="TEntity"/> based on the provided filter of type <typeparamref name="TFilter"/>.
    /// </summary>
    /// <param name="filter">The filter criteria to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The entity that matches the filter criteria.</returns>
    public abstract Task<TEntity?> LookupAsync(TFilter filter, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether the provided filter can be used for lookup.
    /// </summary>
    /// <param name="filter">The filter criteria to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the filter can be used for lookup; otherwise, false.</returns>
    public virtual Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        return Task.FromResult(filter is TFilter);
    }

    /// <summary>
    /// Determines whether an entity exists that matches the provided filter of type <typeparamref name="TFilter"/>.
    /// </summary>
    /// <param name="filter">The filter criteria to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if an entity exists that matches the filter criteria; otherwise, false.</returns>
    public abstract Task<bool> HasAsync(TFilter filter, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether an entity exists that matches the provided filter of type <typeparamref name="TFilter"/>.
    /// </summary>
    /// <param name="filter">The filter criteria to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if an entity exists that matches the filter criteria; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided filter is not of the expected type <typeparamref name="TFilter"/>.</exception>
    public Task<bool> HasAsync(object? filter, CancellationToken cancellationToken)
    {
        if (filter is not TFilter typedFilter)
        {
            throw new ArgumentException($"Invalid filter type. Expected {typeof(TFilter).Name}.", nameof(filter));
        }

        return HasAsync(typedFilter, cancellationToken);
    }
}