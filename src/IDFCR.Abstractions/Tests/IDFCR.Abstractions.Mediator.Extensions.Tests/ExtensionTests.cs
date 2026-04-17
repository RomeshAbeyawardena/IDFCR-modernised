using IDFCR.Abstractions.Mediator;
using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Mediator.Extensions.Extensions;
using IDFCR.Abstractions.Results;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IDFCR.Abstractions.Mediator.Extensions.Tests;

public sealed class SuccessfulRequest : IUnitResultRequest
{
}

public sealed class SuccessfulRequestHandler : IUnitResultRequestHandler<SuccessfulRequest>
{
    public Task<IUnitResult> Handle(SuccessfulRequest request, CancellationToken cancellationToken)
        => Task.FromResult(UnitResult.Success(UnitAction.Get));
}

public sealed class ThrowingRequest : IUnitResultRequest
{
}

public sealed class ThrowingRequestHandler : IUnitResultRequestHandler<ThrowingRequest>
{
    public Task<IUnitResult> Handle(ThrowingRequest request, CancellationToken cancellationToken)
        => throw new InvalidOperationException("Simulated failure");
}

[TestFixture]
public class ExtensionTests
{
    [Test]
    public async Task AddMediatorServicesAndPipelines_dispatches_unit_result_requests()
    {
        using var services = CreateServiceProvider();
        var mediator = services.GetRequiredService<IMediator>();

        var result = await mediator.Send(new SuccessfulRequest());

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Action, Is.EqualTo(UnitAction.Get));
            Assert.That(result.Exception, Is.Null);
        });
    }

    [Test]
    public async Task GenericDefaultExceptionPipeline_converts_handler_exceptions_to_failed_unit_results()
    {
        using var services = CreateServiceProvider(registerExceptionPipeline: true);
        var mediator = services.GetRequiredService<IMediator>();

        var result = await mediator.Send(new ThrowingRequest());

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Action, Is.EqualTo(UnitAction.None));
            Assert.That(result.FailureReason, Is.EqualTo(FailureReason.Unknown));
            Assert.That(result.Exception, Is.TypeOf<InvalidOperationException>());
            Assert.That(result.Exception?.Message, Is.EqualTo("Simulated failure"));
        });
    }

    private static ServiceProvider CreateServiceProvider(bool registerExceptionPipeline = false)
    {
        var services = new ServiceCollection()
            .AddLogging()
            .ConfigureExceptionBehaviourManager(_ => { })
            .AddMediatorServicesAndPipelines(typeof(ExtensionTests).Assembly);

        return services.BuildServiceProvider();
    }
}
