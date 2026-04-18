namespace IDFCR.Abstractions.Results.Exceptions;


/// <summary>
/// Defines an interface for exceptions that can be safely exposed to external clients.
/// Implementing this interface indicates that the exception contains information suitable for client consumption.
/// </summary>
public interface IExposableException
{
    /// <summary>
    /// Gets the message that describes the exception.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Gets additional details about the exception, if available.
    /// </summary>
    string? Details { get; }
}
