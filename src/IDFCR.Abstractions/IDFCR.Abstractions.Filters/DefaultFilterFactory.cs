using IDFCR.Abstractions.Results;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.WebRequestMethods;

namespace IDFCR.Abstractions.Filters;

internal class DefaultFilterFactory(IEnumerable<IFilter> filters, IServiceProvider serviceProvider) : IFilterFactory
{
    public IEnumerable<IFilter<TRequest, TDb>> GetFilters<TRequest, TDb>()
    {
        var filterList = filters.OfType<IFilter<TRequest, TDb>>()
            .Cast<IFilter<TRequest, TDb>>()
            .ToList();

        var genericFilters = serviceProvider.GetServices<IFilter<TRequest, TDb>>();
        filterList.AddRange(genericFilters);

        return filterList;
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

    public (IQueryable<TDb> query, int totalRows) ApplyPaged<TDb, TRequest>(IQueryable<TDb> queryable, TRequest request, Func<IQueryable<TDb>, int>? customCount) where TRequest : IPagedQuery
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

        (query, totalEntries) = PagedFilter.ApplyPaging(query, request, customCount: customCount);
        
        return (query, totalEntries);
    }
}
