using IDFCR.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Caching.Http.Extensions;

/// <summary>
/// Defines extension methods for the IServiceCollection to add grouped distributed caching services. These methods facilitate the registration of necessary services for managing cache groups and their associated cache entries, enabling organized and efficient caching functionality within an application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the necessary services for grouped distributed caching to the specified IServiceCollection. This method registers the IDistributedCacheGroups and IDistributedGroupCache implementations, and optionally configures the MemoryDistributedCacheOptions if a setup action is provided. If no IDistributedCache implementation is already registered, it adds a default in-memory distributed cache.
    /// <para>If you are using a custom <see cref="IDistributedCache"/> implementation, ensure you call that before calling this method.</para>
    /// </summary>
    /// <param name="services">The IServiceCollection to which the caching services will be added.</param>
    /// <param name="setupAction">An optional action to configure the MemoryDistributedCacheOptions.</param>
    /// <returns>The updated IServiceCollection with the caching services registered.</returns>
    public static IServiceCollection AddGroupedDistributedCache(this IServiceCollection services, Action<MemoryDistributedCacheOptions>? setupAction = null)
    {
        services
            .AddSingleton<IDistributedCacheGroups, DistributedCacheGroups>()
            .AddSingleton<IDistributedGroupCache, DefaultDistributedGroupCache>();

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

        return services;
    }
}
