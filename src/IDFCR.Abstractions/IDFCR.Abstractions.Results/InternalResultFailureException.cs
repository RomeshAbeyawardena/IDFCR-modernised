namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a failure that occurred during the execution of an operation, where the failure is considered internal to the system. This exception is thrown when an operation fails due to an internal issue, such as a database error, a network failure, or any other unexpected condition that prevents the operation from completing successfully. The exception contains information about the failed result, including the exception that caused the failure and a message describing the failure. This allows developers to handle internal failures in a consistent manner and provides insights into the underlying issues that may have caused the failure.
/// </summary>
/// <param name="failedResult">The result that caused the failure.</param>
/// <param name="message">A message describing the failure.</param>
/// <param name="innerException">The exception that caused the failure, if any.</param>
public sealed class InternalResultFailureException(IUnitResult failedResult, 
    string message, 
    Exception? innerException) : Exception(message, innerException)
{
    /// <summary>
    /// Throws an <see cref="InternalResultFailureException"/> if the provided <see cref="IUnitResult"/> indicates a failure that originated from an internal issue. This method checks the success status of the result and its failure origin, and if the result is not successful and the failure origin is internal, it throws an exception containing details about the failed result. This allows developers to easily identify and handle internal failures in their code by simply calling this method with the result of an operation. If the result is successful or if the failure origin is not internal, no exception is thrown, allowing the code to continue executing normally.
    /// </summary>
    /// <param name="result">The result to check for internal failure.</param>
    /// <exception cref="InternalResultFailureException">Thrown if the result indicates an internal failure.</exception>
    public static void ThrowOnFailure(IUnitResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (!result.IsSuccess && result.FailureOrigin == FailureOrigin.Internal)
        {
            throw new InternalResultFailureException(result, result.Exception?.Message ?? "An internal result exception occurred", result.Exception);
        }
    }

    /// <summary>
    /// Gets the failed result that caused this exception. This property provides access to the <see cref="IUnitResult"/> instance that represents the failed operation, allowing developers to inspect the details of the failure, such as the exception, action, failure reason, and failure origin. This information can be useful for debugging and handling internal failures in a consistent manner.
    /// </summary>
    public IUnitResult FailedResult { get; } = failedResult;
}