namespace IDFCR.Abstractions.Results;

internal static class ChainedUnitResultResolver
{
    public static bool ResolveIsSuccess(IUnitResult first, IUnitResult second, bool setAsFailWhenAnyUnitsFail)
        => setAsFailWhenAnyUnitsFail
            ? first.IsSuccess && second.IsSuccess
            : first.IsSuccess;

    public static Exception? ResolveException(IUnitResult first, IUnitResult second, bool setAsFailWhenAnyUnitsFail)
    {
        if (!setAsFailWhenAnyUnitsFail)
        {
            return first.Exception;
        }

        if (!first.IsSuccess) return first.Exception;
        if (!second.IsSuccess) return second.Exception;
        return first.Exception ?? second.Exception;
    }

    public static FailureReason? ResolveFailureReason(IUnitResult first, IUnitResult second, bool setAsFailWhenAnyUnitsFail)
    {
        if (!setAsFailWhenAnyUnitsFail)
        {
            return first.FailureReason;
        }

        if (!first.IsSuccess) return first.FailureReason;
        if (!second.IsSuccess) return second.FailureReason;
        return first.FailureReason ?? second.FailureReason;
    }
}

internal record DefaultChainedUnitResult(IUnitResult Last, Exception? Exception = null, UnitAction Action = UnitAction.None, bool IsSuccess = false, FailureReason? FailureReason = FailureReason.None)
    : DefaultUnitResult(Exception, Action, IsSuccess, FailureReason), IChainedUnitResult
{
    public IUnitResult Current { get; } = null!;

    public DefaultChainedUnitResult(IUnitResult unitResult, IUnitResult secondUnitResult, bool setAsFailWhenAnyUnitsFail = true)
        : this(
            secondUnitResult,
            ChainedUnitResultResolver.ResolveException(unitResult, secondUnitResult, setAsFailWhenAnyUnitsFail),
            unitResult.Action,
            ChainedUnitResultResolver.ResolveIsSuccess(unitResult, secondUnitResult, setAsFailWhenAnyUnitsFail),
            ChainedUnitResultResolver.ResolveFailureReason(unitResult, secondUnitResult, setAsFailWhenAnyUnitsFail))
    {
        foreach (var (key, value) in unitResult.Meta)
        {
            AddMeta(key, value);
        }

        Current = unitResult;
    }
}

internal record DefaultChainedUnitResult<T>(IUnitResult Last, T? Value = default, Exception? Exception = null, UnitAction Action = UnitAction.None, bool IsSuccess = false, FailureReason? FailureReason = FailureReason.None)
    : DefaultUnitResult<T>(Value, Action, IsSuccess, Exception, FailureReason), IChainedUnitResult<T>
{
    IUnitResult IChainedUnitResult.Current => Current;
    public IUnitResult<T> Current { get; } = null!;

    public DefaultChainedUnitResult(IUnitResult<T> unitResult, IUnitResult secondUnitResult, bool setAsFailWhenAnyUnitsFail = true)
        : this(
            secondUnitResult,
            unitResult.Result,
            ChainedUnitResultResolver.ResolveException(unitResult, secondUnitResult, setAsFailWhenAnyUnitsFail),
            unitResult.Action,
            ChainedUnitResultResolver.ResolveIsSuccess(unitResult, secondUnitResult, setAsFailWhenAnyUnitsFail),
            ChainedUnitResultResolver.ResolveFailureReason(unitResult, secondUnitResult, setAsFailWhenAnyUnitsFail))
    {
        foreach (var (key, value) in unitResult.Meta)
        {
            AddMeta(key, value);
        }

        Current = unitResult;
    }
}