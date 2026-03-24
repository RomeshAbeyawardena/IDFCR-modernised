namespace IDFCR.AI.Abstractions;

public interface IAIService
{
    Task<VerifiedConnectionResult> VerifyConnection<TConfiguration>(TConfiguration configuration, CancellationToken cancellationToken)
        where TConfiguration : IAIServiceConfiguration;

    Task<AIServiceResponse> SendAsync<TConfiguration>(TConfiguration configuration, AIServiceRequest request, CancellationToken cancellationToken)
        where TConfiguration : IAIServiceConfiguration;
}
