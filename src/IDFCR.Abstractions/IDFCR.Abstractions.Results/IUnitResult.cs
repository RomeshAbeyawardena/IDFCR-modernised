using System.Diagnostics.CodeAnalysis;

namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a builder for creating unit results. This interface provides methods for adding metadata, casting the result to different types, and building the final result. It allows for flexible construction of unit results with various states and associated data, enabling developers to create results that accurately reflect the outcome of operations and provide additional context through metadata. The builder pattern used here facilitates a fluent and expressive way to construct unit results, making it easier to manage complex result scenarios in a consistent manner.
/// </summary>
public interface IUnitResultBuilder
{
    /// <summary>
    /// Adds or replaces a metadata value.
    /// </summary>
    IUnitResultBuilder AddMeta(string key, object? value);

    /// <summary>
    /// Casts the result to another typed unit result.
    /// </summary>
    IUnitResult<T> As<T>(T? value = default);

    /// <summary>
    /// Casts the result to a collection result.
    /// </summary>
    IUnitResultCollection<T> AsCollection<T>(IEnumerable<T>? value = default);

    /// <summary>
    /// Builds the final unit result based on the current state of the builder. This method consolidates all the information and configurations that have been set on the builder, such as metadata, state, and any other relevant properties, into a single unit result object. The resulting unit result encapsulates the outcome of an operation, including success or failure status, associated data, and any additional context provided through metadata. The Build method is typically called after all necessary configurations have been made to ensure that the final result accurately reflects the intended state and information.
    /// </summary>
    /// <returns>The constructed unit result.</returns>
    IUnitResult Build();
}

/// <summary>
/// Represents a builder for creating typed unit results. This interface extends the IUnitResultBuilder interface and provides additional functionality for building unit results that contain a specific type of result value. It allows for the construction of unit results with associated data of type T, enabling developers to create results that accurately reflect the outcome of operations and provide additional context through metadata. The builder pattern used here facilitates a fluent and expressive way to construct typed unit results, making it easier to manage complex result scenarios in a consistent manner.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IUnitResultBuilder<T> : IUnitResultBuilder
{
    /// <summary>
    /// Builds the final typed unit result based on the current state of the builder. This method consolidates all the information and configurations that have been set on the builder, such as metadata, state, and any other relevant properties, into a single typed unit result object. The resulting typed unit result encapsulates the outcome of an operation, including success or failure status, associated data of type T, and any additional context provided through metadata. The Build method is typically called after all necessary configurations have been made to ensure that the final typed result accurately reflects the intended state and information.
    /// </summary>
    /// <returns>The constructed typed unit result.</returns>
    new IUnitResult<T> Build();
}


/// <summary>
/// Represents the common shape of a unit result.
/// </summary>
public interface IUnitResult
{
    /// <summary>
    /// Attempts to set the state of the result to the provided value. The specific behavior of this method may depend on the implementation, but it generally allows for updating the result's state based on new information or conditions. The method returns a boolean indicating whether the state was successfully set, which can be used to determine if the operation was valid or if any constraints were violated.
    /// </summary>
    /// <param name="value">The value to set the state to.</param>
    /// <returns>True if the state was successfully set; otherwise, false.</returns>
    bool TrySetState(object? value);
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
    /// Attempts to set the state of the result to the provided value. The specific behavior of this method may depend on the implementation, but it generally allows for updating the result's state based on new information or conditions. The method returns a boolean indicating whether the state was successfully set, which can be used to determine if the operation was valid or if any constraints were violated.
    /// </summary>
    /// <param name="value">The value to set the state to.</param>
    /// <returns>True if the state was successfully set; otherwise, false.</returns>
    bool TrySetState(TResult? value);
    /// <summary>
    /// Gets the original state of the result, if one is available. This property represents the initial value of the result before any modifications or updates have been made. It can be used to compare against the modified state to determine if any changes have occurred, or to provide a reference point for the original data that was processed to produce the result.
    /// </summary>
    TResult? OriginalState { get; }
    /// <summary>
    /// Gets the modified state of the result, if one is available. This property can be used to track changes to the result's state after it has been created, allowing for more dynamic and flexible handling of results in various scenarios. The specific meaning and usage of the modified state may depend on the context in which the result is being used, but it generally provides insight into how the result has evolved over time.
    /// </summary>
    TResult? ModifiedState { get; }
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
