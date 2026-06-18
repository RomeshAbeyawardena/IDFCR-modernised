using IDFCR.Abstractions.Caching;
using IDFCR.Caching.Http.Auditing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IDFCR.Caching.Http.Extensions;

/// <summary>
/// Defines extension methods for the IServiceCollection to add grouped distributed caching services. These methods facilitate the registration of necessary services for managing cache groups and their associated cache entries, enabling organized and efficient caching functionality within an application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add the necessary services for grouped distributed caching to the specified IServiceCollection. This method registers the <see cref="IDistributedCacheGroups"/> and <see cref="IDistributedGroupCache"/> implementations, and optionally configures the <see cref="MemoryDistributedCacheOptions"/> if a setup action is provided. If no <see cref="IDistributedCache"/> implementation is already registered, it adds a default in-memory distributed cache.
    /// <para>If you are using a custom <see cref="IDistributedCache"/> implementation, ensure you call that before calling this method.</para>
    /// </summary>
    /// <param name="services">The IServiceCollection to which the caching services will be added.</param>
    /// <param name="setupAction">An optional action to configure the <see cref="MemoryDistributedCacheOptions"/>.</param>
    /// <returns>The updated IServiceCollection with the caching services registered.</returns>
    public static IServiceCollection AddGroupedDistributedCache(this IServiceCollection services,
        Action<MemoryDistributedCacheOptions>? setupAction = null)
    {
        services
            .AddSingleton<IDistributedCacheGroups, DefaultDistributedCacheGroups>()
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

    /// <summary>
    /// Adds the necessary services for grouped distributed caching with auditing to the specified IServiceCollection. This method registers the <see cref="IDistributedCacheGroups"/> and <see cref="IDistributedGroupCache"/> implementations, along with a custom <see cref="IDistributedGroupCacheAuditSink"/> implementation for auditing cache operations. It also optionally configures the <see cref="MemoryDistributedCacheOptions"/> if a setup action is provided. If no <see cref="IDistributedCache"/> implementation is already registered, it adds a default in-memory distributed cache.
    /// <para>If you are using a custom <see cref="IDistributedCache"/> implementation, ensure you call that before calling this method.</para>
    /// </summary>
    /// <typeparam name="TDistributedGroupCacheAuditSink">The type of the audit sink to use for auditing cache operations.</typeparam>
    /// <param name="services">The IServiceCollection to which the caching services will be added.</param>
    /// <param name="setupAction">An optional action to configure the <see cref="MemoryDistributedCacheOptions"/>.</param>
    /// <returns>The updated IServiceCollection with the caching services registered.</returns>
    public static IServiceCollection AddGroupedDistributedCache<TDistributedGroupCacheAuditSink>(
        this IServiceCollection services,
        Action<MemoryDistributedCacheOptions>? setupAction = null)
        where TDistributedGroupCacheAuditSink : class, IDistributedGroupCacheAuditSink
    {
        services
            .AddGroupedDistributedCache(setupAction)
            .AddSingleton<IDistributedGroupCacheAuditSink, TDistributedGroupCacheAuditSink>();

        services.Replace(ServiceDescriptor.Singleton<IDistributedGroupCache, DefaultDistributedGroupCacheWithAuditing>());

        return services;
        
    }

    /// <summary>
    /// Adds the necessary services for grouped distributed caching with logging-based auditing to the specified IServiceCollection. This method registers the <see cref="IDistributedCacheGroups"/> and <see cref="IDistributedGroupCache"/> implementations, along with a <see cref="LoggerDistributedGroupCacheAuditSink"/> implementation for auditing cache operations using logging. It also optionally configures the <see cref="MemoryDistributedCacheOptions"/> if a setup action is provided. If no <see cref="IDistributedCache"/> implementation is already registered, it adds a default in-memory distributed cache.
    /// <para>If you are using a custom <see cref="IDistributedCache"/> implementation, ensure you call that before calling this method.</para>
    /// </summary>
    /// <param name="services">The IServiceCollection to which the caching services will be added.</param>
    /// <param name="setupAction">An optional action to configure the MemoryDistributedCacheOptions.</param>
    /// <returns>The updated IServiceCollection with the caching services registered.</returns>
    public static IServiceCollection AddGroupedDistributedCacheWithLogAuditing(
        this IServiceCollection services,
        Action<MemoryDistributedCacheOptions>? setupAction = null)
    {
        return services.AddGroupedDistributedCache<LoggerDistributedGroupCacheAuditSink>(setupAction);
    }
}
