namespace IDFCR.AI.Abstractions;

/// <summary>
/// Defines the low-level transport contract used by AI providers.
/// </summary>
public interface IAIService
{
    /// <summary>
    /// Verifies that the configured AI service can be reached and accepts the provided configuration.
    /// </summary>
    /// <typeparam name="TConfiguration">The configuration type used by the concrete service implementation.</typeparam>
    /// <param name="configuration">The service configuration to verify.</param>
    /// <param name="cancellationToken">A token that can cancel the operation.</param>
    /// <returns>A result describing whether the connection check succeeded.</returns>
    Task<VerifiedConnectionResult> VerifyConnection<TConfiguration>(TConfiguration configuration, CancellationToken cancellationToken)
        where TConfiguration : IAIServiceConfiguration;

    /// <summary>
    /// Sends a request to the configured AI service and returns the raw response details.
    /// </summary>
    /// <typeparam name="TConfiguration">The configuration type used by the concrete service implementation.</typeparam>
    /// <param name="configuration">The service configuration to use.</param>
    /// <param name="request">The request payload to send.</param>
    /// <param name="cancellationToken">A token that can cancel the operation.</param>
    /// <returns>The HTTP-like response returned by the underlying service.</returns>
    Task<AIServiceResponse> SendAsync<TConfiguration>(TConfiguration configuration, AIServiceRequest request, CancellationToken cancellationToken)
        where TConfiguration : IAIServiceConfiguration;
}
