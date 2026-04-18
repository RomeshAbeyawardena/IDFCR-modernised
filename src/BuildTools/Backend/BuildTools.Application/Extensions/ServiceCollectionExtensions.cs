using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Mediator.Extensions.Extensions;
using IDFCR.Abstractions.Results;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace BuildTools.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatorServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.ConfigureExceptionBehaviourManager(x => { x
            .Set<NullReferenceException>(new ExceptionBehaviour(UnitAction.Get, FailureReason.NotFound))
            .Set<InvalidCastException>(new ExceptionBehaviour(UnitAction.Conflict, FailureReason.Conflict));
        })
        .AddMediatorServicesAndPipelines([.. assemblies, typeof(ServiceCollectionExtensions).Assembly]);
    }
}
