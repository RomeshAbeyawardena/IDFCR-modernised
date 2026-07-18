using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IDFCR.Abstractions.GRPC.Extensions;
using IDFCR.Abstractions.Results;

namespace IDFCR.Results.Http.Grpc.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="ServerCallContext"/> class, which is part of the gRPC framework. These extension methods provide functionality to set the status of a gRPC call based on the result of an operation represented by an <see cref="IUnitResult"/>. This allows for seamless integration of unit results with gRPC responses, enabling developers to easily map operation outcomes to appropriate gRPC status codes and messages.
/// </summary>
public static class ServerCallContextExtensions
{
    /// <summary>
    /// Converts an <see cref="IUnitResult"/> into a gRPC <see cref="Google.Rpc.Status"/> object. This method evaluates the result of an operation and constructs a corresponding gRPC status that reflects the outcome. If the operation was successful, it returns a status code based on the action performed; if it failed, it includes details about the failure, such as exception messages or fallback details. This allows for clear communication of operation results to gRPC clients.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="fallbackDetails"></param>
    /// <returns></returns>
    public static Google.Rpc.Status ToUnitResultStatus(this IUnitResult result, string? fallbackDetails)
    {
        var statusCode = GetStatusCode(result) ?? StatusCode.Unknown;
        var detail = result.Exception?.Message ?? fallbackDetails ?? string.Empty;
        return new Google.Rpc.Status
        {
            Code = (int)statusCode,
            Message = detail,
            Details =
            {
                Any.Pack(result.From(),
                "https://schema.idfcr.net/common/")
            }
        };
    }

    /// <summary>
    /// Maps the result of an operation represented by an <see cref="IUnitResult"/> to a corresponding gRPC <see cref="StatusCode"/>. This method evaluates the success or failure of the operation and returns the appropriate gRPC status code that reflects the outcome. If the operation was successful, it returns a status code based on the action performed; if it failed, it delegates to <see cref="GetFailedStatusCode(IUnitResult)"/> to determine the appropriate failure status code.
    /// </summary>
    /// <param name="result">The result of the operation.</param>
    /// <returns>The corresponding gRPC status code.</returns>
    public static StatusCode? GetStatusCode(IUnitResult result)
    {
        if (!result.IsSuccess)
        {
            return GetFailedStatusCode(result);
        }

        return StatusCode.OK;
    }

    /// <summary>
    /// Maps the <see cref="FailureReason"/> of an <see cref="IUnitResult"/> to a corresponding gRPC <see cref="StatusCode"/>. This method provides a way to translate the outcome of an operation into a gRPC status code that can be returned to the client, ensuring that the client receives meaningful feedback about the result of their request.
    /// </summary>
    /// <param name="result">The result of the operation.</param>
    /// <returns>The corresponding gRPC status code.</returns>
    public static StatusCode? GetFailedStatusCode(IUnitResult result)
    {
        return result.FailureReason switch
        {
            FailureReason.AuthorizationError => StatusCode.Unauthenticated,
            FailureReason.Conflict => StatusCode.AlreadyExists,
            FailureReason.ExternalDependencyError => StatusCode.FailedPrecondition,
            FailureReason.Forbidden => StatusCode.PermissionDenied,
            FailureReason.InternalError => StatusCode.Internal,
            FailureReason.NotFound => StatusCode.NotFound,
            FailureReason.Unauthorized => StatusCode.Unauthenticated,
            FailureReason.ValidationError => StatusCode.InvalidArgument,
            _ => null
        };
    }

    /// <summary>
    /// Sets the status of a gRPC call based on the provided <see cref="IUnitResult"/>. This method updates the <see cref="ServerCallContext.Status"/> property with a new <see cref="Status"/> object that reflects the outcome of the operation. If the result contains an exception, its message is used as the status detail; otherwise, a fallback detail can be provided. This allows for clear communication of operation results to gRPC clients.
    /// <para>May throw <see cref="RpcException"/> if the operation result indicates a failure.</para>
    /// </summary>
    /// <param name="context">The gRPC server call context.</param>
    /// <param name="result">The result of the operation.</param>
    /// <param name="fallbackdetails">The fallback details to use if the result does not contain an exception message.</param>
    /// <param name="fallbackStatusCode">The fallback gRPC status code to use if the result does not map to a specific status code.</param>
    public static void SetStatus(this ServerCallContext context, 
        IUnitResult result, 
        string? fallbackdetails = null, 
        StatusCode fallbackStatusCode = StatusCode.Unknown)
    {
        if (!result.IsSuccess)
        {
            throw result.ToUnitResultStatus(fallbackdetails).ToRpcException();
        }

        context.Status = new Status(GetStatusCode(result).GetValueOrDefault(fallbackStatusCode), string.Empty);
    }
}
