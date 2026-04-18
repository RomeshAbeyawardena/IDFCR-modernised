using BuildTools.GRPC.Shared.Contracts.Common;
using System;
using Contracts = IDFCR.Abstractions.Results;

namespace BuildTools.GRPC.Application.Extensions;

public static class UnitResultExtensions
{
    public static UnitResult Map(this Contracts.IUnitResult result)
    {
        if (!Enum.TryParse<UnitAction>(result.Action.ToString(), false, out var unitAction))
        {
            unitAction = UnitAction.None;
        }

        if (!Enum.TryParse<FailureReason>(result.FailureReason.ToString(), false, out var failureReason))
        {
            failureReason = FailureReason.Unknown;
        }

        return new UnitResult()
        {
            IsSuccess = result.IsSuccess,
            Action = unitAction,
            ErrorMessage = result.Exception?.Message ?? string.Empty,
            FailureReason = failureReason
        };
    }
}
