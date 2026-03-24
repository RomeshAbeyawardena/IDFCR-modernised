using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Filters;

/// <summary>
/// Describes a filter that also provides row counts for paging.
/// </summary>
/// <typeparam name="TRequest">The request type used to decide whether the filter should run.</typeparam>
/// <typeparam name="TDb">The queryable element type.</typeparam>
public interface IPagedFilter<TRequest, TDb> : IFilter<TRequest, TDb>
    where TRequest : IPagedQuery
{
    /// <summary>
    /// Applies the filter to the queryable source and returns the filtered query together with the row count.
    /// </summary>
    /// <param name="queryable">The source query.</param>
    /// <param name="request">The request that drives the filter.</param>
    /// <returns>The filtered query and the total number of entries.</returns>
    new (IQueryable<TDb> query, int totalEntries) Apply(IQueryable<TDb> queryable, TRequest request);
}
