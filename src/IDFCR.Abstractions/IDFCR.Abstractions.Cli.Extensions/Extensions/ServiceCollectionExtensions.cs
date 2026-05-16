using IDFCR.Abstractions.Cli.Dispatchers;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Cli.Prompts;
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
            .AddSingleton<IPromptGreeter, DefaultPromptGreeter>()
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
    /// Configures the options for the prompt greeter service by accepting a delegate that builds an instance of IPromptGreeterOptions using an IPromptGreeterOptionsBuilder. This method registers the configured options as a singleton service in the dependency injection container, allowing it to be injected into other services that depend on IPromptGreeterOptions. By calling this method, developers can easily customize the behavior of the prompt greeter service in their CLI applications by providing specific configurations for greeting messages and time-based greetings.
    /// <para>You can start with a <see cref="IPromptGreeterOptionsBuilder.UseDefault(PromptGreeterDefaults)"/> as a starting point and customise the options further using the builder methods.</para>
    /// </summary>
    /// <param name="services">The IServiceCollection to add the configured options to.</param>
    /// <param name="buildConfig">A delegate that builds an instance of IPromptGreeterOptions using an IPromptGreeterOptionsBuilder.</param>
    /// <returns>The IServiceCollection with the configured prompt greeter options added.</returns>
    public static IServiceCollection ConfigurePromptGreeterOptions(this IServiceCollection services, Func<IPromptGreeterOptionsBuilder, IPromptGreeterOptions> buildConfig)
    {
        return services.AddSingleton(buildConfig(new DefaultPromptGreeterOptionsBuilder()));
    }

    /// <summary>
    /// Adds services related to injectable command operations in a CLI application by scanning the specified assemblies for classes that implement the IInjectableCommandOperation interface. This method registers the base command services and feature-specific command services with transient lifetimes, allowing them to be resolved using service keys based on the FeatureCommandAttribute. By calling this method, developers can easily set up dependency injection for their CLI applications and manage command operations in a modular and extensible way.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the injectable command services to.</param>
    /// <param name="assemblies">The assemblies to scan for injectable command services.</param>
    /// <returns>The IServiceCollection with the injectable command services added.</returns>
    public static IServiceCollection AddInjectableCommandServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.AddBaseCommandServices()
            .AddFeatureCommandServices(assemblies);
    }

    /// <summary>
    /// Adds a standard console managed stream service to the dependency injection container, allowing for interaction with the console's standard input, output, and error streams. This method registers a singleton instance of the ConsoleStream class, which provides access to the console streams in a structured manner. By calling this method, developers can easily integrate console stream functionality into their CLI applications and interact with the console for input and output operations using the IManagedStream interface.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the managed stream service to.</param>
    /// <returns>The IServiceCollection with the managed stream service added.</returns>
    public static IServiceCollection AddStandardConsoleManagedStream(this IServiceCollection services)
    {
        return services.AddSingleton(ConsoleStream.Std);
    }

    /// <summary>
    /// Adds a logger-derived managed stream service to the dependency injection container, allowing for logging output to be captured and written to the appropriate log levels. This method registers an implementation of IManagedStream that uses an ILogger to write output to the console or other logging targets based on the configured log levels. By calling this method, developers can easily integrate logging functionality into their CLI applications and capture output in a structured manner using the ILogger interface.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the managed stream service to.</param>
    /// <returns>The IServiceCollection with the managed stream service added.</returns>
    public static IServiceCollection AddLoggerDerivedManagedStream(this IServiceCollection services)
    {
        return services.AddSingleton<IManagedStream, LoggerDerivedManagedStream>();
    }
}
