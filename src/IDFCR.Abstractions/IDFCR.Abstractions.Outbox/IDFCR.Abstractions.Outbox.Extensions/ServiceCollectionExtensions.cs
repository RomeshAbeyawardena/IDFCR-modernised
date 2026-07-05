
using IDFCR.Abstractions.DependencyInjection.Extensions;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Outbox.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.Outbox.Extensions;

/// <summary>
/// Represents extension methods for <see cref="IServiceCollection"/> to add services related to the outbox pattern, allowing for the processing of outbox messages and the tracking of their status. This class provides a method for registering services that implement the outbox pattern, enabling developers to easily integrate outbox functionality into their applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services related to the outbox pattern to the specified <see cref="IServiceCollection"/>. This method registers the necessary services for processing outbox messages and tracking their status, allowing developers to easily integrate outbox functionality into their applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the outbox services will be added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the outbox services added.</returns>
    public static IServiceCollection AddOutboxPattern(IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;

        return services
            .ScanGenericServices<IEntityInterceptor>(ServiceLifetime.Transient, assembly);
    }
}
