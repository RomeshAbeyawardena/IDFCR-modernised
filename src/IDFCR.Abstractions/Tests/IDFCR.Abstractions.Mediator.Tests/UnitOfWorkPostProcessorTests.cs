using IDFCR.Abstractions.DependencyInjection;
using IDFCR.Abstractions.Mediator.Extensions.Pipelines;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Outbox;
using IDFCR.Abstractions.Outbox.Handlers;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;
using MELT;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace IDFCR.Abstractions.Mediator.Tests;

// ── Test doubles ──────────────────────────────────────────────────────────────

file record PlainRequest;

file record UowRequest(bool CommitChanges) : IUnitOfWorkRequest;

file class StubIdentifiable(object? id) : IIdentifiable
{
    public object? Id => id;
}

// ── Tests ─────────────────────────────────────────────────────────────────────

[TestFixture]
internal class UnitOfWorkPostProcessorTests
{
    private Mock<IUnitOfWork> _unitOfWork = null!;
    private Mock<IOutboxEntityNotificationHandler> _outboxHandler = null!;
    private Mock<IScopedResources> _scopedResources = null!;
    private Mock<IServiceProvider> _serviceProvider = null!;
    private ITestLoggerFactory _loggerFactory;
    private ManualTimeProvider _time = null!;

    [SetUp]
    public void SetUp()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);

        _outboxHandler = new Mock<IOutboxEntityNotificationHandler>();
        _outboxHandler.Setup(h => h.Map(It.IsAny<IOutboxEntity>()))
                      .Returns<IOutboxEntity>(e => e);
        _outboxHandler.Setup(h => h.UpdateNotificationAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync((object?)null);

        _scopedResources = new Mock<IScopedResources>();
        _scopedResources
            .SetupGet(s => s.Items)
            .Returns(new Dictionary<Type, object?>());

        _serviceProvider = new Mock<IServiceProvider>();

        _time = new ManualTimeProvider(new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero));
        _loggerFactory = TestLoggerFactory.Create();
    }

    // ── Guard: request not IUnitOfWorkRequest ─────────────────────────────────

    [TearDown]
    public void TearDown()
    {
        _loggerFactory?.Dispose();
    }

    [Test]
    public async Task Process_WhenRequestIsNotUnitOfWorkRequest_DoesNotSaveChanges()
    {
        var sut = BuildSut<PlainRequest, IUnitResult>();
        var response = UnitResult.Success(UnitAction.None);

        await sut.Process(new PlainRequest(), response, CancellationToken.None);

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // ── Guard: CommitChanges = false ──────────────────────────────────────────

    [Test]
    public async Task Process_WhenCommitChangesIsFalse_DoesNotSaveChanges()
    {
        var sut = BuildSut<UowRequest, IUnitResult>();
        var response = UnitResult.Success(UnitAction.None);

        await sut.Process(new UowRequest(CommitChanges: false), response, CancellationToken.None);

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // ── Guard: response not IUnitResult ───────────────────────────────────────

    [Test]
    public async Task Process_WhenResponseIsNotUnitResult_DoesNotSaveChanges()
    {
        var sut = BuildSut<UowRequest, string>();

        await sut.Process(new UowRequest(CommitChanges: true), "not-a-unit-result", CancellationToken.None);

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // ── Guard: IsSuccess = false ──────────────────────────────────────────────

    [Test]
    public async Task Process_WhenResultIsFailure_DoesNotSaveChanges()
    {
        var sut = BuildSut<UowRequest, IUnitResult>();
        var response = UnitResult.Failed(new Exception("oops"), UnitAction.None, FailureReason.Unknown);

        await sut.Process(new UowRequest(CommitChanges: true), response, CancellationToken.None);

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // ── Happy path: saves changes ─────────────────────────────────────────────

    [Test]
    public async Task Process_WhenCommitChangesAndSuccessResult_SavesChanges()
    {
        var sut = BuildSut<UowRequest, IUnitResult>();
        var response = UnitResult.Success(UnitAction.None);

        await sut.Process(new UowRequest(CommitChanges: true), response, CancellationToken.None);

        _unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Exactly(2));
    }

    // ── Outbox: no handler registered → no notification ───────────────────────

    [Test]
    public async Task Process_WhenNoOutboxHandlerRegistered_DoesNotNotify()
    {
        RegisterScopedResources();
        // outbox handler deliberately NOT registered
        var sut = BuildSut<UowRequest, IUnitResult>();

        await sut.Process(new UowRequest(CommitChanges: true),
            UnitResult.Success(UnitAction.None), CancellationToken.None);

        _outboxHandler.Verify(
            h => h.UpdateNotificationAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ── Outbox: no scoped resources registered → no notification ─────────────

    [Test]
    public async Task Process_WhenNoScopedResourcesRegistered_DoesNotNotify()
    {
        RegisterOutboxHandler();
        // scoped resources deliberately NOT registered
        var sut = BuildSut<UowRequest, IUnitResult>();

        await sut.Process(new UowRequest(CommitChanges: true),
            UnitResult.Success(UnitAction.None), CancellationToken.None);

        _outboxHandler.Verify(
            h => h.UpdateNotificationAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ── Outbox: scoped resources present without IIdentifiable still notifies ──

    [Test]
    public async Task Process_WhenScopedResourcesHasNoIdentifiable_StillNotifies()
    {
        RegisterOutboxHandler();
        RegisterScopedResources();

        var sut = BuildSut<UowRequest, IUnitResult>();

        await sut.Process(new UowRequest(CommitChanges: true),
            UnitResult.Success(UnitAction.None), CancellationToken.None);

        _outboxHandler.Verify(
            h => h.UpdateNotificationAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ── Outbox: full success path → completed entity notified ─────────────────

    [Test]
    public async Task Process_WhenSaveSucceeds_NotifiesWithCompletedTimestamps()
    {
        RegisterOutboxHandler();
        RegisterScopedResourcesWithId(new StubIdentifiable(42));

        IOutboxEntity? captured = null;
        _outboxHandler
            .Setup(h => h.UpdateNotificationAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((entity, _) => captured = entity as IOutboxEntity)
            .ReturnsAsync((object?)null);

        var sut = BuildSut<UowRequest, IUnitResult>();

        await sut.Process(new UowRequest(CommitChanges: true),
            UnitResult.Success(UnitAction.None), CancellationToken.None);

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.CompletedTimestampUtc, Is.EqualTo(_time.GetUtcNow()));
        Assert.That(captured.ModifiedTimestampUtc, Is.EqualTo(_time.GetUtcNow()));
        Assert.That(captured.FailedTimestampUtc, Is.Null);
    }

    // ── Outbox: save throws → failed entity notified and exception propagates

    [Test]
    public async Task Process_WhenSaveThrows_NotifiesWithFailedTimestampsAndThrows()
    {
        RegisterOutboxHandler();
        RegisterScopedResourcesWithId(new StubIdentifiable(42));

        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ThrowsAsync(new InvalidOperationException("db exploded"));

        IOutboxEntity? captured = null;
        _outboxHandler
            .Setup(h => h.NotifyFailureAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((entity, _) => captured = entity as IOutboxEntity)
            .ReturnsAsync((object?)null);

        var sut = BuildSut<UowRequest, IUnitResult>();

        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await sut.Process(new UowRequest(CommitChanges: true),
                UnitResult.Success(UnitAction.None), CancellationToken.None));

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.FailedTimestampUtc, Is.EqualTo(_time.GetUtcNow()));
        Assert.That(captured.ModifiedTimestampUtc, Is.EqualTo(_time.GetUtcNow()));
        Assert.That(captured.CompletedTimestampUtc, Is.Null);
    }

    // ── Outbox: save throws, failure notify throws → notify exception propagates

    [Test]
    public async Task Process_WhenSaveThrowsAndFailureNotifyAlsoThrows_NotifyExceptionPropagates()
    {
        RegisterOutboxHandler();
        RegisterScopedResourcesWithId(new StubIdentifiable(1));

        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ThrowsAsync(new InvalidOperationException("db"));
        _outboxHandler
            .Setup(h => h.NotifyFailureAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("notify blew up"));

        var sut = BuildSut<UowRequest, IUnitResult>();

        var ex = Assert.ThrowsAsync<Exception>(
            async () => await sut.Process(new UowRequest(CommitChanges: true),
                UnitResult.Success(UnitAction.None), CancellationToken.None));
    }

    // ── Outbox: success notify is NOT called when save fails ──────────────────

    [Test]
    public async Task Process_WhenSaveThrows_DoesNotCallCompletedNotify()
    {
        RegisterOutboxHandler();
        RegisterScopedResourcesWithId(new StubIdentifiable(1));

        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ThrowsAsync(new Exception("fail"));

        IOutboxEntity? captured = null;
        _outboxHandler
            .Setup(h => h.NotifyFailureAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((entity, _) => captured = entity as IOutboxEntity)
            .ReturnsAsync((object?)null);

        var sut = BuildSut<UowRequest, IUnitResult>();

        var ex = Assert.ThrowsAsync<Exception>(
            async () => await sut.Process(new UowRequest(CommitChanges: true),
                UnitResult.Success(UnitAction.None), CancellationToken.None));

        // The one notify that DID fire must be the failure one
        Assert.That(captured?.CompletedTimestampUtc, Is.Null,
            "Completed timestamp must not be set on a failed save.");
        Assert.That(captured?.FailedTimestampUtc, Is.Not.Null);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private UnitOfWorkPostPipelineProcessor<TReq, TRes> BuildSut<TReq, TRes>()
        where TReq : notnull
        => new(_unitOfWork.Object, _time, _serviceProvider.Object, _loggerFactory.CreateLogger<UnitOfWorkPostPipelineProcessor<TReq,TRes>>());

    private void RegisterOutboxHandler()
        => _serviceProvider
            .Setup(s => s.GetService(typeof(IOutboxEntityNotificationHandler)))
            .Returns(_outboxHandler.Object);

    private void RegisterScopedResources()
        => _serviceProvider
            .Setup(s => s.GetService(typeof(IScopedResources)))
            .Returns(_scopedResources.Object);

    private void RegisterScopedResourcesWithId(IIdentifiable? id)
    {
        RegisterScopedResources();
        _scopedResources.Setup(s => s.TryGetScopedResource(out id))
                        .Returns(true);
    }
}
