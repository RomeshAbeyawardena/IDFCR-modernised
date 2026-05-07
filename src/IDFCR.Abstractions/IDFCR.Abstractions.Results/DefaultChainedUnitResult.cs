namespace IDFCR.Abstractions.Results;

internal static class ChainedUnitResultResolver
{
    public static bool ResolveIsSuccess(IUnitResult first, IUnitResult second, bool setAsFailWhenAnyUnitsFail)
        => setAsFailWhenAnyUnitsFail
            ? first.IsSuccess && second.IsSuccess
            : second.IsSuccess;

    public static Exception? ResolveException(IUnitResult first, IUnitResult second, bool setAsFailWhenAnyUnitsFail)
    {
        if (!setAsFailWhenAnyUnitsFail)
        {
            return second.Exception;
        }

        return first.Exception ?? second.Exception;
    }

    public static FailureReason? ResolveFailureReason(IUnitResult first, IUnitResult second, bool setAsFailWhenAnyUnitsFail)
    {
        if (!setAsFailWhenAnyUnitsFail)
        {
            return second.FailureReason;
        }

        return first.FailureReason ?? second.FailureReason;
    }
}

internal record DefaultChainedUnitResult(IUnitResult Last, Exception? Exception = null, UnitAction Action = UnitAction.None, bool IsSuccess = false, FailureReason? FailureReason = FailureReason.None)
    : DefaultUnitResult(Exception, Action, IsSuccess, FailureReason), IChainedUnitResult
{
    public IUnitResult Current { get; } = null!;

    public DefaultChainedUnitResult(IUnitResult currentResult, IUnitResult lastResult, bool setAsFailWhenAnyUnitsFail = true)
        : this(
            lastResult,
            ChainedUnitResultResolver.ResolveException(currentResult, lastResult, setAsFailWhenAnyUnitsFail),
            currentResult.Action,
            ChainedUnitResultResolver.ResolveIsSuccess(currentResult, lastResult, setAsFailWhenAnyUnitsFail),
            ChainedUnitResultResolver.ResolveFailureReason(currentResult, lastResult, setAsFailWhenAnyUnitsFail))
    {
        foreach (var (key, value) in lastResult.Meta)
        {
            AddMeta(key, value);
        }

        Current = currentResult;
    }
}

internal record DefaultChainedUnitResult<T>(IUnitResult Last, T? Value = default, Exception? Exception = null, UnitAction Action = UnitAction.None, bool IsSuccess = false, FailureReason? FailureReason = FailureReason.None)
    : DefaultUnitResult<T>(Value, Action, IsSuccess, Exception, FailureReason), IChainedUnitResult<T>
{
    IUnitResult IChainedUnitResult.Current => Current;
    public IUnitResult<T> Current { get; } = null!;

    public DefaultChainedUnitResult(IUnitResult<T> currentResult, IUnitResult lastResult, bool setAsFailWhenAnyUnitsFail = true)
        : this(
            lastResult,
            currentResult.Result,
            ChainedUnitResultResolver.ResolveException(currentResult, lastResult, setAsFailWhenAnyUnitsFail),
            currentResult.Action,
            ChainedUnitResultResolver.ResolveIsSuccess(currentResult, lastResult, setAsFailWhenAnyUnitsFail),
            ChainedUnitResultResolver.ResolveFailureReason(currentResult, lastResult, setAsFailWhenAnyUnitsFail))
    {
        foreach (var (key, value) in lastResult.Meta)
        {
            AddMeta(key, value);
        }

        Current = currentResult;
    }
}