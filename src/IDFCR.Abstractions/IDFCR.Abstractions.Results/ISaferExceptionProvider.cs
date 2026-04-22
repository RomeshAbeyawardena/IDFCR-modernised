namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a provider for safer exceptions, which can be used to create exceptions that are more secure and less likely to expose sensitive information. This interface can be implemented by classes that provide functionality for creating safer exceptions, such as by sanitizing exception messages or by providing additional context for exceptions.
/// </summary>
public interface ISaferExceptionProvider
{
    /// <summary>
    /// Tries to get a safer exception implementation for the specified exception type.
    /// </summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <param name="exception">The exception instance for which to get a safer implementation.</param>
    /// <param name="saferException">The safer exception instance, if available.</param>
    /// <returns>True if a safer exception implementation is available; otherwise, false.</returns>
    bool TryGetImplementation<TException>(TException exception, out SaferException? saferException)
        where TException : Exception;

    /// <summary>
    /// Tries to get a safer exception for the specified exception type. This method can be used to retrieve a safer exception instance that is associated with the specified exception type, if available. The method returns true if a safer exception instance is available; otherwise, it returns false. The output parameter contains the safer exception instance if it is available, or null if it is not available.
    /// </summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <param name="exception">The exception instance for which to get a safer implementation.</param>
    /// <param name="saferException">The safer exception instance, if available.</param>
    /// <returns>True if a safer exception instance is available; otherwise, false.</returns>
    bool TryGet<TException>(TException exception, out ISaferException? saferException)
        where TException : Exception;
}
