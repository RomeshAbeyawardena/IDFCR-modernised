namespace IDFCR.Abstractions.GRPC;

/// <summary>
/// Represents an attribute that can be applied to a gRPC service implementation class to indicate that it should be registered as a gRPC service in the dependency injection container. The attribute has two parameters: "enabled" which indicates whether the service should be registered, and "enabledValueConfigurationKey" which specifies a configuration key that can be used to enable or disable the service at runtime. If "enabled" is set to true, the service will be registered regardless of the configuration key. If "enabled" is set to false, the service will only be registered if the configuration key is present and set to true in the application's configuration.
/// </summary>
/// <param name="enabled">Indicates whether the service should be registered.</param>
/// <param name="enabledValueConfigurationKey">The configuration key that can be used to enable or disable the service at runtime.</param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RegisteredGRPCServiceImplementationAttribute(bool enabled, string? enabledValueConfigurationKey) : Attribute
{
    /// <summary>
    /// Gets a value indicating whether the service should be registered. If true, the service will be registered regardless of the configuration key. If false, the service will only be registered if the configuration key is present and set to true in the application's configuration.
    /// </summary>
    public bool Enabled { get; } = enabled;
    /// <summary>
    /// Gets the configuration key that can be used to enable or disable the service at runtime. If "Enabled" is set to true, this configuration key will be ignored. If "Enabled" is set to false, the service will only be registered if this configuration key is present and set to true in the application's configuration.
    /// </summary>
    public string? EnabledValueConfigurationKey { get; } = enabledValueConfigurationKey;
}
