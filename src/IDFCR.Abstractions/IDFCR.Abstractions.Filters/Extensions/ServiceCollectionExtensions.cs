using IDFCR.Abstractions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDFCR.Abstractions.Filters.Extensions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class GlobalFilter : Attribute
{

}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGenericFilter(this IServiceCollection services, Type genericType)
    {
        if (genericType.GenericTypeArguments.Length != 2)
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(genericType.GetGenericArguments().Length, 2, nameof(genericType));
        }

        if (genericType.GetCustomAttribute<GlobalFilter>() is null)
        {
            throw new InvalidCastException($"Unable to add a generic filter that has not been marked as a {nameof(GlobalFilter)}.");
        }


        return services.AddTransient(typeof(IFilter<,>), genericType);
    }

    public static IServiceCollection ScanFilters(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.ScanGenericServices<IFilter>(ServiceLifetime.Transient, f => f.WithoutAttribute<GlobalFilter>(), assemblies);
    }
}
