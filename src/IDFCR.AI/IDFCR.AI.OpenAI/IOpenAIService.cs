using IDFCR.AI.Abstractions;
using IDFCR.AI.OpenAI.Configurations;
using IDFCR.AI.OpenAI.Models;

namespace IDFCR.AI.OpenAI;

/// <summary>
/// High-level OpenAI operations built on top of <see cref="IAIService"/>.
/// </summary>
public interface IOpenAIService
{
    /// <summary>
    /// Verifies that the configured OpenAI endpoint is reachable and accepts the supplied credentials.
    /// </summary>
    /// <param name="configuration">The OpenAI configuration to verify.</param>
    /// <param name="cancellationToken">A token that can cancel the operation.</param>
    /// <returns>A result describing whether the connection check succeeded.</returns>
    Task<VerifiedConnectionResult> VerifyConnection(OpenAIConfiguration configuration, CancellationToken cancellationToken);

    /// <summary>
    /// Generates text using the OpenAI Responses API.
    /// </summary>
    /// <param name="configuration">The OpenAI configuration to use.</param>
    /// <param name="request">The text-generation request payload.</param>
    /// <param name="cancellationToken">A token that can cancel the operation.</param>
    /// <returns>A response containing the raw payload and extracted text, if available.</returns>
    Task<OpenAITextResponse> GenerateTextAsync(OpenAIConfiguration configuration, OpenAITextRequest request, CancellationToken cancellationToken);
}
