using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.gRPC.Extensions;

/// <summary>
/// Defines extension methods for converting between gRPC contract types and common types related to unit results.
/// </summary>
public static class UnitResultExtensions
{
    /// <summary>
    /// Defines an extension method to convert an IUnitResult from the gRPC contract type to the common UnitResult type.
    /// </summary>
    /// <param name="unitResult"></param>
    /// <returns></returns>
    public static GRPC.Contracts.Common.UnitResult From(IUnitResult unitResult)
    {
        var result = new GRPC.Contracts.Common.UnitResult
        {
            Action = Enum.Parse<GRPC.Contracts.Common.UnitAction>(unitResult.Action.ToString()),
            IsSuccess = unitResult.IsSuccess,
            ErrorMessage = unitResult?.Exception?.Message ?? ""
        };

        if (unitResult!.FailureReason.HasValue)
        {
            result.FailureReason = Enum.Parse<GRPC.Contracts.Common.FailureReason>(unitResult.FailureReason.Value.ToString());
        }

        return result;
    }

    /// <summary>
    /// Defines an extension method to convert a Common.UnitResult from the common UnitResult type to the gRPC contract IUnitResult type.
    /// </summary>
    /// <param name="unitResult">The common UnitResult to convert.</param>
    /// <returns>An IUnitResult representing the converted unit result.</returns>
    public static IUnitResult From(this GRPC.Contracts.Common.UnitResult unitResult)
    {
        return UnitResult.Create(unitResult.IsSuccess, new Exception(unitResult.ErrorMessage), 
            Enum.Parse<UnitAction>(unitResult.Action.ToString()));
    }
}
