using IDFCR.Abstractions.Results.Exceptions;

namespace IDFCR.Abstractions.Results;

/// <summary>
/// Factory helpers for unit results.
/// </summary>
public static class UnitResult
{
    /// <summary>
    /// Creates a unit result with the specified success status, exception, action, and failure reason.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="exception">The exception associated with the result, if any.</param>
    /// <param name="action">The action associated with the result.</param>
    /// <param name="failureReason">The reason for the failure, if any.</param>
    /// <param name="failureOrigin">The origin of the failure, if any.</param>
    /// <returns>An <see cref="IUnitResult"/> representing the result of the operation.</returns>
    public static IUnitResult Create(bool isSuccess, Exception? exception = null, UnitAction action = UnitAction.None, FailureReason? failureReason = null, FailureOrigin? failureOrigin = null)
    {
        return new DefaultUnitResult(exception, action, isSuccess, failureReason, failureOrigin);
    }

    /// <summary>
    /// Creates a unit result with the specified result value, success status, exception, action, and failure reason.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result value.</param>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="exception">The exception associated with the result, if any.</param>
    /// <param name="action">The action associated with the result.</param>
    /// <param name="failureReason">The reason for the failure, if any.</param>
    /// <param name="namedResult"></param>
    /// <param name="failureOrigin">The origin of the failure, if any.</param>
    /// <returns>An <see cref="IUnitResult{T}"/> representing the result of the operation.</returns>
    public static IUnitResult<T> Create<T>(T? result, 
        bool isSuccess, 
        Exception? exception = null, 
        UnitAction action = UnitAction.None, 
        FailureReason? failureReason = null,
        FailureOrigin? failureOrigin = null,
        string? namedResult = null)
    {
        return new DefaultUnitResult<T>(result, action, isSuccess, exception, failureReason, namedResult, failureOrigin);
    }

    /// <summary>
    /// Creates a failed result that represents a missing entity.
    /// </summary>
    /// <param name="id">The identifier of the missing entity.</param>
    /// <param name="innerException">The inner exception, if any.</param>
    /// <param name="failureReason">The reason for the failure, if any.</param>
    /// <param name="failureOrigin">The origin of the failure, if any.</param>
    public static IUnitResult<T> NotFound<T>(object id, Exception? innerException = null, FailureReason? failureReason = FailureReason.NotFound, FailureOrigin? failureOrigin = null)
        => Failed<T>(new EntityNotFoundException(typeof(T), id, innerException), UnitAction.None, failureReason, failureOrigin: failureOrigin);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="exception">The exception that caused the failure. Cannot be null.</param>
    /// <param name="action">The action associated with the failed result.</param>
    /// <param name="failureReason">The reason for the failure.</param>
    /// <param name="failureOrigin">The origin of the failure, if any.</param>
    /// <param name="namedResult">The name of the result, if any.</param>
    public static IUnitResult<T> Failed<T>(Exception exception, UnitAction action = UnitAction.None, FailureReason? failureReason = null, FailureOrigin? failureOrigin = null, string? namedResult = null)
        => new DefaultUnitResult<T>(default, action, false, exception, failureReason, namedResult, failureOrigin);

    /// <summary>
    /// Creates a unit result from the supplied value.
    /// </summary>
    /// <param name="result">The result value.</param>
    /// <param name="action">The action associated with the result.</param>
    /// <param name="isSuccess">Indicates whether the operation was successful. Defaults to true.</param>
    /// <param name="exception">The exception associated with the result, if any. Defaults to null.</param>
    /// <param name="namedResult">The name of the result, if any. Defaults to null.</param>
    public static IUnitResult<T> FromResult<T>(T? result, UnitAction action = UnitAction.Get,
        bool isSuccess = true, Exception? exception = null, string? namedResult = null)
    {
        return new DefaultUnitResult<T>(result, action, isSuccess, exception, null, namedResult);
    }

    /// <summary>
    /// Creates a successful unit result with the specified action.
    /// </summary>
    /// <param name="action">The action that describes the outcome of the operation. This value is associated with the result to provide
    /// additional context.</param>
    /// <returns>An object representing a successful unit result containing the specified action.</returns>
    public static IUnitResult Success(UnitAction action)
    {
        return new DefaultUnitResult(Action: action, IsSuccess: true);
    }

    /// <summary>
    /// Creates a failed unit result with the specified exception and action.
    /// </summary>
    /// <param name="exception">The exception that caused the failure. Cannot be null.</param>
    /// <param name="action">The action associated with the failed result.</param>
    /// <param name="failureReason">The reason for the failure.</param>
    /// <param name="failureOrigin">The origin of the failure, if any.</param>
    /// <returns>An <see cref="IUnitResult"/> representing a failed operation with the provided exception and action.</returns>
    public static IUnitResult Failed(Exception exception, UnitAction action, FailureReason failureReason, FailureOrigin? failureOrigin = null)
    {
        return new DefaultUnitResult(exception, action, false, failureReason, failureOrigin);
    }

    /// <summary>
    /// Throws an <see cref="InternalResultFailureException"/> if the provided result indicates a failure with an internal failure origin.
    /// </summary>
    /// <param name="result">The result to check for internal failure.</param>
    public static void ThrowIfInternalFailure(this IUnitResult result)
    {
        InternalResultFailureException.ThrowOnInternalFailure(result);
    }

    /// <summary>
    /// Determines whether the provided result indicates a failure with an internal failure origin.
    /// </summary>
    /// <param name="result">The result to check for internal failure.</param>
    /// <returns>True if the result indicates an internal failure; otherwise, false.</returns>
    public static bool IsInternalFailure(this IUnitResult result)
    {
        return  !result.IsSuccess 
                && result.FailureOrigin == FailureOrigin.Internal;
    }
}
