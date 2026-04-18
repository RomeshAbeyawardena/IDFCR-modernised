using IDFCR.Abstractions.Mediator;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Mediator.Extensions.Extensions;
using IDFCR.Abstractions.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IDFCR.Abstractions.Mediator.Extensions.Tests;

public sealed class ServiceCollectionThrowingRequest : IUnitResultRequest
{
}

public sealed class ServiceCollectionThrowingRequestHandler : IUnitResultRequestHandler<ServiceCollectionThrowingRequest>
{
    public Task<IUnitResult> Handle(ServiceCollectionThrowingRequest request, CancellationToken cancellationToken)
        => throw new InvalidOperationException("service collection failure");
}

[TestFixture]
public class ServiceCollectionExtensionsTests
{
    [Test]
    public void ConfigureExceptionBehaviourManager_registers_singleton_manager_with_configured_behaviour()
    {
        var expected = new ExceptionBehaviour(UnitAction.Update, FailureReason.Conflict);

        using var services = new ServiceCollection()
            .ConfigureExceptionBehaviourManager(builder => builder.Set<InvalidOperationException>(expected))
            .BuildServiceProvider();

        var first = services.GetRequiredService<IExceptionBehaviourManager>();
        var second = services.GetRequiredService<IExceptionBehaviourManager>();
        var behaviour = first.GetExceptionBehaviour<InvalidOperationException>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.SameAs(second));
            Assert.That(behaviour, Is.Not.Null);
            Assert.That(behaviour, Is.EqualTo(expected));
        }
    }

    [Test]
    public async Task AddMediatorServicesAndPipelines_registers_mediator_and_applies_configured_exception_behaviour()
    {
        var expected = new ExceptionBehaviour(UnitAction.Conflict, FailureReason.Forbidden);

        using var services = new ServiceCollection()
            .AddLogging()
            .ConfigureExceptionBehaviourManager(builder => builder.Set<InvalidOperationException>(expected))
            .AddMediatorServicesAndPipelines(typeof(ServiceCollectionExtensionsTests).Assembly)
            .BuildServiceProvider();

        var mediator = services.GetRequiredService<IMediator>();
        var result = await mediator.Send(new ServiceCollectionThrowingRequest());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(mediator, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Action, Is.EqualTo(UnitAction.Conflict));
            Assert.That(result.FailureReason, Is.EqualTo(FailureReason.Forbidden));
            Assert.That(result.Exception, Is.TypeOf<InvalidOperationException>());
            Assert.That(result.Exception?.Message, Is.EqualTo("service collection failure"));
        }
    }
}
