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
    /// Adds gRPC client types to the service collection with the specified configuration.
    /// </summary>
    /// <param name="services">The service collection to add the gRPC client types to.</param>
    /// <param name="configureClient">An action to configure the gRPC client options.</param>
    /// <param name="types">The gRPC client types to add.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddGrpcClientTypes(this IServiceCollection services, Action<GrpcClientFactoryOptions> configureClient, params Type[] types)
    {
        var targetMethod = typeof(GrpcClientServiceExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .FirstOrDefault(m => m.Name.StartsWith(nameof(GrpcClientServiceExtensions.AddGrpcClient)) && m.GetParameters().Length == 2);

        foreach (var type in types)
        {
            var method = targetMethod?.MakeGenericMethod(type);
            IHttpClientBuilder? clientBuilder = null;
            method?.Invoke(clientBuilder, [services, configureClient]);
        }

        return services;
    }
}
