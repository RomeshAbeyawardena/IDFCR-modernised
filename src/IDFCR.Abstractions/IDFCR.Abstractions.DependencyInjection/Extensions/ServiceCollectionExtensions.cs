using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace IDFCR.Abstractions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
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

    public static IServiceCollection ScanGenericServices<TGenericServiceType>(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        return services.ScanGenericServices<TGenericServiceType>(lifetime, null, assemblies);
    }
}
