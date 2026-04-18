using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Mediator.Extensions.Extensions;
using IDFCR.Abstractions.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace BuildTools.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatorServices(this IServiceCollection services,
        IConfiguration configuration, params Assembly[] assemblies)
    {
        var licenses = configuration.GetSection("Licenses").Get<LicenseConfiguration>();

        return services
            .ConfigureExceptionBehaviourManager(x =>
            {
                x
            .Set<NullReferenceException>(new ExceptionBehaviour(UnitAction.Get, FailureReason.NotFound))
            .Set<InvalidCastException>(new ExceptionBehaviour(UnitAction.Conflict, FailureReason.Conflict));
            })
        .AddMediatorServicesAndPipelines(cfg =>
        {
            cfg.LicenseKey = licenses?.LuckyPenny;
        }, [.. assemblies, typeof(ServiceCollectionExtensions).Assembly]);
    }
}
