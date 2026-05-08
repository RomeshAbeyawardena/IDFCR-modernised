namespace IDFCR.Abstractions.Results;

internal static class ChainedUnitResultResolver
{
    public static bool ResolveIsSuccess(IUnitResult current, IUnitResult last, bool setAsFailWhenAnyUnitsFail)
        => setAsFailWhenAnyUnitsFail
            ? current.IsSuccess && last.IsSuccess
            : current.IsSuccess;

    public static Exception? ResolveException(IUnitResult current, IUnitResult last, bool setAsFailWhenAnyUnitsFail)
    {
        if (!setAsFailWhenAnyUnitsFail)
        {
            return current.Exception;
        }

        return current.Exception ?? last.Exception;
    }

    public static FailureReason? ResolveFailureReason(IUnitResult current, IUnitResult last, bool setAsFailWhenAnyUnitsFail)
    {
        if (!setAsFailWhenAnyUnitsFail)
        {
            return current.FailureReason;
        }

        return current.FailureReason ?? last.FailureReason;
    }
}
