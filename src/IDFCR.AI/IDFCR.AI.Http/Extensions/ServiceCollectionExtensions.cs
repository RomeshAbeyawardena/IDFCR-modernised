using IDFCR.AI.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.AI.Http.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttpAIService(this IServiceCollection services)
    {
        services.AddHttpClient<IAIService, HttpAIService>();
        return services;
    }
}
