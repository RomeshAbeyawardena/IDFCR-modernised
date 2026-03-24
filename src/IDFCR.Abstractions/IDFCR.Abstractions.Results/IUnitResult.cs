using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents the common shape of a unit result.
/// </summary>
public interface IUnitResult : IReadOnlyDictionary<string, object?>
{
    /// <summary>
    /// Gets the failure reason, when one is available.
    /// </summary>
    FailureReason? FailureReason { get; }

    /// <summary>
    /// Gets the exception captured by the result, if any.
    /// </summary>
    Exception? Exception { get; }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the action associated with the result.
    /// </summary>
    UnitAction Action { get; }

    /// <summary>
    /// Adds or replaces a metadata value.
    /// </summary>
    IUnitResult AddMeta(string key, object? value);

    /// <summary>
    /// Casts the result to another typed unit result.
    /// </summary>
    IUnitResult<T> As<T>(T? value = default);

    /// <summary>
    /// Casts the result to a collection result.
    /// </summary>
    IUnitResultCollection<T> AsCollection<T>(IEnumerable<T>? value = default);
}

/// <summary>
/// Represents a typed unit result.
/// </summary>
/// <typeparam name="TResult">The result value type.</typeparam>
public interface IUnitResult<TResult> : IUnitResult
{
    /// <summary>
    /// Gets the underlying result value.
    /// </summary>
    TResult? Result { get; }

    /// <summary>
    /// Gets a value indicating whether the result contains a non-null value and succeeded.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Result))]
    bool HasValue { get; }
}
