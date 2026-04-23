namespace IDFCR.Abstractions.Results;

/// <summary>
/// Represents a builder for creating instances of <see cref="ISaferExceptionProvider"/>. This interface can be implemented by classes that provide functionality for configuring and building instances of <see cref="ISaferExceptionProvider"/>, such as by allowing the configuration of sanitization rules or by providing additional context for exceptions.
/// </summary>
public interface ISaferExceptionProviderBuilder
{
    /// <summary>
    /// Adds default safer exception implementations for common exception types. This method can be used to quickly set up a provider with predefined safer messages for frequently encountered exceptions, such as <see cref="ArgumentNullException"/>, <see cref="InvalidOperationException"/>, and others. By calling this method, the provider will be configured to return safer messages for these common exception types without the need for manual configuration of each type. This can help improve the security and user-friendliness of error messages in applications that utilize the provider.
    /// <para>
    /// Ensure you run this first before adding or updating specific exception types, as it will set up a baseline of safer exception implementations that can be further customized using the <see cref="AddOrUpdate{TException}(string, int?, FailureReason?)"/> method.
    /// Otherwise your custom configurations may be overwritten by the defaults if you call this method after adding or updating specific exception types.
    /// </para>
    /// </summary>
    /// <returns>The builder instance for chaining.</returns>
    ISaferExceptionProviderBuilder AddDefaults();
    /// <summary>
    /// Adds or updates a safer exception implementation for the specified exception type. This method can be used to configure the provider to return a specific safer message for a given exception type. If an implementation for the specified exception type already exists, it will be updated with the new safer message; otherwise, a new implementation will be added to the provider.
    /// </summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <param name="saferMessage">The safer message to associate with the exception type.</param>
    /// <param name="statusCode">The status code to associate with the exception type.</param>
    /// <param name="failureReason">The failure reason to associate with the exception type.</param>
    /// <returns>The builder instance for chaining.</returns>
    ISaferExceptionProviderBuilder AddOrUpdate<TException>(string saferMessage, int? statusCode, FailureReason? failureReason)
        where TException : Exception;
    /// <summary>
    /// Builds and returns an instance of <see cref="ISaferExceptionProvider"/> based on the configured safer exception implementations. This method can be used to create an instance of <see cref="ISaferExceptionProvider"/> that is ready to be used for retrieving safer exceptions based on the configurations provided through the builder. The returned instance will contain the mappings of exception types to their corresponding safer messages as configured through the builder.
    /// </summary>
    /// <returns>The <see cref="ISaferExceptionProvider"/> instance.</returns>
    ISaferExceptionProvider Build();
}