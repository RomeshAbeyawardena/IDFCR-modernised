using IDFCR.Abstractions.Results;
using System.Data.Entity;

namespace IDFCR.Abstractions.Filters;

/// <summary>
/// Helper methods for applying simple paging to a query.
/// </summary>
public static class PagedFilter
{
    /// <summary>
    /// Applies skip/take paging using the supplied request values.
    /// </summary>
    /// <typeparam name="TDb">The queryable element type.</typeparam>
    /// <typeparam name="TRequest">The paging request type.</typeparam>
    /// <param name="query">The source query.</param>
    /// <param name="request">The paging request.</param>
    /// <param name="defaultSize">The page size to use when the request does not provide one.</param>
    /// <param name="customCount">An optional custom count function.</param>
    /// <returns>The paged query and total row count before paging.</returns>
    public static (IQueryable<TDb> query, int totalEntries) ApplyPaging<TDb, TRequest>(IQueryable<TDb> query, TRequest request, int defaultSize = 500, Func<IQueryable<TDb>, int>? customCount = null)
        where TRequest : IPagedQuery
    {
        var totalEntries = customCount is null ? query.Count() : customCount(query);
        var pageIndex = request.PageIndex ?? 0;
        var pageSize = request.PageSize ?? defaultSize;

        var skip = pageIndex * pageSize;

        query = query.Skip(skip).Take(pageSize);
        return (query, totalEntries);
    }
}
