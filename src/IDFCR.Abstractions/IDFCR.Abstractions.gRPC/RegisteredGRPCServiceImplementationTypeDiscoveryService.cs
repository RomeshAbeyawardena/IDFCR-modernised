using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace IDFCR.Abstractions.GRPC;

/// <inheritdoc cref="IRegisteredGRPCServiceImplementationTypeDiscoveryService" />
public sealed class RegisteredGRPCServiceImplementationTypeDiscoveryService : IRegisteredGRPCServiceImplementationTypeDiscoveryService
{
    private Type[] _discoveredTypes = [];
    /// <inheritdoc />
    public IReadOnlyList<Type> DiscoveredTypes => _discoveredTypes;

    /// <inheritdoc />
    public IReadOnlyList<Type> DiscoverTypes(IConfiguration configuration, params Assembly[] assemblies)
    {
        if (_discoveredTypes.Length > 0)
        {
            return DiscoveredTypes;
        }

        var serviceTypes = assemblies.SelectMany(a => a.DefinedTypes)
          .Where(t => {
              if(t.IsClass && !t.IsAbstract)
              {
                  var implementationAttribute = t.GetCustomAttribute<RegisteredGRPCServiceImplementationAttribute>();
                  if(implementationAttribute is null)
                  {
                      return false;
                  }

                  if (!implementationAttribute.Enabled)
                  {
                      return false;
                  }

                  if (!string.IsNullOrWhiteSpace(implementationAttribute.EnabledValueConfigurationKey))
                  {
                      var section = configuration.GetSection(implementationAttribute.EnabledValueConfigurationKey);
                      if (section is not null && bool.TryParse(section.Value, out var isEnabled) && !isEnabled)
                      {
                          return false;
                      }
                  }

                  return true;
              }
              return false;
            });

        _discoveredTypes = [.. serviceTypes];

        return DiscoveredTypes;
    }

    /// <inheritdoc />
    public void FlushCache()
    {
        _discoveredTypes = [];
    }
}