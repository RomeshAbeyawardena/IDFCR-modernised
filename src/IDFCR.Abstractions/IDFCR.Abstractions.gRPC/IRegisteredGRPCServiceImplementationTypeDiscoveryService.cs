using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace IDFCR.Abstractions.GRPC;

/// <summary>
/// Represents a service that can discover types that are decorated with the RegisteredGRPCServiceImplementationAttribute. This service can be used to automatically register gRPC service implementations in the dependency injection container based on the presence of the RegisteredGRPCServiceImplementationAttribute on their classes. The DiscoverTypes method takes an array of assemblies to search for types and returns an enumerable of types that are decorated with the RegisteredGRPCServiceImplementationAttribute and meet the criteria specified by the attribute's parameters.
/// </summary>
public interface IRegisteredGRPCServiceImplementationTypeDiscoveryService
{
    /// <summary>
    /// Flushes the cache of discovered types. This method can be called to clear the cache and force the DiscoverTypes method to perform a fresh discovery of types the next time it is called. This can be useful if the assemblies being searched have changed or if the configuration has changed in a way that affects which types should be discovered.
    /// </summary>
    void FlushCache();
    /// <summary>
    /// Gets a cache of the types that have been discovered by the DiscoverTypes method. This property can be used to avoid redundant discovery of types if the same assemblies are searched multiple times. The DiscoverTypes method should update this property with the newly discovered types each time it is called.
    /// </summary>
    IEnumerable<Type> DiscoveredTypes { get; }
    /// <summary>
    /// Defines a method that discovers types that are decorated with the RegisteredGRPCServiceImplementationAttribute in the specified assemblies. The method should return an enumerable of types that are decorated with the RegisteredGRPCServiceImplementationAttribute and meet the criteria specified by the attribute's parameters. The method should also update the DiscoveredTypes property with the newly discovered types each time it is called. The configuration parameter can be used to evaluate the EnabledValueConfigurationKey for each type if necessary.
    /// </summary>
    /// <param name="configuration">The application's configuration, which can be used to evaluate the EnabledValueConfigurationKey for each type if necessary.</param>
    /// <param name="assemblies">The assemblies to search for types.</param>
    /// <returns>An enumerable of types that are decorated with the RegisteredGRPCServiceImplementationAttribute and meet the criteria specified by the attribute's parameters.</returns>
    IEnumerable<Type> DiscoverTypes(IConfiguration configuration, params Assembly[] assemblies);
}
