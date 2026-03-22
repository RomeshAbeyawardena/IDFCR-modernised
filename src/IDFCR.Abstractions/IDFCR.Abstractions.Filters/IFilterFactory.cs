using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Filters;

public interface IFilterFactory
{
    IQueryable<TDb> Apply<TDb, TRequest>(IQueryable<TDb> queryable, TRequest request);
    (IQueryable<TDb> query, int totalRows) ApplyWithTotalRowsReturned<TDb, TRequest>(IQueryable<TDb> queryable, TRequest request)
        where TRequest : IPagedQuery;
    IEnumerable<IFilter<TRequest, TDb>> GetFilters<TRequest, TDb>();
    IEnumerable<IPagedFilter<TRequest, TDb>> GetPagedFilters<TRequest, TDb>()
        where TRequest : IPagedQuery;
}
