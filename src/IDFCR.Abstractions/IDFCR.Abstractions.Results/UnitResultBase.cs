using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Abstractions.Results;

/// <summary>
/// Base type for unit results that carry metadata.
/// </summary>
/// <param name="Exception">The captured exception.</param>
/// <param name="Action">The associated action.</param>
/// <param name="IsSuccess">A value indicating whether the operation succeeded.</param>
/// <param name="FailureReason">The failure reason.</param>
public abstract record UnitResultBase(Exception? Exception = null, UnitAction Action = UnitAction.None,
    bool IsSuccess = false, FailureReason? FailureReason = null) : IUnitResult
{
    private readonly ConcurrentDictionary<string, object?> _metaProperties = [];
    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?> Meta => _metaProperties;

    /// <inheritdoc />
    public object? this[string key] { get => _metaProperties[key]; }

    /// <inheritdoc />
    public IUnitResult AddMeta(string key, object? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        _metaProperties.AddOrUpdate(key, value, (_, _) => value);
        return this;
    }

    /// <inheritdoc />
    public virtual IUnitResult<T> As<T>(T? value) => new DefaultUnitResult<T>(value, Action, IsSuccess, Exception);

    /// <inheritdoc />
    public virtual IUnitResultCollection<T> AsCollection<T>(IEnumerable<T>? value) => new UnitResultCollection<T>(value, Action, IsSuccess, Exception);

    /// <inheritdoc />
    public virtual bool TrySetState(object value) => false;
}

/// <summary>
/// Base type for typed unit results.
/// </summary>
/// <typeparam name="TResult">The result value type.</typeparam>
/// <param name="OriginalState">The result value.</param>
/// <param name="Action">The associated action.</param>
/// <param name="IsSuccess">A value indicating whether the operation succeeded.</param>
/// <param name="Exception">The captured exception.</param>
/// <param name="FailureReason">The failure reason.</param>
/// <param name="NamedResult">The name of the result.</param>
public abstract record UnitResultBase<TResult>(TResult? OriginalState = default, UnitAction Action = UnitAction.None,
    bool IsSuccess = true, Exception? Exception = null, FailureReason? FailureReason = null, string? NamedResult = null)
    : UnitResultBase(Exception, Action, IsSuccess, FailureReason), IUnitResult<TResult>
{
    /// <inheritdoc />
    public TResult? ModifiedState { get; private set; }

    /// <inheritdoc />
    public TResult? Result => ModifiedState ?? OriginalState;

    /// <inheritdoc />
    public override bool TrySetState(object value)
    {
        if (value is TResult result)
        {
            ModifiedState = result;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override IUnitResult<T> As<T>(T? value) where T : default
    {
        if (Result is not T result)
        {
            throw new InvalidCastException($"Unable to cast result of type {typeof(TResult)} to {typeof(T)}");
        }

        return base.As(value ?? result);
    }

    /// <inheritdoc />
    public override IUnitResultCollection<T> AsCollection<T>(IEnumerable<T>? value)
    {
        if (Result is not IEnumerable<T> result)
        {
            throw new InvalidCastException($"Unable to cast result of type {typeof(TResult)} to {typeof(T)}");
        }

        return base.AsCollection(value ?? result);
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(Result))]
    public bool HasValue => IsSuccess && Result is not null;
}
