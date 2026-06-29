using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IDFCR.GRPC.Client.Extensions;

/// <summary>
/// Defines extension methods for IServiceCollection to add gRPC client types.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds gRPC client types to the service collection with service discovery enabled.
    /// </summary>
    /// <param name="services">The service collection to add the gRPC client types to.</param>
    /// <param name="configureClient">An action to configure the gRPC client options.</param>
    /// <param name="types">The gRPC client types to add.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddGrpcClientTypesWithServiceDiscovery(this IServiceCollection services, Action<GrpcClientFactoryOptions> configureClient, params Type[] types)
    {
        return services.AddGrpcClientTypes(configureClient, true, types);
    }

    /// <summary>
    /// Adds gRPC client types to the service collection with the specified configuration.
    /// </summary>
    /// <param name="services">The service collection to add the gRPC client types to.</param>
    /// <param name="configureClient">An action to configure the gRPC client options.</param>
    /// <param name="types">The gRPC client types to add.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddGrpcClientTypes(this IServiceCollection services, Action<GrpcClientFactoryOptions> configureClient, params Type[] types)
    {
        return services.AddGrpcClientTypes(configureClient, false, types);
    }

    /// <summary>
    /// Adds gRPC client types to the service collection with the specified configuration.
    /// </summary>
    /// <param name="services">The service collection to add the gRPC client types to.</param>
    /// <param name="configureClient">An action to configure the gRPC client options.</param>
    /// <param name="addServiceDiscovery">Indicates whether to add service discovery for the gRPC clients.</param>
    /// <param name="types">The gRPC client types to add.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddGrpcClientTypes(this IServiceCollection services, Action<GrpcClientFactoryOptions> configureClient, bool addServiceDiscovery, params Type[] types)
    {
        var targetMethod = typeof(GrpcClientServiceExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .FirstOrDefault(m => {
                var parameters = m.GetParameters();
                return m.Name.StartsWith(nameof(GrpcClientServiceExtensions.AddGrpcClient))
                    && parameters.Length == 2
                    && parameters[1].ParameterType.GenericTypeArguments[0] == typeof(GrpcClientFactoryOptions);
            }) ?? throw new InvalidOperationException("Could not find the target AddGrpcClient extension method via reflection.");

        foreach (var type in types)
        {
            var method = targetMethod?.MakeGenericMethod(type);
            var builder = method?.Invoke(null, [services, configureClient]) as IHttpClientBuilder;

            if (addServiceDiscovery)
            {
                builder?.AddServiceDiscovery();
            }
        }

        return services;
    }
}
