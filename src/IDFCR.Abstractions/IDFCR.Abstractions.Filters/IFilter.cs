namespace IDFCR.Abstractions.Filters;

/// <summary>
/// Marker interface for filters.
/// </summary>
public interface IFilter
{

}

/// <summary>
/// Represents a filter that can inspect a request and apply it to a queryable source.
/// </summary>
public interface IFilter<TDb> : IFilter
{
    /// <summary>
    /// Gets a value indicating whether the filter can be applied to the supplied request.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    bool CanFilter(object? request);
    /// <summary>
    /// Applies the filter to the queryable source.
    /// </summary>
    /// <param name="queryable">The source query.</param>
    /// <param name="request">The request that drives the filter.</param>
    /// <returns>The filtered query.</returns>
    IQueryable<TDb> Apply(IQueryable<TDb> queryable, object? request);
}

/// <summary>
/// Describes a filter that can inspect a request and apply it to a queryable source.
/// </summary>
/// <typeparam name="TRequest">The request type used to decide whether the filter should run.</typeparam>
/// <typeparam name="TDb">The queryable element type.</typeparam>
public interface IFilter<TRequest, TDb> : IFilter<TDb>
{
    /// <summary>
    /// Gets a value indicating whether the filter can be applied to the supplied request.
    /// </summary>
    /// <param name="request">The request to evaluate.</param>
    /// <returns><see langword="true"/> when the filter should run; otherwise <see langword="false"/>.</returns>
    bool CanFilter(TRequest request);

    /// <summary>
    /// Applies the filter to the queryable source.
    /// </summary>
    /// <param name="queryable">The source query.</param>
    /// <param name="request">The request that drives the filter.</param>
    /// <returns>The filtered query.</returns>
    IQueryable<TDb> Apply(IQueryable<TDb> queryable, TRequest request);
}
