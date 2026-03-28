using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using BuildTools.Cli.Operations;
using BuildTools.Cli.Dispatchers;
using BuildTools.Cli.Extensions;
namespace BuildTools.Cli.Extensions;

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
