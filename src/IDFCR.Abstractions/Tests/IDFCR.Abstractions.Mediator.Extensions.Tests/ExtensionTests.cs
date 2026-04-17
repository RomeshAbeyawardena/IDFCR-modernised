using IDFCR.Abstractions.Mediator.Extensions.Extensions;
using IDFCR.Abstractions.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IDFCR.Abstractions.Mediator.Extensions.Tests;
public class Customer
{

}
public class MyTestRequest : IUnitResultRequest
{

}

public class MyTestRequestHandler : IUnitResultRequestHandler<MyTestRequest>
{
    public Task<IUnitResult> Handle(MyTestRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

[TestFixture]
public class ExtensionTests
{
    private IServiceProvider services;
    private IMediator mediator;
    [SetUp]
    public void SetUp()
    {
        services = new ServiceCollection()
            .AddLogging()
            .ConfigureExceptionBehaviourManager(cfg =>
            {

            })
            .AddMediatorServicesAndPipelines(typeof(ExtensionTests).Assembly)
            .BuildServiceProvider();
        mediator = services.GetRequiredService<IMediator>();
    }

    [TearDown]
    public void TearDown()
    {
        if (services is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    [Test]
    public async Task Test()
    {

        IUnitResult? result = null;
        
        Assert.DoesNotThrowAsync(async () => result = await mediator.Send(new MyTestRequest()));

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Exception, Is.InstanceOf<NotImplementedException>());
        }

    }
}
