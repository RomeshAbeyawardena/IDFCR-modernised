using IDFCR.AI.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.AI.Http.Extensions;

/// <summary>
/// Service registration helpers for the HTTP AI transport.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IAIService"/> using the default HTTP implementation.
    /// </summary>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddHttpAIService(this IServiceCollection services)
    {
        services.AddHttpClient<IAIService, HttpAIService>();
        return services;
    }
}
