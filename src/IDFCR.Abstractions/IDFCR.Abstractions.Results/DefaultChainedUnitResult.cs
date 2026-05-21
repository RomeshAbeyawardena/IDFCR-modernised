namespace IDFCR.Abstractions.Results;

internal record DefaultChainedUnitResult(IUnitResult Last, Exception? Exception = null, UnitAction Action = UnitAction.None, bool IsSuccess = false, FailureReason? FailureReason = FailureReason.None)
    : DefaultUnitResult(Exception, Action, IsSuccess, FailureReason), IChainedUnitResult
{
    public static IEnumerable<IUnitResult> Enumerate(IUnitResult target)
    {
        List<IUnitResult> items = [];
        Stack<IUnitResult> stack = [];
        stack.Push(target);

        while (stack.Count > 0)
        {
            var result = stack.Pop();

            if (result is IChainedUnitResult chained)
            {
                // Push in reverse so Current is processed before Last.
                stack.Push(chained.Last);
                stack.Push(chained.Current);
                continue;
            }

            items.Add(result);
        }

        return items;
    }

    /// <inheritdoc/>
    public string? Key { get; private set; }

    /// <inheritdoc/>
    public IChainedUnitResult WithKey(string key)
    {
        Key = key;
        return this;
    }

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

    public IUnitResult GetRoot()
    {
        return Enumerate().LastOrDefault() ?? this;
    }

    public IUnitResult GetFirstFailure()
    {
        return Enumerate().FirstOrDefault(r => !r.IsSuccess) ?? this;
    }

    public IEnumerable<IUnitResult> Enumerate()
    {
        return Enumerate(this);
    }

    public IUnitResult GetDeepest()
    {
        return Enumerate().FirstOrDefault() ?? this;
    }
}

internal record DefaultChainedUnitResult<T>(IUnitResult Last, T? Value = default, Exception? Exception = null, UnitAction Action = UnitAction.None, bool IsSuccess = false, FailureReason? FailureReason = FailureReason.None)
    : DefaultUnitResult<T>(Value, Action, IsSuccess, Exception, FailureReason), IChainedUnitResult<T>
{
    IUnitResult IChainedUnitResult.Current => Current;
    public IUnitResult<T> Current { get; } = null!;

    /// <inheritdoc/>
    public string? Key { get; private set; }

    /// <inheritdoc/>
    IChainedUnitResult IChainedUnitResult.WithKey(string key) => WithKey(key);

    public IChainedUnitResult<T> WithKey(string key)
    {
        Key = key;
        return this;
    }

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

    public IUnitResult GetRoot()
    {
        return Enumerate().LastOrDefault() ?? this;
    }

    public IUnitResult GetFirstFailure()
    {
        return Enumerate().FirstOrDefault(r => !r.IsSuccess) ?? this;
    }

    public IEnumerable<IUnitResult> Enumerate()
    {
        return DefaultChainedUnitResult.Enumerate(this);
    }

    public IUnitResult GetDeepest()
    {
        return Enumerate().FirstOrDefault() ?? this;
    }
}