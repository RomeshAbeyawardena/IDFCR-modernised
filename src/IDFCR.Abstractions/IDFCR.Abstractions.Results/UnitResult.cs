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
    /// <returns>An <see cref="IUnitResult"/> representing the result of the operation.</returns>
    public static IUnitResult Create(bool isSuccess, Exception? exception = null, UnitAction action = UnitAction.None, FailureReason? failureReason = null)
    {
        return new DefaultUnitResult(exception, action, isSuccess, failureReason);
    }

    /// <summary>
    /// Creates a failed result that represents a missing entity.
    /// </summary>
    public static IUnitResult<T> NotFound<T>(object id, Exception? innerException = null, FailureReason? failureReason = FailureReason.NotFound)
        => Failed<T>(new EntityNotFoundException(typeof(T), id, innerException), UnitAction.None, failureReason);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static IUnitResult<T> Failed<T>(Exception exception, UnitAction action = UnitAction.None, FailureReason? failureReason = null, string? namedResult = null)
        => new DefaultUnitResult<T>(default, action, false, exception, failureReason, namedResult);

    /// <summary>
    /// Creates a unit result from the supplied value.
    /// </summary>
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
    /// <returns>An <see cref="IUnitResult"/> representing a failed operation with the provided exception and action.</returns>
    public static IUnitResult Failed(Exception exception, UnitAction action, FailureReason failureReason)
    {
        return new DefaultUnitResult(exception, action, false, failureReason);
    }
}
