using IDFCR.Abstractions.DependencyInjection.Extensions;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Outbox.Handlers;
using IDFCR.Abstractions.Outbox.Interceptors;
using IDFCR.Outbox.Extensions.Dispatchers;
using IDFCR.Utilities;
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

    private static Assembly[] GetAssembliesOrDefault(IAssemblyDescriptor<OutboxServiceType> descriptor, 
        OutboxServiceType type, 
        IEnumerable<Assembly> defaultFallbackAssemblies)
    {
        var assemblies = descriptor.GetAssemblies(type);

        if (!assemblies.Any())
        {
            assemblies = defaultFallbackAssemblies;
        }

        return [.. assemblies];
    }

    /// <summary>
    /// Adds background services related to the outbox pattern to the specified <see cref="IServiceCollection"/>. This method registers the necessary services for processing outbox messages and tracking their status in a background context, allowing developers to easily integrate outbox functionality into their applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// <para>This method uses dynamic assembly scanning by <see cref="OutboxServiceType"/> to discover and register the appropriate services via a builder pattern.</para>
    /// </summary>
    /// <typeparam name="TOutboxPipeline">The type of the outbox pipeline to be registered.</typeparam>
    /// <typeparam name="TMessage">The type of the message to be processed by the outbox pipeline.</typeparam>
    /// <typeparam name="TPagedQuery">The type of the paged query to be used for retrieving messages.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the outbox services will be added.</param>
    /// <param name="configureAssemblyDescriptorBuilder">An action to configure the assembly descriptor builder for the outbox services.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the outbox services added.</returns>
    public static IServiceCollection AddOutboxPatternBackgroundServices<TOutboxPipeline, TMessage, TPagedQuery>(this IServiceCollection services, 
        Action<IAssemblyDescriptorBuilder<OutboxServiceType>> configureAssemblyDescriptorBuilder)
        where TOutboxPipeline : class, IOutboxPipeline
    {
        var builder = AssemblyDescriptorBuilder.Create<OutboxServiceType>();
        configureAssemblyDescriptorBuilder(builder);
        var descriptor = builder.Build();

        var defaultFallbackAssemblies = descriptor.GetAssemblies(OutboxServiceType.Unknown);

        var assemblies = GetAssembliesOrDefault(descriptor, OutboxServiceType.Dispatcher, defaultFallbackAssemblies);
        services.ScanGenericServices<IOutboxDispatcher>(ServiceLifetime.Scoped, [.. assemblies]);

        assemblies = GetAssembliesOrDefault(descriptor, OutboxServiceType.Pipeline, defaultFallbackAssemblies);
        services.ScanGenericServices<IOutboxPipeline>(ServiceLifetime.Scoped, [.. assemblies]);

        assemblies = GetAssembliesOrDefault(descriptor, OutboxServiceType.Publisher, defaultFallbackAssemblies);
        services.ScanGenericServices<IOutboxPublisher>(ServiceLifetime.Scoped, [.. assemblies]);

        assemblies = GetAssembliesOrDefault(descriptor, OutboxServiceType.Reader, defaultFallbackAssemblies);
        services.ScanGenericServices<IOutboxReader>(ServiceLifetime.Scoped, [.. assemblies]);

        return services
            .AddScoped(typeof(IOutboxDispatcherFactory<,>), typeof(DefaultOutboxDispatcherFactory<,>))
            .AddScoped(typeof(IOutboxReaderFactory<>), typeof(DefaultOutboxReaderFactory<>));
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
        services.AddSingleton<IOutboxPipeline, TOutboxPipeline>()
                .ScanGenericServices<IOutboxReader>(ServiceLifetime.Scoped, assemblies)
                .ScanGenericServices<IOutboxPublisher>(ServiceLifetime.Scoped, assemblies)
                .ScanGenericServices<IOutboxDispatcher>(ServiceLifetime.Scoped, assemblies);

        return services
            .AddScoped(typeof(IOutboxDispatcherFactory<,>), typeof(DefaultOutboxDispatcherFactory<,>))
            .AddScoped(typeof(IOutboxReaderFactory<>), typeof(DefaultOutboxReaderFactory<>));

    }
}
