using IDFCR.Abstractions.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Cryptography.Extensions;

/// <summary>
/// Defines extension methods for registering cryptography services in the dependency injection container. This class provides a convenient way to add the necessary services for token payload protection and password-derived key generation to the service collection, allowing for easy integration of cryptographic functionality into applications that use dependency injection. By calling the AddCryptographyServices method, developers can ensure that the required services are registered and available for use throughout the application wherever they are needed.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the necessary cryptography services to the specified service collection. This method registers the ITokenPayloadProtector and IPasswordDerivedKeyGenerator interfaces with their respective default implementations, allowing them to be injected and used throughout the application. By calling this method, developers can easily set up the required services for token payload protection and password-derived key generation in their applications that utilize dependency injection.
    /// </summary>
    /// <param name="services">The service collection to which the cryptography services will be added.</param>
    /// <returns>The updated service collection with the cryptography services registered.</returns>
    public static IServiceCollection AddCryptographyServices(IServiceCollection services)
    {
        return services
            .AddSingleton<ITokenPayloadProtector, DefaultTokenPayloadProtector>()
            .AddSingleton<IPasswordDerivedKeyGenerator, DefaultPasswordDerivedKeyGenerator>();
    }
}
