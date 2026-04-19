using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace IDFCR.Abstractions.GRPC;

/// <inheritdoc />
public class RegisteredGRPCServiceImplementationTypeDiscoveryService : IRegisteredGRPCServiceImplementationTypeDiscoveryService
{
    /// <inheritdoc />
    public IEnumerable<Type> DiscoveredTypes { get; private set; } = [];

    /// <inheritdoc />
    public IEnumerable<Type> DiscoverTypes(IConfiguration configuration, params Assembly[] assemblies)
    {
        if (DiscoveredTypes.Any())
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

        DiscoveredTypes = [.. serviceTypes];

        return DiscoveredTypes;
    }

    /// <inheritdoc />
    public void FlushCache()
    {
        DiscoveredTypes = [];
    }
}