using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDFCR.Abstractions.Lookups.Extensions;

/// <summary>
/// Defines extension methods for registering async lookup services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the default async lookup factory and scans assemblies for implementations of <see cref="IAsyncLookup{T}"/>.
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <param name="assemblies">The assemblies to scan for async lookup implementations.</param>
    public static void AddAsyncLookupFactoryAndLookups(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddTransient<IAsyncLookupFactory, DefaultAsyncLookupFactory>();

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(filter => filter
                .AssignableTo(typeof(IAsyncLookup<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());
    }
}
