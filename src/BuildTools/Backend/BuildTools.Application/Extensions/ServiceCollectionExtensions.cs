using IDFCR.Abstractions.Mediator.Extensions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildTools.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatorServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.ConfigureExceptionBehaviourManager(x => { })
        .AddMediatorServicesAndPipelines([.. assemblies, typeof(ServiceCollectionExtensions).Assembly]);
    }
}
