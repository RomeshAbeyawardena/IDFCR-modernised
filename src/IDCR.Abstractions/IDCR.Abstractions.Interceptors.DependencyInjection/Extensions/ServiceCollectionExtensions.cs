using IDCR.Abstractions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDCR.Abstractions.Interceptors.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInterceptors(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services
            .AddSingleton<IEntityInterceptorFactory, DefaultEntityInterceptorFactory>()
            .AddGenericServices<IEntityInterceptor>(ServiceLifetime.Transient, assemblies);
    }
}
