using IDFCR.Abstractions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDFCR.Abstractions.Filters.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGenericFilter(this IServiceCollection services, Type genericType)
    {
        if (genericType.GenericTypeArguments.Length != 2)
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(genericType.GetGenericArguments().Length, 2, nameof(genericType));
        }

        if (genericType.GetCustomAttribute<GlobalFilterAttribute>() is null)
        {
            throw new InvalidCastException($"Unable to add a generic filter that has not been marked as a {nameof(GlobalFilterAttribute)}.");
        }

        services.AddTransient(typeof(IFilter<,>), genericType);

        var interfaces = genericType.GetInterfaces();

        if (interfaces.Any(a => a.Name.StartsWith(nameof(IPagedFilter<,>))))
        {
            services.AddTransient(typeof(IPagedFilter<,>), genericType);
        }

        return services;
    }

    

    public static IServiceCollection ScanFilters(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services
            .AddTransient<IFilterFactory, DefaultFilterFactory>()
            .ScanGenericServices<IFilter>(ServiceLifetime.Transient, f => f.WithoutAttribute<GlobalFilterAttribute>(), assemblies);
    }
}
