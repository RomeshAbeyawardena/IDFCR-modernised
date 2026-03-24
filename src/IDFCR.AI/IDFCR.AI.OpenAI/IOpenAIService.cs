using IDFCR.AI.Abstractions;
using IDFCR.AI.OpenAI.Configurations;
using IDFCR.AI.OpenAI.Models;

namespace IDFCR.AI.OpenAI;

public interface IOpenAIService
{
    Task<VerifiedConnectionResult> VerifyConnection(OpenAIConfiguration configuration, CancellationToken cancellationToken);

    Task<OpenAITextResponse> GenerateTextAsync(OpenAIConfiguration configuration, OpenAITextRequest request, CancellationToken cancellationToken);
}
