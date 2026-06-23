using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents the common shape of a unit result.
/// </summary>
public interface IUnitResult
{
    /// <summary>
    /// Gets the metadata associated with the result.
    /// </summary>
    IReadOnlyDictionary<string, object?> Meta { get; }
    
    /// <summary>
    /// Gets the failure reason, when one is available.
    /// </summary>
    FailureReason? FailureReason { get; }

    /// <summary>
    /// Gets the origin of the failure, when one is available. This property provides insight into where the failure occurred, which can be useful for debugging and error handling. The specific values for failure origin may include categories such as internal failures, caller code failures, or unknown origins, allowing for more granular classification of failures and better understanding of their context.
    /// </summary>
    FailureOrigin? FailureOrigin { get; }

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
}

/// <summary>
/// Represents a typed unit result.
/// </summary>
/// <typeparam name="TResult">The result value type.</typeparam>
public interface IUnitResult<TResult> : IUnitResult
{
    /// <summary>
    /// Gets the name of the result, if one is available. This can be used to identify the result in a collection of results or to provide additional context about the result.
    /// </summary>
    string? NamedResult { get; }
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
