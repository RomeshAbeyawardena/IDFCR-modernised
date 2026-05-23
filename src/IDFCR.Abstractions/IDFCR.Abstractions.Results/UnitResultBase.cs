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
    public virtual bool TrySetState(object? value) => false;

    /// <inheritdoc />
    public virtual bool Equals(UnitResultBase? other)
    // Mutable runtime state (metadata/state overlays) is intentionally
    // excluded from record equality/hash semantics because it is not
    // part of the immutable value identity of the result.
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null || EqualityContract != other.EqualityContract)
        {
            return false;
        }

        return Action == other.Action
               && IsSuccess == other.IsSuccess
               && Equals(Exception, other.Exception)
               && FailureReason == other.FailureReason;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Action,
            IsSuccess,
            Exception,
            FailureReason);
    }

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
    /// <summary>
    /// Gets a value indicating whether the state has been modified. This property is used to track whether the original state has been changed or updated, allowing for scenarios where the result may need to be modified after its initial creation. By tracking this state, it enables the implementation of features such as change tracking, auditing, or conditional logic based on whether the result has been modified since it was created.
    /// </summary>
    protected bool IsStateModified;
    
    /// <inheritdoc />
    public TResult? ModifiedState { get; private set; }

    /// <summary>
    /// Gets the underlying result value. If a modified state is available, it returns the modified state; otherwise, it returns the original state. This allows for a flexible representation of the result, where the original state can be preserved while still allowing for modifications or updates to be tracked through the modified state.
    /// </summary>
    public TResult? Result => IsStateModified ? ModifiedState : OriginalState;

    /// <inheritdoc />
    protected override Type EqualityContract => typeof(UnitResultBase<TResult>);

    
    /// <inheritdoc />
    public virtual bool Equals(UnitResultBase<TResult>? other)
    // Mutable runtime state (metadata/state overlays) is intentionally
    // excluded from record equality/hash semantics because it is not
    // part of the immutable value identity of the result.
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null || EqualityContract != other.EqualityContract)
        {
            return false;
        }

        return EqualityComparer<TResult?>.Default.Equals(OriginalState, other.OriginalState)
               && Action == other.Action
               && IsSuccess == other.IsSuccess
               && Equals(Exception, other.Exception)
               && FailureReason == other.FailureReason
               && NamedResult == other.NamedResult;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            OriginalState,
            Action,
            IsSuccess,
            Exception,
            FailureReason,
            NamedResult);
    }

    /// <inheritdoc />
    public bool TrySetState(TResult? result)
    {
        IsStateModified = result is not null;
        ModifiedState = result;
        return IsStateModified;
    }

    /// <summary>
    /// Attempts to set the modified state of the result by setting the ModifiedState property. Returns true if the value is of the correct type and was set successfully; otherwise, returns false.
    /// </summary>
    /// <param name="value">The value to set as the modified state.</param>
    /// <returns>True if the modified state was set successfully; otherwise, false.</returns>
    public override bool TrySetState(object? value)
    {
        if (value is TResult result)
        {
            return TrySetState(result);
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
