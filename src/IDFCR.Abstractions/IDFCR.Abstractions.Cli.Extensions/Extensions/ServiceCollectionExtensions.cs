using IDFCR.Abstractions.Cli.Dispatchers;
using IDFCR.Abstractions.Cli.Operations;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Defines extension methods for the IServiceCollection interface to register services related to injectable command operations in a CLI application. This class provides methods to add base command services and feature-specific command services by scanning assemblies for classes that implement the IInjectableCommandOperation interface. The registered services are configured with transient lifetimes and can be resolved using service keys based on the FeatureCommandAttribute. By using these extension methods, developers can easily set up dependency injection for their CLI applications and manage command operations in a modular and extensible way.
/// </summary>
public static class ServiceCollectionExtensions
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

    /// <summary>
    /// Adds services related to injectable command operations in a CLI application by scanning the specified assemblies for classes that implement the IInjectableCommandOperation interface. This method registers the base command services and feature-specific command services with transient lifetimes, allowing them to be resolved using service keys based on the FeatureCommandAttribute. By calling this method, developers can easily set up dependency injection for their CLI applications and manage command operations in a modular and extensible way.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddInjectableCommandServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.AddBaseCommandServices()
            .AddFeatureCommandServices(assemblies);
    }
}
