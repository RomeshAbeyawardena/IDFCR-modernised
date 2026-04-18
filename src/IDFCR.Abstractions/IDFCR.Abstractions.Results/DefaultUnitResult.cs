namespace IDFCR.Abstractions.Results;

internal record DefaultUnitResult(Exception? Exception = null, UnitAction Action = UnitAction.None,
    bool IsSuccess = false, FailureReason? FailureReason = null)
    : UnitResultBase(Exception, Action, IsSuccess, FailureReason)
{
}

internal record DefaultUnitResult<TResult>(TResult? Result = default, UnitAction Action = UnitAction.None,
    bool IsSuccess = true, Exception? Exception = null, FailureReason? FailureReason = null)
    : UnitResultBase<TResult>(Result, Action, IsSuccess, Exception, FailureReason)
{

}