using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace IDFCR.Abstractions.GRPC.HostExtensions;

/// <summary>
/// Defines a static class that provides extension methods for the WebApplication class to discover and register gRPC services that are decorated with the RegisteredGRPCServiceImplementationAttribute. The DiscoverGRPCServices method takes an array of assemblies to search for types and returns the WebApplication instance for chaining. The method uses the RegisteredGRPCServices class to discover the types that are decorated with the RegisteredGRPCServiceImplementationAttribute and meet the criteria specified by the attribute's parameters, and then uses reflection to invoke the MapGrpcService method for each discovered type to register it as a gRPC service in the application's request pipeline. This allows for automatic registration of gRPC service implementations based on the presence of the RegisteredGRPCServiceImplementationAttribute on their classes, without needing to directly reference the MapGrpcService method or manually register each service implementation in the application's request pipeline.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Discovers and registers gRPC services that are decorated with the RegisteredGRPCServiceImplementationAttribute in the specified assemblies. The method returns the WebApplication instance for chaining. The method uses the RegisteredGRPCServices class to discover the types that are decorated with the RegisteredGRPCServiceImplementationAttribute and meet the criteria specified by the attribute's parameters, and then uses reflection to invoke the MapGrpcService method for each discovered type to register it as a gRPC service in the application's request pipeline. This allows for automatic registration of gRPC service implementations based on the presence of the RegisteredGRPCServiceImplementationAttribute on their classes, without needing to directly reference the <see cref="GrpcEndpointRouteBuilderExtensions.MapGrpcService{TService}(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder)"/> MapGrpcService method or manually register each service implementation in the application's request pipeline.
    /// </summary>
    /// <param name="app">The WebApplication instance to register the gRPC services with.</param>
    /// <param name="configuration">The application's configuration, which can be used to evaluate the EnabledValueConfigurationKey for each type if necessary.</param>
    /// <param name="assemblies">The assemblies to search for types.</param>
    /// <returns>The WebApplication instance for chaining.</returns>
    public static WebApplication DiscoverGRPCServices(this WebApplication app, IConfiguration configuration, params Assembly[] assemblies)
    {
        return app.DiscoverGRPCServices(configuration, false, assemblies);
    }

    /// <summary>
    /// Discovers and registers gRPC services that are decorated with the RegisteredGRPCServiceImplementationAttribute in the specified assemblies, with an option to flush the cache of discovered types. The method returns the WebApplication instance for chaining. The method uses the RegisteredGRPCServices class to discover the types that are decorated with the RegisteredGRPCServiceImplementationAttribute and meet the criteria specified by the attribute's parameters, and then uses reflection to invoke the MapGrpcService method for each discovered type to register it as a gRPC service in the application's request pipeline. This allows for automatic registration of gRPC service implementations based on the presence of the RegisteredGRPCServiceImplementationAttribute on their classes, without needing to directly reference the <see cref="GrpcEndpointRouteBuilderExtensions.MapGrpcService{TService}(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder)"/> method or manually register each service implementation in the application's request pipeline.
    /// </summary>
    /// <param name="app">The WebApplication instance to register the gRPC services with.</param>
    /// <param name="configuration">The application's configuration, which can be used to evaluate the EnabledValueConfigurationKey for each type if necessary.</param>
    /// <param name="flushCache">A boolean value indicating whether to flush the cache of discovered types before performing the discovery. If true, the cache will be flushed and a fresh discovery of types will be performed. If false, the cached results will be returned if the same assemblies are searched again.</param>
    /// <param name="assemblies">The assemblies to search for types.</param>
    /// <returns>The WebApplication instance for chaining.</returns>
    public static WebApplication DiscoverGRPCServices(this WebApplication app, IConfiguration configuration, bool flushCache, params Assembly[] assemblies)
    {
        var services = RegisteredGRPCServices.DiscoverTypes(configuration, flushCache, assemblies);

        var method = typeof(GrpcEndpointRouteBuilderExtensions)
        .GetMethods()
        .FirstOrDefault(x => x.IsGenericMethod && x.Name.StartsWith(nameof(GrpcEndpointRouteBuilderExtensions.MapGrpcService)));

        foreach (var type in services)
        {
            var genericMethod = method?.MakeGenericMethod(type);
            genericMethod?.Invoke(null, [app]);
        }

        return app;
    }
}
