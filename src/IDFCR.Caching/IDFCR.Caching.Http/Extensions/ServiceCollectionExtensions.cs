using IDFCR.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Caching.Http.Extensions;

/// <summary>
/// Defines extension methods for the IServiceCollection interface to facilitate the registration of caching services and related dependencies in a .NET application. These extensions provide a convenient way to configure and add caching functionality, including distributed cache groups, to the application's dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the necessary services for managing grouped distributed caching to the IServiceCollection. This method registers the IDistributedCacheGroups and IDistributedGroupCache implementations, enabling the application to utilize grouped caching functionality. It allows for organized management of cache entries by grouping related cache keys together, facilitating efficient retrieval and storage of cached data.
    /// </summary>
    /// <param name="services">The IServiceCollection to which the caching services will be added.</param>
    /// <param name="setupAction">An optional action to configure the MemoryDistributedCacheOptions.</param>
    /// <returns>The updated IServiceCollection with the caching services registered.</returns>
    public static IServiceCollection AddGroupedDistributedCache(this IServiceCollection services, Action<MemoryDistributedCacheOptions>? setupAction = null)
    {
        if (!services.Any(s => s.ServiceType == typeof(IDistributedCache)))
        {
            if (setupAction is null)
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddDistributedMemoryCache(setupAction);
            }
        }

        services.AddSingleton<IDistributedCacheGroups, DistributedCacheGroups>();
        services.AddSingleton<IDistributedGroupCache, DefaultDistributedGroupCache>();
        return services;
    }
}
