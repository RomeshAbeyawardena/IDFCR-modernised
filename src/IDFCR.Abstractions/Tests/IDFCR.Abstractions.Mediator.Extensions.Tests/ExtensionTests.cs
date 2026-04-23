using IDFCR.Abstractions.Mediator.Extensions.Pipelines;
using IDFCR.Abstractions.Results;
using MediatR.Pipeline;
using Moq;
using NUnit.Framework;

namespace IDFCR.Abstractions.Mediator.Extensions.Tests;

public sealed record Customer(int Id, string Name);

public sealed class UnitResultRequest : IUnitResultRequest
{
}

public sealed class TypedUnitResultRequest : IUnitResultRequest<Customer>
{
}

public sealed class CollectionUnitResultRequest : IUnitResultCollectionRequest<Customer>
{
}

public sealed record PagedUnitResultRequest() : PagedQuery(2, 1), IUnitPagedResultRequest<Customer>;

public sealed class UnsupportedResponseRequest
{
}

[TestFixture]
public class ExtensionTests
{

    private Mock<IServiceProvider> serviceProviderMock;
    DefaultSaferExceptionProviderBuilder saferExceptionbuilder;

    [SetUp]
    public void SetUp()
    {
        serviceProviderMock = new();
        saferExceptionbuilder = new();
    }

    [Test]
    public async Task GenericDefaultExceptionPipeline_handles_non_generic_unit_results_using_default_behaviour()
    {
        var exception = new InvalidOperationException("boom");
        var state = await Execute<UnitResultRequest, IUnitResult>(new UnitResultRequest(), exception);
        var response = state.Response;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.IsSuccess, Is.False);
            Assert.That(response.Action, Is.EqualTo(UnitAction.None));
            Assert.That(response.FailureReason, Is.EqualTo(FailureReason.Unknown));
            Assert.That(response.Exception, Is.SameAs(exception));
        }
    }

    [Test]
    public async Task GenericDefaultExceptionPipeline_uses_registered_exception_behaviour_instead_of_default_fallback()
    {
        var exception = new InvalidOperationException("boom");
        var defaultBehaviour = new ExceptionBehaviour(UnitAction.Delete, FailureReason.InternalError);
        var registeredBehaviour = new ExceptionBehaviour(UnitAction.Update, FailureReason.Conflict);

        var state = await Execute<TypedUnitResultRequest, IUnitResult<Customer>>(
            new TypedUnitResultRequest(),
            exception,
            registeredBehaviour: registeredBehaviour,
            defaultBehaviour: defaultBehaviour);

        var response = state.Response;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Action, Is.EqualTo(UnitAction.Update));
            Assert.That(response.FailureReason, Is.EqualTo(FailureReason.Conflict));
            Assert.That(response.Action, Is.Not.EqualTo(defaultBehaviour.UnitAction));
            Assert.That(response.FailureReason, Is.Not.EqualTo(defaultBehaviour.FailureReason));
            Assert.That(response.Exception, Is.SameAs(exception));
        }
    }

    [Test]
    public async Task GenericDefaultExceptionPipeline_handles_typed_unit_results_using_registered_exception_behaviour()
    {
        var exception = new InvalidOperationException("boom");
        var behaviour = new ExceptionBehaviour(UnitAction.Update, FailureReason.Conflict);

        var state = await Execute<TypedUnitResultRequest, IUnitResult<Customer>>(
            new TypedUnitResultRequest(),
            exception,
            registeredBehaviour: behaviour);

        var response = state.Response;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.IsSuccess, Is.False);
            Assert.That(response.Action, Is.EqualTo(UnitAction.Update));
            Assert.That(response.FailureReason, Is.EqualTo(FailureReason.Conflict));
            Assert.That(response.Exception, Is.SameAs(exception));
            Assert.That(response.Result, Is.Null);
            Assert.That(response.HasValue, Is.False);
        }
    }

    [Test]
    public async Task GenericDefaultExceptionPipeline_handles_collection_results_and_embeds_the_thrown_exception()
    {
        var exception = new InvalidOperationException("boom");
        var behaviour = new ExceptionBehaviour(UnitAction.Delete, FailureReason.InternalError);

        var state = await Execute<CollectionUnitResultRequest, IUnitResultCollection<Customer>>(
            new CollectionUnitResultRequest(),
            exception,
            registeredBehaviour: behaviour);

        var response = state.Response;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.IsSuccess, Is.False);
            Assert.That(response.Action, Is.EqualTo(UnitAction.Delete));
            Assert.That(response.Exception, Is.SameAs(exception));
            Assert.That(response.Result, Is.Null);
        }
    }

    [Test]
    public async Task GenericDefaultExceptionPipeline_uses_safer_exception_from_provider_when_mapping_exists()
    {
        var exception = new InvalidOperationException("boom");
        var behaviour = new ExceptionBehaviour(UnitAction.Update, FailureReason.Conflict);

        var state = await Execute<TypedUnitResultRequest, IUnitResult<Customer>>(
            new TypedUnitResultRequest(),
            exception,
            registeredBehaviour: behaviour,
            saferExceptionProviderSetup: x => x.AddDefaults());

        var response = state.Response;
        var saferException = response?.Exception as SaferException;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(response, Is.Not.Null);
            Assert.That(saferException, Is.Not.Null);
            Assert.That(saferException!.Message, Is.EqualTo("The operation could not be completed"));
            Assert.That(saferException.InnerException, Is.SameAs(exception));
            Assert.That(response!.FailureReason, Is.EqualTo(FailureReason.InternalError));
            Assert.That(response.Action, Is.EqualTo(UnitAction.Update));
        }
    }

    [Test]
    public async Task GenericDefaultExceptionPipeline_keeps_raw_exception_when_provider_has_no_mapping()
    {
        var exception = new InvalidOperationException("boom");
        var behaviour = new ExceptionBehaviour(UnitAction.Update, FailureReason.Conflict);

        var state = await Execute<TypedUnitResultRequest, IUnitResult<Customer>>(
            new TypedUnitResultRequest(),
            exception,
            registeredBehaviour: behaviour,
            saferExceptionProviderSetup: x => x.AddOrUpdate<NullReferenceException>("sanitised", 500, FailureReason.InternalError));

        var response = state.Response;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Exception, Is.SameAs(exception));
            Assert.That(response.FailureReason, Is.EqualTo(FailureReason.Conflict));
        }
    }

    [Test]
    public async Task GenericDefaultExceptionPipeline_handles_paged_results_with_empty_payload_and_paging_metadata()
    {
        var request = new PagedUnitResultRequest();
        var exception = new InvalidOperationException("boom");
        var behaviour = new ExceptionBehaviour(UnitAction.Conflict, FailureReason.Forbidden);

        var state = await Execute<PagedUnitResultRequest, IUnitPagedResult<Customer>>(
            request,
            exception,
            registeredBehaviour: behaviour);

        var response = state.Response;
        var result = response?.Result?.ToArray();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.IsSuccess, Is.False);
            Assert.That(response.Action, Is.EqualTo(UnitAction.Conflict));
            Assert.That(response.FailureReason, Is.EqualTo(FailureReason.Forbidden));
            Assert.That(response.Exception, Is.SameAs(exception));
            Assert.That(response.TotalRows, Is.EqualTo(0));
            Assert.That(response.PagedQuery, Is.SameAs(request));
            Assert.That(response.PagedQuery.PageSize, Is.EqualTo(2));
            Assert.That(response.PagedQuery.PageIndex, Is.EqualTo(1));
            Assert.That(result, Is.Empty);
            Assert.That(response.Meta["pageSize"], Is.EqualTo(2));
            Assert.That(response.Meta["pageIndex"], Is.EqualTo(1));
            Assert.That(response.Meta["totalRows"], Is.EqualTo(0));
            Assert.That(response.Meta["totalPages"], Is.EqualTo(0));
        }
    }

    [Test]
    public async Task GenericDefaultExceptionPipeline_does_not_handle_unsupported_response_types()
    {
        var exception = new InvalidOperationException("boom");
        var state = await Execute<UnsupportedResponseRequest, string>(new UnsupportedResponseRequest(), exception);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.False);
            Assert.That(state.Response, Is.Null);
        }
    }

    [Test]
    public async Task GenericDefaultExceptionPipeline_consumes_custom_safer_exception_registered_with_builder()
    {
        var exception = new InvalidOperationException("sensitive failure details");
        var behaviour = new ExceptionBehaviour(UnitAction.Update, FailureReason.Conflict);
        const string safeMessage = "A safe error message";

        var state = await Execute<TypedUnitResultRequest, IUnitResult<Customer>>(
            new TypedUnitResultRequest(),
            exception,
            registeredBehaviour: behaviour,
            saferExceptionProviderSetup: x => x.AddOrUpdate<InvalidOperationException>(safeMessage, 418, FailureReason.Forbidden));

        var response = state.Response;
        var saferException = response?.Exception as SaferException;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(response, Is.Not.Null);

            Assert.That(saferException, Is.Not.Null);
            Assert.That(saferException!.Message, Is.EqualTo(safeMessage));
            Assert.That(saferException.InnerException, Is.SameAs(exception));
            Assert.That(saferException.StatusCode, Is.EqualTo(418));
            Assert.That(saferException.FailureReason, Is.EqualTo(FailureReason.Forbidden));

            // proves pipeline consumed safer exception metadata over registered behaviour default
            Assert.That(response!.FailureReason, Is.EqualTo(FailureReason.Forbidden));
            Assert.That(response.Action, Is.EqualTo(UnitAction.Update));

            serviceProviderMock.Verify(x => x.GetService(typeof(ISaferExceptionProvider)), Times.AtLeastOnce);
        }
    }

    private async Task<RequestExceptionHandlerState<TResponse>> Execute<TRequest, TResponse>(
        TRequest request,
        InvalidOperationException exception,
        ExceptionBehaviour? registeredBehaviour = null,
        ExceptionBehaviour? defaultBehaviour = null,
        Action<DefaultSaferExceptionProviderBuilder>? saferExceptionProviderSetup = null)
        where TRequest : notnull
    {
        var builder = new ExceptionBehaviourManagerBuilder();

        if (defaultBehaviour is not null)
        {
            builder.SetDefault(defaultBehaviour);
        }

        if (registeredBehaviour is not null)
        {
            builder.Set<InvalidOperationException>(registeredBehaviour);
        }

        saferExceptionProviderSetup?.Invoke(saferExceptionbuilder);

        serviceProviderMock.Setup(x => x.GetService(typeof(ISaferExceptionProvider)))
            .Returns(saferExceptionbuilder.Build());

        var pipeline = new GenericDefaultExceptionPipeline<TRequest, TResponse, InvalidOperationException>(builder.Build(), serviceProviderMock.Object);
        var state = new RequestExceptionHandlerState<TResponse>();

        await pipeline.Handle(request, exception, state, CancellationToken.None);

        return state;
    }
}
