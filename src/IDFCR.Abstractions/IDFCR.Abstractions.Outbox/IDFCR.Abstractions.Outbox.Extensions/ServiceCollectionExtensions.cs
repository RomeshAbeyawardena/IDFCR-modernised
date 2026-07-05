
using IDFCR.Abstractions.DependencyInjection.Extensions;
using IDFCR.Abstractions.Interceptors.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.Outbox.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOutboxPattern(IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;
        return services.ScanGenericServices<IEntityInterceptor>(ServiceLifetime.Transient, assembly);
    }
}
