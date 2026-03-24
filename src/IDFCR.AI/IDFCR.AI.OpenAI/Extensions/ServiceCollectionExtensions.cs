using IDFCR.AI.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.AI.OpenAI.Extensions;

/// <summary>
/// Service registration helpers for OpenAI support.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the OpenAI-specific services and their underlying HTTP transport.
    /// </summary>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddOpenAI(this IServiceCollection services)
    {
        services.AddHttpAIService();
        services.AddTransient<IOpenAIService, OpenAIService>();
        return services;
    }
}
