namespace IDFCR.Abstractions.GRPC;

/// <summary>
/// Marks a concrete gRPC service implementation as discoverable for automatic endpoint registration.
/// <para>
/// Discovery uses a hard-stop rule:
/// if <paramref name="enabled" /> is <see langword="false" />, the type is never discovered,
/// regardless of configuration.
/// </para>
/// <para>
/// If <paramref name="enabled" /> is <see langword="true" />, an optional
/// <paramref name="enabledValueConfigurationKey" /> can be used to turn discovery off when the
/// resolved configuration value is explicitly <c>false</c>.
/// </para>
/// </summary>
/// <param name="enabled">
/// Hard-stop switch for discovery. When <see langword="false" />, discovery is always blocked.
/// </param>
/// <param name="enabledValueConfigurationKey">
/// Optional configuration key evaluated only when <paramref name="enabled" /> is <see langword="true" />.
/// If the key resolves to <c>false</c>, discovery is blocked.
/// </param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RegisteredGRPCServiceImplementationAttribute(bool enabled, string? enabledValueConfigurationKey = null) : Attribute
{
    /// <summary>
    /// Gets the hard-stop discovery flag.
    /// <para>
    /// <see langword="false" /> means the service is never discovered, and configuration cannot re-enable it.
    /// </para>
    /// </summary>
    public bool Enabled { get; } = enabled;

    /// <summary>
    /// Gets the optional configuration key used to conditionally disable discovery when
    /// <see cref="Enabled" /> is <see langword="true" />.
    /// </summary>
    public string? EnabledValueConfigurationKey { get; } = enabledValueConfigurationKey;
}
