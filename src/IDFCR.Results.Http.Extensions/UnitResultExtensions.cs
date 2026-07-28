using IDFCR.Abstractions.Results;

namespace IDFCR.Results.Http.Extensions;

/// <summary>
/// Defines extension methods for converting gRPC UnitResult messages into IUnitResult instances. These extension methods allow you to easily convert gRPC UnitResult messages into IUnitResult instances that can be used within your application, providing a convenient way to handle the conversion between gRPC messages and the corresponding result objects.
/// </summary>
public static class UnitResultExtensions
{
    /// <summary>
    /// Converts a gRPC UnitResult message into an IUnitResult instance.
    /// </summary>
    /// <param name="result">The gRPC UnitResult message to convert.</param>
    /// <returns>An IUnitResult instance representing the converted result.</returns>
    public static IUnitResult From (this Abstractions.GRPC.Contracts.Common.V1.UnitResult result)
    {
        return UnitResult.Create(result.IsSuccess,
            new Exception(result.ErrorMessage),
            Enum.Parse<UnitAction>(result.Action.ToString()),
            Enum.TryParse<FailureReason>(result.FailureReason.ToString(), out var failureReason) ? failureReason : FailureReason.None);
    }
}
