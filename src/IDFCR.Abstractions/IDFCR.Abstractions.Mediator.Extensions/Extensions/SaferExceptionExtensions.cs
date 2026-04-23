using IDFCR.Abstractions.Results;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.Mediator.Extensions.Extensions;

/// <summary>
/// Defines extension methods for configuring and registering a safer exception provider in the dependency injection container. These extensions provide a convenient way to set up a provider that can generate safer exceptions based on the configurations provided through the builder. By using these extension methods, developers can easily integrate a consistent and customizable approach to handling exceptions in their applications, allowing for better control over the information exposed in exceptions and improving overall application security and reliability.
/// </summary>
public static class SaferExceptionExtensions
{
    /// <summary>
    /// Configures and registers a safer exception provider as a singleton service in the dependency injection container. This method allows you to define custom mappings between exception types and safer exception representations, including safer messages, status codes, and failure reasons. By using the provided configuration action, you can customize how exceptions are transformed into safer exceptions, which can help improve security and user experience by controlling the information exposed in exceptions. Once configured, the safer exception provider will be available for use throughout the application wherever exception handling is needed.
    /// </summary>
    /// <param name="services">The service collection to which the safer exception provider will be added.</param>
    /// <param name="configureProvider">A delegate that configures the safer exception provider.</param>
    /// <returns>The service collection with the safer exception provider registered.</returns>
    public static IServiceCollection ConfigureSaferExceptions(IServiceCollection services, Action<ISaferExceptionProviderBuilder> configureProvider)
    {
        DefaultSaferExceptionProviderBuilder builder = new();

        configureProvider(builder);

        return services.AddSingleton(builder.Build());
    }
}