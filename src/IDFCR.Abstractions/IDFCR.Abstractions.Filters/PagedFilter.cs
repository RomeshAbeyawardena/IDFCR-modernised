using IDFCR.Abstractions.Results;
using System.Data.Entity;

namespace IDFCR.Abstractions.Filters;

public static class PagedFilter
{
    public static (IQueryable<TDb> query, int totalEntries) ApplyPaging<TDb,TRequest>(IQueryable<TDb> query, TRequest request, int defaultSize = 500)
        where TRequest : IPagedQuery
    {
        var totalEntries = query.Count();
        var pageIndex = request.PageIndex ?? 0;
        var pageSize = request.PageSize ?? defaultSize;

        var skip = pageIndex * pageSize;

        query = query.Skip(skip).Take(pageSize);
        return (query, totalEntries);
    }
}
