using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Results;

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
    /// <inheritdoc />
    public Task<bool> CanLookupAsync(object? filter, CancellationToken cancellationToken)
    {
        return Task.FromResult(filter is TFilter);
    }

    /// <summary>
    /// Determines whether an entity of type <typeparamref name="TEntity"/> exists based on the provided filter of type <typeparamref name="TFilter"/>.
    /// </summary>
    /// <param name="filter">The filter to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a unit result indicating whether the entity exists.</returns>
    protected virtual async Task<IUnitResult<bool>> HasResultAsync(TFilter filter, CancellationToken cancellationToken)
    {
        var result = await HasAsync(filter, cancellationToken);

        return UnitResult.FromResult(result);
    }

    /// <summary>
    /// Looks up an entity of type <typeparamref name="TEntity"/> based on the provided filter of type <typeparamref name="TFilter"/>.
    /// </summary>
    /// <param name="filter">The filter to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a unit result with the found entity or an indication that the entity was not found.</returns>
    protected virtual async Task<IUnitResult<TEntity>> LookupResultAsync(TFilter filter, CancellationToken cancellationToken)
    {
        var result = await LookupAsync(filter, cancellationToken);

        if (result is null)
        {
            return UnitResult.NotFound<TEntity>(filter, new NullReferenceException("Entity not found"));
        }

        return UnitResult.FromResult(result);
    }

    /// <summary>
    /// Determines whether an entity of type <typeparamref name="TEntity"/> exists based on the provided filter of type <typeparamref name="TFilter"/>.
    /// </summary>
    /// <param name="filter">The filter to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the entity exists.</returns>
    protected virtual Task<bool> HasAsync(TFilter filter, CancellationToken cancellationToken)
    {
        return Task.FromResult(false);
    }
    /// <summary>
    /// Looks up an entity of type <typeparamref name="TEntity"/> based on the provided filter of type <typeparamref name="TFilter"/>.
    /// </summary>
    /// <param name="filter">The filter to apply for the lookup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the found entity or null if not found.</returns>
    protected virtual Task<TEntity?> LookupAsync(TFilter filter, CancellationToken cancellationToken)
    {
        return Task.FromResult(default(TEntity));
    }

    Task<IUnitResult<bool>> IAsyncLookup<TEntity, TFilter>.HasAsync(TFilter filter, CancellationToken cancellationToken)
    {
        return HasResultAsync(filter, cancellationToken);
    }

    Task<IUnitResult<bool>> IAsyncLookup<TEntity>.HasAsync(object? filter, CancellationToken cancellationToken)
    {
        if (filter is not TFilter typedFilter)
        {
            throw new ArgumentException($"Invalid filter type. Expected {typeof(TFilter).Name}.", nameof(filter));
        }

        return HasResultAsync(typedFilter, cancellationToken);
    }

    Task<IUnitResult<TEntity>> IAsyncLookup<TEntity, TFilter>.LookupAsync(TFilter filter, CancellationToken cancellationToken)
    {
        return LookupResultAsync(filter, cancellationToken);
    }

    Task<IUnitResult<TEntity>> IAsyncLookup<TEntity>.LookupAsync(object? filter, CancellationToken cancellationToken)
    {
        if (filter is not TFilter typedFilter)
        {
            throw new ArgumentException($"Invalid filter type. Expected {typeof(TFilter).Name}.", nameof(filter));
        }

        return LookupResultAsync(typedFilter, cancellationToken);
    }
}