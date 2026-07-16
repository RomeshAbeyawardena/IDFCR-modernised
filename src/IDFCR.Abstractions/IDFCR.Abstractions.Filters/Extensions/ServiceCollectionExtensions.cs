using IDFCR.Abstractions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace IDFCR.Abstractions.Filters.Extensions;

/// <summary>
/// Dependency injection helpers for filters.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a global generic filter type.
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <param name="genericType">The generic filter type to register.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="InvalidCastException">Thrown when the type is not decorated with <see cref="GlobalFilterAttribute"/>.</exception>
    public static IServiceCollection AddGenericFilter(this IServiceCollection services, Type genericType)
    {
        if (genericType.GenericTypeArguments.Length != 2)
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(genericType.GetGenericArguments().Length, 2, nameof(genericType));
        }

        var globalFilterAttribute = genericType.GetCustomAttribute<GlobalFilterAttribute>() 
            ?? throw new InvalidCastException($"Unable to add a generic filter that has not been marked as a {nameof(GlobalFilterAttribute)}.");

        if (globalFilterAttribute.IsStandard)
        {
            services.AddTransient(typeof(IFilter<,>), genericType);
        }

        if (globalFilterAttribute.IsPaged)
        {
            var interfaces = genericType.GetInterfaces();

            if (interfaces.Any(a => a.Name.StartsWith(nameof(IPagedFilter<,>))))
            {
                services.AddTransient(typeof(IPagedFilter<,>), genericType);
            }
        }

        return services;
    }

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

    /// <summary>
    /// Scans assemblies for non-global filters and registers the shared filter factory.
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <param name="registerGlobalPagingFilter">
    /// <see langword="true"/> to register the built-in global paging filter; otherwise, <see langword="false"/> to allow consumers to provide their own paging filter registration.
    /// </param>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection ScanFilters(this IServiceCollection services, bool registerGlobalPagingFilter = true, params Assembly[] assemblies)
    {
        services = services
            .AddTransient<IFilterFactory, DefaultFilterFactory>()
            .ScanGenericServices<IFilter>(ServiceLifetime.Transient, f => f.WithoutAttribute<GlobalFilterAttribute>(), assemblies);

        if (registerGlobalPagingFilter)
        {
            services.AddGenericFilter(typeof(DefaultPagedFilter<,>));
        }

        return services;
    }
}
