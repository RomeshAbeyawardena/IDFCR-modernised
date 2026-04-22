namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a builder for creating instances of <see cref="ISaferExceptionProvider"/>. This interface can be implemented by classes that provide functionality for configuring and building instances of <see cref="ISaferExceptionProvider"/>, such as by allowing the configuration of sanitization rules or by providing additional context for exceptions.
/// </summary>
public interface ISaferExceptionProviderBuilder
{
    /// <summary>
    /// Adds or updates a safer exception implementation for the specified exception type. This method can be used to configure the provider to return a specific safer message for a given exception type. If an implementation for the specified exception type already exists, it will be updated with the new safer message; otherwise, a new implementation will be added to the provider.
    /// </summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <param name="saferMessage">The safer message to associate with the exception type.</param>
    /// <returns>The builder instance for chaining.</returns>
    ISaferExceptionProviderBuilder AddOrUpdate<TException>(string saferMessage, int? statusCode, FailureReason? failureReason)
        where TException : Exception;
    /// <summary>
    /// Builds and returns an instance of <see cref="ISaferExceptionProvider"/> based on the configured safer exception implementations. This method can be used to create an instance of <see cref="ISaferExceptionProvider"/> that is ready to be used for retrieving safer exceptions based on the configurations provided through the builder. The returned instance will contain the mappings of exception types to their corresponding safer messages as configured through the builder.
    /// </summary>
    /// <returns>The <see cref="ISaferExceptionProvider"/> instance.</returns>
    ISaferExceptionProvider Build();
}