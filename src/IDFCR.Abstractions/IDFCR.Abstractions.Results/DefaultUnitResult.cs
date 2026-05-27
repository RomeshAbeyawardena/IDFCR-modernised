namespace IDFCR.Abstractions.Results;

internal record DefaultUnitResult(Exception? Exception = null, UnitAction Action = UnitAction.None,
    bool IsSuccess = false, FailureReason? FailureReason = null, FailureOrigin? FailureOrigin = null)
    : UnitResultBase(Exception, Action, IsSuccess, FailureReason, FailureOrigin)
{
}

internal record DefaultUnitResult<TResult>(TResult? Result = default, UnitAction Action = UnitAction.None,
    bool IsSuccess = true, Exception? Exception = null, FailureReason? FailureReason = null, string? NamedResult = null, FailureOrigin? FailureOrigin = null)
    : UnitResultBase<TResult>(Result, Action, IsSuccess, Exception, FailureReason, NamedResult, FailureOrigin)
{

}