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
/// <param name="FailureOrigin">The origin of the failure.</param>
public abstract record UnitResultBase(Exception? Exception = null, UnitAction Action = UnitAction.None,
    bool IsSuccess = false, FailureReason? FailureReason = null, FailureOrigin? FailureOrigin = FailureOrigin.CallerCode) : IUnitResult
{
    private readonly ConcurrentDictionary<string, object?> _metaProperties = [];
    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?> Meta => _metaProperties;

    /// <inheritdoc />
    public object? this[string key] { get => _metaProperties[key]; }

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
/// <param name="Result">The result value.</param>
/// <param name="Action">The associated action.</param>
/// <param name="IsSuccess">A value indicating whether the operation succeeded.</param>
/// <param name="Exception">The captured exception.</param>
/// <param name="FailureReason">The failure reason.</param>
/// <param name="NamedResult">The name of the result.</param>
/// <param name="FailureOrigin"></param>
public abstract record UnitResultBase<TResult>(
    TResult? Result = default, UnitAction Action = UnitAction.None,
    bool IsSuccess = true, Exception? Exception = null, FailureReason? FailureReason = null, string? NamedResult = null, FailureOrigin? FailureOrigin = FailureOrigin.CallerCode)
    : UnitResultBase(Exception, Action, IsSuccess, FailureReason, FailureOrigin), IUnitResult<TResult>
{
    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(Result))]
    public bool HasValue => IsSuccess && Result is not null;
}
