using IDFCR.Abstractions.Cli.Dispatchers;
using IDFCR.Abstractions.Cli.Operations;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDFCR.Abstractions.Cli.Extensions.Extensions;

internal static class ServiceCollectionExtensions
{
    private static IServiceCollection AddBaseCommandServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICommandRouteDispatcher, DefaultCommandRouteDispatcher>();
    }

    private static IServiceCollection AddFeatureCommandServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.Scan(ts => ts.FromAssemblies(assemblies)
            .AddClasses(t => t.HasInterface<IInjectableCommandOperation>())
            .AsImplementedInterfaces()
            .WithServiceKey(GetServiceKey)
            .WithTransientLifetime()
        );
    }

    private static object? GetServiceKey(Type type)
    {
        FeatureCommandAttribute? attribute;
        if (type.HasInterface<IInjectableCommandOperationRoot>()
            || (attribute = type.GetCustomAttribute<FeatureCommandAttribute>()) is null)
        {
            return null;
        }
        var serviceKey = attribute.ToString();
        
        return serviceKey;
    }

    public static IServiceCollection AddInjectableCommandServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.AddBaseCommandServices()
            .AddFeatureCommandServices(assemblies);
    }
}
