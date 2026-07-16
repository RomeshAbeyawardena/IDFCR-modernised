using IDFCR.Abstractions.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.Lookups;

internal sealed class DefaultAsyncLookupFactory(IServiceProvider serviceProvider) : IAsyncLookupFactory
{
    public async Task<IEnumerable<TEntity>> LookupAsync<TEntity>(object filter, CancellationToken cancellationToken)
        where TEntity : class
    {
        using var scope = serviceProvider.CreateScope();

        var lookupProviders = scope.ServiceProvider.GetServices<IAsyncLookup<TEntity>>();

        List<TEntity> lookedUpEntities = [];

        foreach (var lookup in lookupProviders)
        {
            if (await lookup.CanLookupAsync(filter, cancellationToken))
            {
                var entity = await lookup.LookupAsync(filter, cancellationToken);

                if (entity is not null)
                {
                    lookedUpEntities.Add(entity);
                }
            }
        }

        return lookedUpEntities;
    }

    public async Task<IEnumerable<TEntity>> LookupAsync<TEntity, TFilter>(TFilter filter, CancellationToken cancellationToken)
        where TEntity : class
        where TFilter : IFilter
    {
        using var scope = serviceProvider.CreateScope();

        var lookupProviders = scope.ServiceProvider.GetServices<IAsyncLookup<TEntity, TFilter>>();

        List<TEntity> lookedUpEntities = [];

        foreach(var lookup in lookupProviders)
        {
            var entity = await lookup.LookupAsync(filter, cancellationToken);

            if (entity is not null)
            {
                lookedUpEntities.Add(entity);
            }
        }

        return lookedUpEntities;
    }
}