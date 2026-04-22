namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a safer exception, which is an exception that is designed to be more secure and less likely to expose sensitive information. This interface can be implemented by classes that represent safer exceptions, such as by sanitizing exception messages or by providing additional context for exceptions.
/// </summary>
public interface ISaferException
{
    /// <summary>
    /// Gets the reason for the failure that caused the exception, if available. This property can be used to provide additional context for the exception, such as by indicating the specific reason for the failure or by providing additional information about the failure. The value of this property may be null if the reason for the failure is not available or if it is not applicable to the exception.
    /// </summary>
    FailureReason? FailureReason { get; }
    /// <summary>
    /// Gets the message that describes the exception. This property can be used to provide a more detailed description of the exception, including any relevant context or information about the failure.
    /// </summary>
    string Message { get; }
    /// <summary>
    /// Gets the application status code associated with the exception, if available. This property can be used to provide additional context for the exception, such as by indicating the specific status code that is associated with the failure or by providing additional information about the failure. The value of this property may be null if the status code is not available or if it is not applicable to the exception.
    /// </summary>
    int? StatusCode { get; }
}
