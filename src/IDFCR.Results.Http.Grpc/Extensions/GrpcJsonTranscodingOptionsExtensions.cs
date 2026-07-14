using Google.Protobuf.Reflection;
using Microsoft.AspNetCore.Grpc.JsonTranscoding;
using V1Common = IDFCR.Abstractions.GRPC.Contracts.Common.V1;

namespace IDFCR.Results.Http.Grpc.Extensions;

/// <summary>
/// Defines extension methods for configuring gRPC JSON transcoding options.
/// </summary>
public static class GrpcJsonTranscodingOptionsExtensions
{
    /// <summary>
    /// Defines an extension method to configure the TypeRegistry for gRPC JSON transcoding options with the UnitResult type and any additional message descriptors.
    /// </summary>
    /// <param name="options">The gRPC JSON transcoding options to configure.</param>
    /// <param name="descriptors">Additional message descriptors to include in the TypeRegistry.</param>
    /// <returns>The configured gRPC JSON transcoding options.</returns>
    public static GrpcJsonTranscodingOptions ConfigureUnitResultTypeRegistries(this GrpcJsonTranscodingOptions options, params MessageDescriptor[] descriptors)
    {
        if (options.TypeRegistry is not null)
        {
            throw new InvalidOperationException("TypeRegistry is already configured. Please configure it only once.");
        }

        options.TypeRegistry = TypeRegistry.FromMessages(
           [V1Common.UnitResult.Descriptor, ..descriptors]);
        return options;
    }
}
