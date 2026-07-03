using IDFCR.Abstractions.Mediator.Extensions;
using IDFCR.Abstractions.Mediator.Extensions.Pipelines;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Results;
using MediatR.Pipeline;
using Moq;
using NUnit.Framework;

namespace IDFCR.Abstractions.Mediator.Tests;

sealed record Customer(int Id, string Name);
sealed record PlainRequest;
sealed record PagedRequest() : PagedQuery(10, 2);

[TestFixture]
internal class GenericDefaultExceptionPipelineTests
{
    [Test]
    public async Task Handle_WhenNonGenericUnitResult_UsesGlobalFallbackBehaviour()
    {
        var state = await Execute<PlainRequest, IUnitResult>(
            request: new PlainRequest(),
            exception: new InvalidOperationException("boom"),
            configuredBehaviour: null,
            defaultBehaviour: null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(state.Response, Is.Not.Null);
            Assert.That(state.Response!.IsSuccess, Is.False);
            Assert.That(state.Response.Action, Is.EqualTo(UnitAction.None));
            Assert.That(state.Response.FailureReason, Is.EqualTo(FailureReason.Unknown));
            Assert.That(state.Response.Exception, Is.TypeOf<InvalidOperationException>());
        }
    }

    [Test]
    public async Task Handle_WhenTypedUnitResult_UsesConfiguredBehaviour()
    {
        var behaviour = new ExceptionBehaviour(UnitAction.Update, FailureReason.Conflict);

        var state = await Execute<PlainRequest, IUnitResult<Customer>>(
            request: new PlainRequest(),
            exception: new InvalidOperationException("boom"),
            configuredBehaviour: behaviour);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(state.Response, Is.Not.Null);
            Assert.That(state.Response!.IsSuccess, Is.False);
            Assert.That(state.Response.Action, Is.EqualTo(UnitAction.Update));
            Assert.That(state.Response.FailureReason, Is.EqualTo(FailureReason.Conflict));
            Assert.That(state.Response.Result, Is.Null);
            Assert.That(state.Response.HasValue, Is.False);
        }
    }

    [Test]
    public async Task Handle_WhenCollectionResult_ProducesFailedCollection()
    {
        var behaviour = new ExceptionBehaviour(UnitAction.Delete, FailureReason.InternalError);

        var state = await Execute<PlainRequest, IUnitResultCollection<Customer>>(
            request: new PlainRequest(),
            exception: new InvalidOperationException("boom"),
            configuredBehaviour: behaviour);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(state.Response, Is.Not.Null);
            Assert.That(state.Response!.IsSuccess, Is.False);
            Assert.That(state.Response.Action, Is.EqualTo(UnitAction.Delete));
            Assert.That(state.Response.Result, Is.Null);
            Assert.That(state.Response.Exception, Is.TypeOf<InvalidOperationException>());
        }
    }

    [Test]
    public async Task Handle_WhenPagedResultAndPagedRequest_SetsPagingMetadataAndEmptyResult()
    {
        var behaviour = new ExceptionBehaviour(UnitAction.Get, FailureReason.ValidationError);
        var request = new PagedRequest();

        var state = await Execute<PagedRequest, IPagedUnitResult<Customer>>(
            request: request,
            exception: new InvalidOperationException("boom"),
            configuredBehaviour: behaviour);

        var response = state.Response;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.IsSuccess, Is.False);
            Assert.That(response.Action, Is.EqualTo(UnitAction.Get));
            Assert.That(response.FailureReason, Is.EqualTo(FailureReason.ValidationError));
            Assert.That(response.PagedQuery, Is.SameAs(request));
            Assert.That(response.TotalRows, Is.EqualTo(0));
            Assert.That(response.Result!.ToArray(), Is.Empty);
            Assert.That(response.Meta[Meta.Paging.PageSize], Is.EqualTo(10));
            Assert.That(response.Meta[Meta.Paging.PageIndex], Is.EqualTo(2));
            Assert.That(response.Meta[Meta.Paging.TotalRows], Is.EqualTo(0));
            Assert.That(response.Meta[Meta.Paging.TotalPages], Is.EqualTo(0));
        }
    }

    [Test]
    public async Task Handle_WhenSaferExceptionMappingExists_UsesMappedExceptionAndFailureReason()
    {
        var behaviour = new ExceptionBehaviour(UnitAction.Update, FailureReason.Conflict);
        var original = new InvalidOperationException("sensitive");
        var safer = new SaferException(original, "safe message", 418, FailureReason.Forbidden);

        var saferProvider = new Mock<ISaferExceptionProvider>();
        saferProvider
            .Setup(x => x.TryGetImplementation(It.IsAny<InvalidOperationException>(), out safer))
            .Returns(true);

        var state = await Execute<PlainRequest, IUnitResult>(
            request: new PlainRequest(),
            exception: original,
            configuredBehaviour: behaviour,
            saferExceptionProvider: saferProvider.Object);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(state.Response, Is.Not.Null);
            Assert.That(state.Response!.Exception, Is.SameAs(safer));
            Assert.That(state.Response.FailureReason, Is.EqualTo(FailureReason.Forbidden));
            Assert.That(state.Response.Action, Is.EqualTo(UnitAction.Update));
        }
    }

    [Test]
    public async Task Handle_WhenSpecificBehaviourMissing_UsesManagerDefaultBehaviour()
    {
        var defaultBehaviour = new ExceptionBehaviour(UnitAction.Conflict, FailureReason.Forbidden);

        var state = await Execute<PlainRequest, IUnitResult>(
            request: new PlainRequest(),
            exception: new InvalidOperationException("boom"),
            configuredBehaviour: null,
            defaultBehaviour: defaultBehaviour);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.True);
            Assert.That(state.Response, Is.Not.Null);
            Assert.That(state.Response!.Action, Is.EqualTo(UnitAction.Conflict));
            Assert.That(state.Response.FailureReason, Is.EqualTo(FailureReason.Forbidden));
        }
    }

    [Test]
    public async Task Handle_WhenResponseTypeUnsupported_DoesNotHandle()
    {
        var state = await Execute<PlainRequest, string>(
            request: new PlainRequest(),
            exception: new InvalidOperationException("boom"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Handled, Is.False);
            Assert.That(state.Response, Is.Null);
        }
    }

    private static async Task<RequestExceptionHandlerState<TResponse>> Execute<TRequest, TResponse>(
        TRequest request,
        InvalidOperationException exception,
        ExceptionBehaviour? configuredBehaviour = null,
        ExceptionBehaviour? defaultBehaviour = null,
        ISaferExceptionProvider? saferExceptionProvider = null)
        where TRequest : notnull
    {
        var manager = new Mock<IExceptionBehaviourManager>();
        manager.Setup(x => x.GetExceptionBehaviour<InvalidOperationException>()).Returns(configuredBehaviour);
        manager.SetupGet(x => x.DefaultExceptionBehaviour).Returns(defaultBehaviour);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(ISaferExceptionProvider))).Returns(saferExceptionProvider);

        var sut = new GenericDefaultExceptionPipeline<TRequest, TResponse, InvalidOperationException>(
            manager.Object,
            serviceProvider.Object);

        var state = new RequestExceptionHandlerState<TResponse>();

        await sut.Handle(request, exception, state, CancellationToken.None);

        return state;
    }
}
