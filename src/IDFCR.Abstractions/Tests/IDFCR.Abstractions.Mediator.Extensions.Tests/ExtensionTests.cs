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
    public void Test()
    {
        var result = mediator.Send(new MyTestRequest());
    }
}
