using FluentValidation;
using IDFCR.Abstractions.Mediator.Extensions.Pipelines;
using IDFCR.Abstractions.Results;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace IDFCR.Abstractions.Mediator.Extensions.Extensions;

/// <summary>
/// Defines extension methods for configuring the IExceptionBehaviourManager and adding MediatR services with exception handling pipelines to the dependency injection container. These extensions provide a convenient way to set up exception handling behavior for MediatR requests, allowing developers to specify how exceptions should be handled and what actions should be taken when exceptions occur during request processing. By using these extension methods, developers can easily integrate consistent and customizable exception handling into their MediatR-based applications.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures the exception behavior manager to handle FluentValidation exceptions by setting a specific behavior for ValidationException. This extension method allows you to define how ValidationException should be handled within the exception behavior manager, specifying the unit action and failure reason associated with this type of exception. By calling this method, you can ensure that any ValidationException thrown during request processing is handled consistently according to the defined behavior.
    /// </summary>
    /// <param name="builder">The exception behavior manager builder.</param>
    /// <returns>The updated exception behavior manager builder.</returns>
    public static IExceptionBehaviourManagerBuilder SetFluentValidationBehaviours(this IExceptionBehaviourManagerBuilder builder)
    {
        return builder.Set<ValidationException>(new ExceptionBehaviour(UnitAction.Conflict, FailureReason.ValidationError));
    }

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
        buildConfiguration(builder);
        return services.AddSingleton(builder.Build());
    }

    /// <summary>
    /// Adds MediatR services to the specified service collection, along with a default exception handling pipeline. This method allows you to configure MediatR services and register them from the provided assemblies, while also ensuring that any exceptions thrown during request processing are handled by the DefaultExceptionPipeline. The configuration action can be used to further customize the MediatR setup, such as adding additional behaviors or configuring options. By using this extension method, you can easily integrate MediatR with consistent exception handling across your application.
    /// <para>Use this if you want to control the registration of MediatR services and exception handling pipelines.</para>
    /// </summary>
    /// <param name="services">The service collection to which MediatR services will be added.</param>
    /// <param name="configuration">A delegate that configures the MediatR services.</param>
    /// <param name="options">A boolean indicating whether to register services from the provided assemblies.</param>
    /// <param name="assemblies">The assemblies from which MediatR services will be registered.</param>
    /// <returns>The service collection with MediatR services and exception handling pipelines registered.</returns>
    public static IServiceCollection AddMediatorServicesAndPipelines(this IServiceCollection services, Action<MediatRServiceConfiguration>? configuration, MediatorServiceCollectionOptions options, params Assembly[] assemblies)
    {
        if (!services.Any(x => x.ServiceType == typeof(IExceptionBehaviourManager)))
        {
            throw new InvalidOperationException($"The exception behaviour manager is not registered. Please ensure that you have called {nameof(ConfigureExceptionBehaviourManager)} before adding MediatR services.");
        }

        services.TryAdd(ServiceDescriptor.Singleton(TimeProvider.System));

        return services
            .AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(GenericDefaultExceptionPipeline<,,>))
            .AddMediatR(cfg =>
            {
                if (options.UseUnitOfWorkPostPipelineProcessor)
                {
                    cfg.AddOpenRequestPostProcessor(typeof(UnitOfWorkPostPipelineProcessor<,>));
                }

                if (options.UseFluentValidationProcessor)
                {
                    cfg.AddOpenBehavior(typeof(ValidationPipeline<,>));
                }

                configuration?.Invoke(cfg);
                if (options.RegisterServicesFromAssemblies)
                {
                    cfg.RegisterServicesFromAssemblies(assemblies);
                }
            });
    }

    /// <summary>
    /// Adds MediatR services to the specified service collection, along with a default exception handling pipeline. This method allows you to configure MediatR services and register them from the provided assemblies, while also ensuring that any exceptions thrown during request processing are handled by the DefaultExceptionPipeline. The configuration action can be used to further customize the MediatR setup, such as adding additional behaviors or configuring options. By using this extension method, you can easily integrate MediatR with consistent exception handling across your application.
    /// </summary>
    /// <param name="services">The service collection to which MediatR services will be added.</param>
    /// <param name="configuration">A delegate that configures the MediatR services.</param>
    /// <param name="assemblies">The assemblies from which MediatR services will be registered.</param>
    /// <returns>The service collection with MediatR services and exception handling pipelines registered.</returns>
    public static IServiceCollection AddMediatorServicesAndPipelines(this IServiceCollection services, Action<MediatRServiceConfiguration>? configuration, params Assembly[] assemblies)
    {
        return services.AddMediatorServicesAndPipelines(configuration, new MediatorServiceCollectionOptions(), assemblies);
    }

    /// <summary>
    /// Adds MediatR services to the specified service collection, along with a default exception handling pipeline. This method allows you to configure MediatR services and register them from the provided assemblies, while also ensuring that any exceptions thrown during request processing are handled by the DefaultExceptionPipeline. The configuration action can be used to further customize the MediatR setup, such as adding additional behaviors or configuring options. By using this extension method, you can easily integrate MediatR with consistent exception handling across your application.
    /// </summary>
    /// <param name="services">The service collection to which MediatR services will be added.</param>
    /// <param name="assemblies">The assemblies from which MediatR services will be registered.</param>
    /// <returns>The service collection with MediatR services and exception handling pipelines registered.</returns>
    public static IServiceCollection AddMediatorServicesAndPipelines(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.AddMediatorServicesAndPipelines(null, assemblies);
    }
}