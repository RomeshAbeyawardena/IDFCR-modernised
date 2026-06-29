using IDFCR.Abstractions.Results;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.Filters;

/// <summary>
/// Default <see cref="IFilterFactory"/> backed by the service provider.
/// </summary>
internal class DefaultFilterFactory(IServiceProvider serviceProvider) : IFilterFactory
{
    /// <inheritdoc />
    public IEnumerable<IFilter<TDb>> GetFilters<TRequest, TDb>()
    {
        List<IFilter<TDb>> filters = [];
        
        var filterType = typeof(IFilter<,>);
        var dbType = typeof(TDb);
        foreach (var @interface in typeof(TRequest).GetInterfaces())
        {
            filters.AddRange(
                serviceProvider.GetServices(filterType.MakeGenericType(dbType, @interface))
                .Where(x => x != null)
                .Select(x => (IFilter<TDb>)x!));
        }

        filters.AddRange(serviceProvider.GetServices<IFilter<TRequest, TDb>>());

        return filters;
    }

    /// <inheritdoc />
    public IEnumerable<IPagedFilter<TRequest, TDb>> GetPagedFilters<TRequest, TDb>() where TRequest : IPagedQuery
    {
        return serviceProvider.GetServices<IPagedFilter<TRequest, TDb>>();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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
