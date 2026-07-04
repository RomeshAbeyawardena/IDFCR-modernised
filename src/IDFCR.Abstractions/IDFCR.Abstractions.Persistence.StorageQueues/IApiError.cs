namespace IDFCR.Abstractions.Persistence.StorageQueues;

/// <summary>
/// Represents an error that can occur when interacting with a queue or other systems. The interface provides properties for the error code and message, allowing for identification and description of the error. This interface can be implemented by classes that represent errors in different systems, providing a consistent way to handle and report errors regardless of the underlying implementation.
/// </summary>
public interface IApiError
{
    /// <summary>
    /// Gets the error code associated with the error. This property provides a way to identify the specific error that occurred, allowing for handling and reporting of the error as needed.
    /// </summary>
    int Code { get; }
    /// <summary>
    /// Gets the error message associated with the error. This property provides a description of the error, allowing for understanding and reporting of the error as needed.
    /// </summary>
    string Message { get; }
}