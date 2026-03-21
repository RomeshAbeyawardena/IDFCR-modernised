using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDFCR.Abstractions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGenericServices<TGenericServiceType>(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        return services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo<TGenericServiceType>(), false)
            .AsImplementedInterfaces()
            .WithLifetime(lifetime)
        );
    }
}
