using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace IDFCR.Abstractions.GRPC;

/// <summary>
/// Defines a static class that provides a method for discovering types that are decorated with the RegisteredGRPCServiceImplementationAttribute. This class uses a lazy singleton instance of the RegisteredGRPCServiceImplementationTypeDiscoveryService to perform the discovery of types. The DiscoverTypes method takes an array of assemblies to search for types and returns an enumerable of types that are decorated with the RegisteredGRPCServiceImplementationAttribute and meet the criteria specified by the attribute's parameters. This class can be used to automatically register gRPC service implementations in the dependency injection container based on the presence of the RegisteredGRPCServiceImplementationAttribute on their classes, without needing to directly reference the RegisteredGRPCServiceImplementationTypeDiscoveryService class.
/// </summary>
public static class RegisteredGRPCServices
{
    private static readonly Lazy<IRegisteredGRPCServiceImplementationTypeDiscoveryService> registeredGRPCServiceImplementationTypeDiscoveryService = 
        new(new RegisteredGRPCServiceImplementationTypeDiscoveryService());

    /// <summary>
    /// Discovers types that are decorated with the RegisteredGRPCServiceImplementationAttribute in the specified assemblies. The method returns an enumerable of types that are decorated with the RegisteredGRPCServiceImplementationAttribute and meet the criteria specified by the attribute's parameters. The configuration parameter can be used to evaluate the EnabledValueConfigurationKey for each type if necessary. This method uses a lazy singleton instance of the RegisteredGRPCServiceImplementationTypeDiscoveryService to perform the discovery of types, so it will cache the results of the discovery and return the cached results on subsequent calls with the same assemblies, unless the FlushCache method is called to clear the cache. This allows for efficient discovery of types without redundant reflection operations on subsequent calls with the same assemblies. If you need to force a fresh discovery of types, you can call the DiscoverTypes method with flashCache set to true, which will flush the cache before performing the discovery.
    /// </summary>
    /// <param name="configuration">The application's configuration, which can be used to evaluate the EnabledValueConfigurationKey for each type if necessary.</param>
    /// <param name="assemblies">The assemblies to search for types.</param>
    /// <returns>An enumerable of types that are decorated with the RegisteredGRPCServiceImplementationAttribute and meet the criteria specified by the attribute's parameters.</returns>
    public static IEnumerable<Type> DiscoverTypes(IConfiguration configuration, params Assembly[] assemblies)
    {
        return DiscoverTypes(configuration, false, assemblies);
    }

    /// <summary>
    /// Discovers types that are decorated with the RegisteredGRPCServiceImplementationAttribute in the specified assemblies. The method returns an enumerable of types that are decorated with the RegisteredGRPCServiceImplementationAttribute and meet the criteria specified by the attribute's parameters. The configuration parameter can be used to evaluate the EnabledValueConfigurationKey for each type if necessary. This method uses a lazy singleton instance of the RegisteredGRPCServiceImplementationTypeDiscoveryService to perform the discovery of types, so it will cache the results of the discovery and return the cached results on subsequent calls with the same assemblies, unless the FlushCache method is called to clear the cache. This allows for efficient discovery of types without redundant reflection operations on subsequent calls with the same assemblies. If you need to force a fresh discovery of types, you can call the DiscoverTypes method with flashCache set to true, which will flush the cache before performing the discovery.
    /// </summary>
    /// <param name="configuration">The application's configuration, which can be used to evaluate the EnabledValueConfigurationKey for each type if necessary.</param>
    /// <param name="flashCache">A boolean value indicating whether to flush the cache of discovered types before performing the discovery. If true, the cache will be flushed and a fresh discovery of types will be performed. If false, the cached results will be returned if the same assemblies are searched again.</param>
    /// <param name="assemblies">The assemblies to search for types.</param>
    /// <returns>An enumerable of types that are decorated with the RegisteredGRPCServiceImplementationAttribute and meet the criteria specified by the attribute's parameters.</returns>
    public static IEnumerable<Type> DiscoverTypes(IConfiguration configuration, bool flashCache, params Assembly[] assemblies)
    {
        if(flashCache)
        {
            registeredGRPCServiceImplementationTypeDiscoveryService.Value.FlushCache();
        }

        return registeredGRPCServiceImplementationTypeDiscoveryService.Value.DiscoverTypes(configuration, assemblies);
    }
}
