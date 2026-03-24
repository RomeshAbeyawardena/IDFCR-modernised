using IDFCR.AI.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.AI.OpenAI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAI(this IServiceCollection services)
    {
        services.AddHttpAIService();
        services.AddTransient<IOpenAIService, OpenAIService>();
        return services;
    }
}
