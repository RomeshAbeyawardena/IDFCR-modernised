using IDFCR.Abstractions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDFCR.Abstractions.Interceptors.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInterceptors(this IServiceCollection services, params Assembly[] assemblies)
    {
        var allAssemblies = new List<Assembly>([typeof(DefaultEntityInterceptorFactory).Assembly]);

        allAssemblies.AddRange(assemblies);

        return services
            .AddSingleton<IEntityInterceptorFactory, DefaultEntityInterceptorFactory>()
            .AddGenericServices<IEntityInterceptor>(ServiceLifetime.Transient, [.. allAssemblies]);
    }
}
