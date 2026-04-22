namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a safer exception, which is an exception that is designed to be more secure and less likely to expose sensitive information. This class can be used
/// </summary>
/// <param name="innerException">The original exception that caused the failure.</param>
/// <param name="saferMessage">A safer message that describes the exception without exposing sensitive information.</param>
/// <param name="statusCode">The application status code associated with the exception, if available.</param>
/// <param name="failureReason">The reason for the failure that caused the exception, if available.</param>
public sealed class SaferException(Exception innerException, string saferMessage, int? statusCode, FailureReason? failureReason) : Exception(saferMessage, innerException), ISaferException
{
    /// <inheritdoc /> 
    public FailureReason? FailureReason { get; } = failureReason;
    /// <inheritdoc /> 
    public override string Message { get; } = saferMessage;
    /// <inheritdoc /> 
    public int? StatusCode { get; } = statusCode;
}
