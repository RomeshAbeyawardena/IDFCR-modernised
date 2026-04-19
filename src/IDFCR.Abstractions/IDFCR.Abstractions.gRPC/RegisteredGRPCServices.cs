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
    /// Flushes the cache of discovered types in the RegisteredGRPCServiceImplementationTypeDiscoveryService. This method can be called to clear the cache of discovered types, which will force a fresh discovery of types the next time the DiscoverTypes method is called. This can be useful if you want to ensure that any changes to the assemblies being searched or the configuration are reflected in the results of the DiscoverTypes method, without needing to restart the application or create a new instance of the RegisteredGRPCServiceImplementationTypeDiscoveryService class.
    /// </summary>
    public static void FlushCache()
    {
        registeredGRPCServiceImplementationTypeDiscoveryService.Value.FlushCache();
    }

    /// <summary>
    /// Gets a cache of the types that have been discovered by the DiscoverTypes method. This property can be used to avoid redundant discovery of types if the same assemblies are searched multiple times. The DiscoverTypes method should update this property with the newly discovered types each time it is called.
    /// <para>Will be empty if <see cref="DiscoverTypes(IConfiguration, Assembly[])"/> or <see cref="DiscoverTypes(IConfiguration, bool, Assembly[])"/> has not been called yet.</para>
    /// </summary>
    public static IEnumerable<Type> DiscoveredTypes => registeredGRPCServiceImplementationTypeDiscoveryService.Value.DiscoveredTypes;

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
    /// <param name="flushCache">A boolean value indicating whether to flush the cache of discovered types before performing the discovery. If true, the cache will be flushed and a fresh discovery of types will be performed. If false, the cached results will be returned if the same assemblies are searched again.</param>
    /// <param name="assemblies">The assemblies to search for types.</param>
    /// <returns>An enumerable of types that are decorated with the RegisteredGRPCServiceImplementationAttribute and meet the criteria specified by the attribute's parameters.</returns>
    public static IEnumerable<Type> DiscoverTypes(IConfiguration configuration, bool flushCache, params Assembly[] assemblies)
    {
        if(flushCache)
        {
            registeredGRPCServiceImplementationTypeDiscoveryService.Value.FlushCache();
        }

        return registeredGRPCServiceImplementationTypeDiscoveryService.Value.DiscoverTypes(configuration, assemblies);
    }
}
