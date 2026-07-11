
using IDFCR.Abstractions.DependencyInjection.Extensions;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Outbox.Handlers;
using IDFCR.Abstractions.Outbox.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDFCR.Abstractions.Outbox.Extensions;

/// <summary>
/// Represents extension methods for <see cref="IServiceCollection"/> to add services related to the outbox pattern, allowing for the processing of outbox messages and the tracking of their status. This class provides a method for registering services that implement the outbox pattern, enabling developers to easily integrate outbox functionality into their applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services related to the outbox pattern to the specified <see cref="IServiceCollection"/>. This method registers the necessary services for processing outbox messages and tracking their status, allowing developers to easily integrate outbox functionality into their applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// <typeparam name="TOutboxEntityNotificationHandler">The type of the outbox entity notification handler to be registered.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the outbox services will be added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the outbox services added.</returns>
    public static IServiceCollection AddOutboxPattern<TOutboxEntityNotificationHandler>(this IServiceCollection services)
        where TOutboxEntityNotificationHandler : class, IOutboxEntityNotificationHandler
    {
        var assembly = typeof(OutboxInterceptor).Assembly;

        return services
            .AddScoped<IOutboxEntityNotificationHandler, TOutboxEntityNotificationHandler>()
            .ScanGenericServices<IEntityInterceptor>(ServiceLifetime.Transient, assembly);
    }

    /// <summary>
    /// Adds background services related to the outbox pattern to the specified <see cref="IServiceCollection"/>. This method registers the necessary services for processing outbox messages and tracking their status in a background context, allowing developers to easily integrate outbox functionality into their applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// <typeparam name="TOutboxPipeline"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TPagedQuery"></typeparam>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddOutboxPatternBackgroundServices<TOutboxPipeline, TMessage, TPagedQuery>(this IServiceCollection services, params Assembly[] assemblies)
        where TOutboxPipeline : class, IOutboxPipeline
    {
        return services.AddSingleton<IOutboxPipeline, TOutboxPipeline>()
                .ScanGenericServices<IOutboxReader>(ServiceLifetime.Scoped, assemblies)
                .ScanGenericServices<IOutboxPublisher>(ServiceLifetime.Scoped, assemblies)
                .ScanGenericServices<IOutboxDispatcher>(ServiceLifetime.Scoped, assemblies);

    }
}
