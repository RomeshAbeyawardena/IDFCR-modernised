using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Filters;

/// <summary>
/// Resolves and applies registered filters.
/// </summary>
public interface IFilterFactory
{
    /// <summary>
    /// Applies all matching filters to the supplied query.
    /// </summary>
    IQueryable<TDb> Apply<TDb, TRequest>(IQueryable<TDb> queryable, TRequest request);

    /// <summary>
    /// Applies all matching paged filters and then performs paging.
    /// </summary>
    /// <typeparam name="TDb">The queryable element type.</typeparam>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <param name="queryable">The source query.</param>
    /// <param name="request">The request that drives filtering and paging.</param>
    /// <param name="customCount">An optional count function used instead of <see cref="Queryable.Count{TSource}(IQueryable{TSource})"/>.</param>
    /// <returns>The filtered query and total row count.</returns>
    /// <exception cref="NotSupportedException">Thrown when no paging contract is available.</exception>
    (IQueryable<TDb> query, int totalRows) ApplyPaged<TDb, TRequest>(IQueryable<TDb> queryable, TRequest request, Func<IQueryable<TDb>, int>? customCount = null)
        where TRequest : IPagedQuery;

    /// <summary>
    /// Resolves standard filters for the supplied request and element types.
    /// </summary>
    IEnumerable<IFilter<TDb>> GetFilters<TRequest, TDb>();

    /// <summary>
    /// Resolves paged filters for the supplied request and element types.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TDb">The queryable element type.</typeparam>
    /// <returns>The matching paged filters.</returns>
    IEnumerable<IPagedFilter<TRequest, TDb>> GetPagedFilters<TRequest, TDb>()
        where TRequest : IPagedQuery;
}
