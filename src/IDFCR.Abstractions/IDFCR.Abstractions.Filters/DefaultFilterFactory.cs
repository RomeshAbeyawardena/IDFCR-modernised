using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Filters;

internal class DefaultFilterFactory(IEnumerable<IFilter> filters) : IFilterFactory
{
    public IEnumerable<IFilter<TRequest, TDb>> GetFilters<TRequest, TDb>()
    {
        return filters.OfType<IFilter<TRequest, TDb>>();
    }

    public IEnumerable<IPagedFilter<TRequest, TDb>> GetPagedFilters<TRequest, TDb>() where TRequest : IPagedQuery
    {
        return filters.OfType<IPagedFilter<TRequest, TDb>>();
    }

    public IQueryable<TDb> Apply<TDb, TRequest>(IQueryable<TDb> queryable, TRequest request)
    {
        IQueryable<TDb> query = queryable;
        foreach (var filter in GetFilters<TRequest, TDb>())
        {
            if (filter.CanFilter(request))
            {
                query = filter.Apply(query, request);
            }
        }

        return query;
    }

    public (IQueryable<TDb> query, int totalRows) ApplyWithTotalRowsReturned<TDb, TRequest>(IQueryable<TDb> queryable, TRequest request) where TRequest : IPagedQuery
    {
        IQueryable<TDb> query = queryable;
        int totalEntries;

        var filters = GetPagedFilters<TRequest, TDb>().ToArray();

        for (int i = 0; i < filters.Length; i++)
        {
            var filter = filters[i];

            if (filter.CanFilter(request))
            {
                (query, _) = filter.Apply(query, request);
            }
        }

        (query, totalEntries) = PagedFilter.ApplyPaging(query, request);
        
        return (query, totalEntries);
    }
}
