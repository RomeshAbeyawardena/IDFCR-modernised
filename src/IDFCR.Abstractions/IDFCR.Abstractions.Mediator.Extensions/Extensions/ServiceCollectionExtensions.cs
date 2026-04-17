using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDFCR.Abstractions.Mediator.Extensions.Extensions;

/// <summary>
/// Defines extension methods for configuring the IExceptionBehaviourManager and adding MediatR services with exception handling pipelines to the dependency injection container. These extensions provide a convenient way to set up exception handling behavior for MediatR requests, allowing developers to specify how exceptions should be handled and what actions should be taken when exceptions occur during request processing. By using these extension methods, developers can easily integrate consistent and customizable exception handling into their MediatR-based applications.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures and registers an exception behavior manager as a singleton service in the dependency injection
    /// container.
    /// </summary>
    /// <remarks>Call this method during application startup to enable centralized exception handling
    /// behaviors. The configuration delegate allows customization of exception handling strategies before the manager
    /// is registered.</remarks>
    /// <param name="services">The service collection to which the exception behavior manager will be added.</param>
    /// <param name="buildConfiguration">A delegate that configures the exception behavior manager using the provided builder.</param>
    /// <returns>The service collection with the exception behavior manager registered.</returns>
    public static IServiceCollection ConfigureExceptionBehaviourManager(this IServiceCollection services, Action<IExceptionBehaviourManagerBuilder> buildConfiguration)
    {
        var builder = new ExceptionBehaviourManagerBuilder();
        return services.AddSingleton(builder.Build());
    }

    /// <summary>
    /// Adds MediatR services to the specified service collection, along with a default exception handling pipeline. This method allows you to configure MediatR services and register them from the provided assemblies, while also ensuring that any exceptions thrown during request processing are handled by the DefaultExceptionPipeline. The configuration action can be used to further customize the MediatR setup, such as adding additional behaviors or configuring options. By using this extension method, you can easily integrate MediatR with consistent exception handling across your application.
    /// <para>Use this if you want to control the registration of MediatR services and exception handling pipelines.</para>
    /// </summary>
    /// <param name="services">The service collection to which MediatR services will be added.</param>
    /// <param name="configuration">A delegate that configures the MediatR services.</param>
    /// <param name="registerServicesFromAssemblies">A boolean indicating whether to register services from the provided assemblies.</param>
    /// <param name="assemblies">The assemblies from which MediatR services will be registered.</param>
    /// <returns>The service collection with MediatR services and exception handling pipelines registered.</returns>
    public static IServiceCollection AddMediatorServicesAndPipelines(this IServiceCollection services, Action<MediatRServiceConfiguration> configuration, bool registerServicesFromAssemblies, params Assembly[] assemblies)
    {
        return services
            .AddMediatR(cfg =>
            {
                configuration(cfg);
                if (registerServicesFromAssemblies)
                {
                    cfg.RegisterServicesFromAssemblies(assemblies);
                }

                cfg.AddOpenBehavior(typeof(DefaultExceptionPipeline<,,>));
            });
    }

    /// <summary>
    /// Adds MediatR services to the specified service collection, along with a default exception handling pipeline. This method allows you to configure MediatR services and register them from the provided assemblies, while also ensuring that any exceptions thrown during request processing are handled by the DefaultExceptionPipeline. The configuration action can be used to further customize the MediatR setup, such as adding additional behaviors or configuring options. By using this extension method, you can easily integrate MediatR with consistent exception handling across your application.
    /// </summary>
    /// <param name="services">The service collection to which MediatR services will be added.</param>
    /// <param name="configuration">A delegate that configures the MediatR services.</param>
    /// <param name="assemblies">The assemblies from which MediatR services will be registered.</param>
    /// <returns>The service collection with MediatR services and exception handling pipelines registered.</returns>
    public static IServiceCollection AddMediatorServicesAndPipelines(this IServiceCollection services, Action<MediatRServiceConfiguration> configuration,  params Assembly[] assemblies)
    {
        return services.AddMediatorServicesAndPipelines(configuration, true, assemblies);
    }
}
