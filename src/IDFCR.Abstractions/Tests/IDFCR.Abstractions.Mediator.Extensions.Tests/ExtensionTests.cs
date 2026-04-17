using IDFCR.Abstractions.Mediator;
using IDFCR.Abstractions.Mediator.Extensions.Extensions;
using IDFCR.Abstractions.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IDFCR.Abstractions.Mediator.Extensions.Tests;

public sealed record Customer(int Id, string Name);

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

public sealed class CustomerCollectionRequest : IUnitResultCollectionRequest<Customer>
{
}

public sealed class CustomerCollectionRequestHandler : IRequestHandler<CustomerCollectionRequest, IUnitResultCollection<Customer>>
{
    public Task<IUnitResultCollection<Customer>> Handle(CustomerCollectionRequest request, CancellationToken cancellationToken)
        => Task.FromResult(UnitResultCollection.FromResult(
            [
                new Customer(1, "Ada"),
                new Customer(2, "Grace")
            ],
            UnitAction.Get));
}

public sealed class FailingCustomerCollectionRequest : IUnitResultCollectionRequest<Customer>
{
}

public sealed class FailingCustomerCollectionRequestHandler : IRequestHandler<FailingCustomerCollectionRequest, IUnitResultCollection<Customer>>
{
    public Task<IUnitResultCollection<Customer>> Handle(FailingCustomerCollectionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            throw new InvalidOperationException("Simulated collection failure");
        }
        catch (Exception ex)
        {
            return Task.FromResult(UnitResultCollection.Failed<Customer>(ex));
        }
    }
}

public sealed record CustomerPagedRequest() : PagedQuery(2, 1), IPagedUnitResultRequest<Customer>;

public sealed class CustomerPagedRequestHandler : IRequestHandler<CustomerPagedRequest, IUnitPagedResult<Customer>>
{
    public Task<IUnitPagedResult<Customer>> Handle(CustomerPagedRequest request, CancellationToken cancellationToken)
        => Task.FromResult(UnitPagedResult.FromResult(
            [
                new Customer(3, "Linus"),
                new Customer(4, "Margaret")
            ],
            totalRows: 5,
            pagedQuery: request,
            action: UnitAction.Get));
}

public sealed record FailingCustomerPagedRequest() : PagedQuery(2, 1), IPagedUnitResultRequest<Customer>;

public sealed class FailingCustomerPagedRequestHandler : IRequestHandler<FailingCustomerPagedRequest, IUnitPagedResult<Customer>>
{
    public Task<IUnitPagedResult<Customer>> Handle(FailingCustomerPagedRequest request, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException("Simulated paged failure");
    }
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
    public async Task AddMediatorServicesAndPipelines_dispatches_unit_result_collection_requests()
    {
        using var services = CreateServiceProvider();
        var mediator = services.GetRequiredService<IMediator>();

        var result = await mediator.Send(new CustomerCollectionRequest());
        var customers = result.Result?.ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Action, Is.EqualTo(UnitAction.Get));
            Assert.That(customers, Has.Length.EqualTo(2));
            Assert.That(customers?.Select(x => x.Name), Is.EqualTo(new[] { "Ada", "Grace" }));
        });
    }

    [Test]
    public async Task AddMediatorServicesAndPipelines_returns_failed_unit_result_collection_with_embedded_exception()
    {
        using var services = CreateServiceProvider();
        var mediator = services.GetRequiredService<IMediator>();

        var result = await mediator.Send(new FailingCustomerCollectionRequest());

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Action, Is.EqualTo(UnitAction.None));
            Assert.That(result.Result, Is.Null);
            Assert.That(result.Exception, Is.TypeOf<InvalidOperationException>());
            Assert.That(result.Exception?.Message, Is.EqualTo("Simulated collection failure"));
        });
    }

    [Test]
    public async Task AddMediatorServicesAndPipelines_dispatches_paged_unit_result_requests()
    {
        using var services = CreateServiceProvider();
        var mediator = services.GetRequiredService<IMediator>();

        var result = await mediator.Send(new CustomerPagedRequest());
        var customers = result.Result?.ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Action, Is.EqualTo(UnitAction.Get));
            Assert.That(result.TotalRows, Is.EqualTo(5));
            Assert.That(result.PagedQuery.PageSize, Is.EqualTo(2));
            Assert.That(result.PagedQuery.PageIndex, Is.EqualTo(1));
            Assert.That(result.Meta["totalRows"], Is.EqualTo(5));
            Assert.That(result.Meta["totalPages"], Is.EqualTo(3));
            Assert.That(customers, Has.Length.EqualTo(2));
            Assert.That(customers?.Select(x => x.Name), Is.EqualTo(new[] { "Linus", "Margaret" }));
        });
    }

    [Test]
    public async Task AddMediatorServicesAndPipelines_returns_failed_paged_unit_result_with_embedded_exception()
    {
        using var services = CreateServiceProvider();
        var mediator = services.GetRequiredService<IMediator>();

        var result = await mediator.Send(new FailingCustomerPagedRequest());
        var customers = result.Result?.ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Action, Is.EqualTo(UnitAction.None));
            Assert.That(result.TotalRows, Is.EqualTo(0));
            Assert.That(result.PagedQuery.PageSize, Is.EqualTo(2));
            Assert.That(result.PagedQuery.PageIndex, Is.EqualTo(1));
            Assert.That(result.Meta["totalRows"], Is.EqualTo(0));
            Assert.That(result.Meta["totalPages"], Is.EqualTo(0));
            Assert.That(customers, Is.Empty);
            Assert.That(result.Exception, Is.TypeOf<InvalidOperationException>());
            Assert.That(result.Exception?.Message, Is.EqualTo("Simulated paged failure"));
            Assert.That(result.FailureReason, Is.EqualTo(FailureReason.Unknown));
        });
    }

    [Test]
    public async Task AddMediatorServicesAndPipelines_converts_unit_result_request_exceptions_to_failed_results()
    {
        using var services = CreateServiceProvider();
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

    private static ServiceProvider CreateServiceProvider()
    {
        return new ServiceCollection()
            .AddLogging()
            .ConfigureExceptionBehaviourManager(_ => { })
            .AddMediatorServicesAndPipelines(typeof(ExtensionTests).Assembly)
            .BuildServiceProvider();
    }
}
