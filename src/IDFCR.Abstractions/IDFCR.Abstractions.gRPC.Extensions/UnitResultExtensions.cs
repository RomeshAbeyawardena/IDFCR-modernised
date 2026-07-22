using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Results;
using V1Common = IDFCR.Abstractions.GRPC.Contracts.Common.V1;
namespace IDFCR.Abstractions.GRPC.Extensions;

/// <summary>
/// Defines extension methods for converting between gRPC contract types and common types related to unit results.
/// </summary>
public static class UnitResultExtensions
{
    /// <summary>
    /// Converts a gRPC contract StringListDelta to a common StringListDelta.
    /// </summary>
    /// <param name="delta">The gRPC contract StringListDelta to convert.</param>
    /// <returns>A common StringListDelta representing the converted delta.</returns>
    public static StringListDelta From(this V1Common.StringListDelta delta)
    {
        return new StringListDelta
        {
            Add = delta.Add,
            Remove = delta.Remove
        };
    }

    /// <summary>
    /// Converts a common StringListDelta to a gRPC contract StringListDelta.
    /// </summary>
    /// <param name="unitResult">The common StringListDelta to convert.</param>
    /// <returns>A gRPC contract StringListDelta representing the converted delta.</returns>
    public static V1Common.UnitResult From(this IUnitResult unitResult)
    {
        var result = new V1Common.UnitResult
        {
            Action = Enum.Parse<V1Common.UnitAction>(unitResult.Action.ToString()),
            IsSuccess = unitResult.IsSuccess,
            ErrorMessage = unitResult?.Exception?.Message ?? "",
        };

        if (unitResult!.FailureReason.HasValue)
        {
            result.FailureReason = Enum.Parse<V1Common.FailureReason>(unitResult.FailureReason.Value.ToString());
        }

        return result;
    }

    /// <summary>
    /// Defines an extension method to convert a Common.UnitResult from the common UnitResult type to the gRPC contract IUnitResult type.
    /// </summary>
    /// <param name="unitResult">The common UnitResult to convert.</param>
    /// <returns>An IUnitResult representing the converted unit result.</returns>
    public static IUnitResult From(this V1Common.UnitResult unitResult)
    {
        FailureReason? failureReason = unitResult.HasFailureReason
            ? Enum.Parse<FailureReason>(unitResult.FailureReason.ToString())
            : null;

        Exception? exception = string.IsNullOrWhiteSpace(unitResult.ErrorMessage)
            ? null
            : new Exception(unitResult.ErrorMessage);

        return UnitResult.Create(unitResult.IsSuccess, exception, 
            Enum.Parse<UnitAction>(unitResult.Action.ToString()), failureReason);
    }
}
