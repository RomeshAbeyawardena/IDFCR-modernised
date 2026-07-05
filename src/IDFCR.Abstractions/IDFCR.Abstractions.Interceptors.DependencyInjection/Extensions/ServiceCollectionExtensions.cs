using IDFCR.Abstractions.DependencyInjection;
using IDFCR.Abstractions.DependencyInjection.Extensions;
using IDFCR.Abstractions.Interceptors.Factories;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Interceptors.Processors;
using IDFCR.Abstractions.Interceptors.Providers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDFCR.Abstractions.Interceptors.DependencyInjection.Extensions;

/// <summary>
/// Defines a static class that provides extension methods for registering entity interceptors in a dependency injection container. The AddInterceptors method allows developers to easily add implementations of the IEntityInterceptor interface from specified assemblies to the service collection, enabling the use of interception mechanisms for managing entity operations within applications and systems. By using this extension method, developers can centralize the registration of entity interceptors, ensuring that they are properly configured and available for use throughout the application when needed.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds implementations of the IEntityInterceptor interface from the specified assemblies to the service collection. This method registers a singleton instance of the DefaultEntityInterceptorFactory as the implementation of the IEntityInterceptorFactory interface, and then scans the provided assemblies for any classes that implement the IEntityInterceptor interface, registering them as transient services in the dependency injection container. By using this method, developers can easily configure their applications to utilize entity interceptors for managing entity operations, allowing for more flexible and reusable interception logic within applications and systems that utilize interception mechanisms.
    /// </summary>
    /// <param name="services">The service collection to which the interceptors will be added.</param>
    /// <param name="assemblies">The assemblies to scan for implementations of the IEntityInterceptor interface.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddInterceptors(this IServiceCollection services, params Assembly[] assemblies)
    {
        var allAssemblies = new List<Assembly>([typeof(DefaultEntityInterceptorFactory).Assembly]);

        allAssemblies.AddRange(assemblies);

        return services
            .AddTransient<IEntityInterceptorFactory, DefaultEntityInterceptorFactory>()
            .AddTransient<IAuditProcessorProvider, DefaultAuditProcessorProvider>()
            .AddScoped<IScopedResources, DefaultScopedResources>()
            .ScanGenericServices<IEntityInterceptor>(ServiceLifetime.Transient, [.. allAssemblies])
            .ScanGenericServices<IAuditProcessor>(ServiceLifetime.Transient, [.. allAssemblies]);
    }
}
