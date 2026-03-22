using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Filters;

public interface IFilterFactory
{
    IQueryable<TDb> Apply<TDb, TRequest>(IQueryable<TDb> queryable, TRequest request);
    (IQueryable<TDb> query, int totalRows) ApplyPaged<TDb, TRequest>(IQueryable<TDb> queryable, TRequest request, Func<IQueryable<TDb>, int>? customCount = null)
        where TRequest : IPagedQuery;
    IEnumerable<IFilter<TRequest, TDb>> GetFilters<TRequest, TDb>();
    IEnumerable<IPagedFilter<TRequest, TDb>> GetPagedFilters<TRequest, TDb>()
        where TRequest : IPagedQuery;
}
