using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace IDFCR.Abstractions.DependencyInjection.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to scan and register generic services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Scans the specified assemblies for types assignable to the generic service type and registers them with their implemented interfaces.
    /// </summary>
    /// <typeparam name="TGenericServiceType">The generic service type to scan for.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> to register the services with.</param>
    /// <param name="typeFilter">An optional filter function to further refine which implementation types should be registered.</param>
    /// <param name="assemblies">The assemblies to scan for service implementations.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection ScanGenericServices<TGenericServiceType>(this IServiceCollection services, ServiceLifetime lifetime,
        Func<IImplementationTypeFilter, IImplementationTypeFilter>? typeFilter, params Assembly[] assemblies)
    {
        return services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c =>
            {
                var filter = c.AssignableTo<TGenericServiceType>();
                if (typeFilter is not null)
                {
                    typeFilter(filter);
                }
            }, false)
            .AsImplementedInterfaces()
            .WithLifetime(lifetime)
        );
    }

    /// <summary>
    /// Scans the specified assemblies for types assignable to the generic service type and registers them with their implemented interfaces.
    /// </summary>
    /// <typeparam name="TGenericServiceType">The generic service type to scan for.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> to register the services with.</param>
    /// <param name="assemblies">The assemblies to scan for service implementations.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection ScanGenericServices<TGenericServiceType>(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        return services.ScanGenericServices<TGenericServiceType>(lifetime, null, assemblies);
    }
}
