using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace IDFCR.Abstractions.GRPC;

/// <summary>
/// Discovers concrete types decorated with <see cref="RegisteredGRPCServiceImplementationAttribute" />.
/// <para>
/// Discovery rules:
/// </para>
/// <list type="bullet">
/// <item><description>Type must be a non-abstract class.</description></item>
/// <item><description>Type must have <see cref="RegisteredGRPCServiceImplementationAttribute" />.</description></item>
/// <item><description>
/// If attribute <c>Enabled</c> is <see langword="false" />, discovery is blocked (hard stop).
/// </description></item>
/// <item><description>
/// If attribute <c>Enabled</c> is <see langword="true" /> and a configuration key is provided,
/// an explicit configuration value of <c>false</c> blocks discovery.
/// </description></item>
/// </list>
/// </summary>
public interface IRegisteredGRPCServiceImplementationTypeDiscoveryService
{
    /// <summary>
    /// Clears the discovery cache so the next call to <see cref="DiscoverTypes(IConfiguration, Assembly[])" />
    /// performs a fresh scan.
    /// </summary>
    void FlushCache();

    /// <summary>
    /// Gets cached discovered types from the most recent discovery operation.
    /// </summary>
    IReadOnlyList<Type> DiscoveredTypes { get; }

    /// <summary>
    /// Scans the provided assemblies and returns types that satisfy the discovery rules.
    /// Also updates <see cref="DiscoveredTypes" />.
    /// </summary>
    /// <param name="configuration">
    /// Configuration used for optional per-type disable checks via
    /// <c>EnabledValueConfigurationKey</c>.
    /// </param>
    /// <param name="assemblies">Assemblies to scan.</param>
    /// <returns>Types eligible for gRPC registration.</returns>
    IReadOnlyList<Type> DiscoverTypes(IConfiguration configuration, params Assembly[] assemblies);
}
