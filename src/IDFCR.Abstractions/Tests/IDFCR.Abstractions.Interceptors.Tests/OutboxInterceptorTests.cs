using IDFCR.Abstractions.DependencyInjection;
using IDFCR.Abstractions.Interceptors.Factories;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Outbox;
using IDFCR.Abstractions.Outbox.Handlers;
using IDFCR.Abstractions.Outbox.Interceptors;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace IDFCR.Abstractions.Interceptors.Tests;

public class OutboxEntity : MapperBase<IOutboxEntity>, IOutboxEntity<Guid>
{
    public Guid Id { get; set; }
    public string EntityType { get; set; } = null!;
    public string? Data { get; set; }
    public DateTimeOffset? CompletedTimestampUtc { get; set; }
    public DateTimeOffset? FailedTimestampUtc { get; set; }
    public DateTimeOffset? ProcessedTimestampUtc { get; set; }
    public DateTimeOffset CreatedTimestampUtc { get; set; }
    public DateTimeOffset? ModifiedTimestampUtc { get; set; }

    protected override void MapMembers(IOutboxEntity source)
    {
        Data = source.Data;
        CompletedTimestampUtc = source.CompletedTimestampUtc;
        FailedTimestampUtc = source.FailedTimestampUtc;
        ProcessedTimestampUtc = source.ProcessedTimestampUtc;
        CreatedTimestampUtc = source.CreatedTimestampUtc;
        ModifiedTimestampUtc = source.ModifiedTimestampUtc;
    }
}

internal class MockOutboxEntityNotificationHandler(ILogger logger) : OutboxEntityNotificationHandlerBase<OutboxEntity, Guid>(logger)
{
    public IOutboxEntity? LastMapped { get; private set; }
    public OutboxEntity? LastNotified { get; private set; }
    public OutboxEntity? LastUpdated { get; private set; }
    public Guid? LastUpdateKey { get; private set; }
    public Guid? NotifyResult { get; set; }

    public override Task<Guid?> NotifyFailureAsync(Guid key, OutboxEntity entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    public override IOutboxEntity Map(IOutboxEntity entity)
    {
        var outboxEntity = new OutboxEntity();
        outboxEntity.Map(entity);
        LastMapped = outboxEntity;
        return outboxEntity;
    }

    public override Task<Guid?> NotifyAsync(OutboxEntity entity, CancellationToken cancellationToken)
    {
        LastNotified = entity;
        return Task.FromResult(NotifyResult);
    }

    public override Task<Guid?> UpdateNotificationAsync(Guid key, OutboxEntity entity, CancellationToken cancellationToken)
    {
        LastUpdateKey = key;
        LastUpdated = entity;
        return Task.FromResult<Guid?>(key);
    }
}

[TestFixture]
internal class OutboxInterceptorTests
{
    private OutboxInterceptor _interceptor;
    private Mock<IServiceProvider> _serviceProvider;
    private Mock<ILogger<OutboxInterceptor>> _loggerMock;
    private DefaultScopedResources _scopedResources;
    private MockOutboxEntityNotificationHandler _handler;

    [SetUp]
    public void Setup()
    {
        _scopedResources = new();
        _loggerMock = new();
        _handler = new MockOutboxEntityNotificationHandler(_loggerMock.Object)
        {
            ScopedResources = _scopedResources
        };

        _serviceProvider = new Mock<IServiceProvider>();

        _serviceProvider
            .Setup(s => s.GetService(typeof(IOutboxEntityNotificationHandler)))
            .Returns(_handler);

        _interceptor = new OutboxInterceptor(_serviceProvider.Object, _loggerMock.Object, TimeProvider.System);
    }

    private static Mock<IEntityInterceptorContext> BuildContext(
        object? model = null,
        EntityContextBehaviorStage stage = EntityContextBehaviorStage.Post,
        EntityContextBehavior behavior = EntityContextBehavior.Insert)
    {
        var ctx = new Mock<IEntityInterceptorContext>();
        ctx.Setup(c => c.Stage).Returns(stage);
        ctx.Setup(c => c.Behavior).Returns(behavior);
        ctx.Setup(c => c.Model).Returns(model);
        ctx.Setup(c => c.Data).Returns(new Dictionary<string, object>());
        return ctx;
    }

    // ---------------------------------------------------------------------------
    // ShouldIntercept / CanIntercept
    // ---------------------------------------------------------------------------

    [Test]
    public void ShouldIntercept_WhenHandlerIsRegistered_ReturnsTrue()
    {
        var ctx = BuildContext().Object;
        Assert.That(_interceptor.ShouldIntercept(ctx), Is.True);
    }

    [Test]
    public void ShouldIntercept_WhenHandlerIsNotRegistered_ReturnsFalse()
    {
        _serviceProvider
            .Setup(s => s.GetService(typeof(IOutboxEntityNotificationHandler)))
            .Returns(() => null);   

        var interceptor = new OutboxInterceptor(_serviceProvider.Object, _loggerMock.Object, TimeProvider.System);
        var ctx = BuildContext().Object;

        Assert.That(interceptor.ShouldIntercept(ctx), Is.False);
    }

    [Test]
    public void CanIntercept_PostInsert_ReturnsTrue()
    {
        var ctx = BuildContext(new object(), EntityContextBehaviorStage.Post, EntityContextBehavior.Insert).Object;
        Assert.That(_interceptor.CanIntercept(ctx), Is.True);
    }

    [Test]
    public void CanIntercept_PostUpdate_ReturnsTrue()
    {
        var ctx = BuildContext(new object(), EntityContextBehaviorStage.Post, EntityContextBehavior.Update).Object;
        Assert.That(_interceptor.CanIntercept(ctx), Is.True);
    }

    [Test]
    public void CanIntercept_PreInsert_ReturnsFalse()
    {
        var ctx = BuildContext(new object(), EntityContextBehaviorStage.Pre, EntityContextBehavior.Insert).Object;
        Assert.That(_interceptor.CanIntercept(ctx), Is.False);
    }

    [Test]
    public void CanIntercept_PostDelete_ReturnsFalse()
    {
        var ctx = BuildContext(new object(), EntityContextBehaviorStage.Post, EntityContextBehavior.Delete).Object;
        Assert.That(_interceptor.CanIntercept(ctx), Is.False);
    }

    [Test]
    public async Task InterceptAsync_WithModel_SerializesModelAsJson()
    {
        var model = new { Value = 42 };
        var ctx = BuildContext(model).Object;

        await _interceptor.InterceptAsync(ctx, CancellationToken.None);

        Assert.That(_handler.LastMapped!.Data, Does.Contain("42"));
    }

    [Test]
    public async Task InterceptAsync_WithNullModel_DoesNotCallHandler()
    {
        var ctx = BuildContext(model: null).Object;

        await _interceptor.InterceptAsync(ctx, CancellationToken.None);

        Assert.That(_handler.LastMapped, Is.Null);
        Assert.That(_handler.LastNotified, Is.Null);
        Assert.That(_handler.LastUpdated, Is.Null);
    }

    [Test]
    public async Task InterceptAsync_WithModelAndNoScopedKey_DoesNotUpdateHandler()
    {
        var ctx = BuildContext(new { Name = "test-payload" }).Object;

        await _interceptor.InterceptAsync(ctx, CancellationToken.None);

        Assert.That(_handler.LastMapped, Is.Not.Null);
        Assert.That(_handler.LastUpdated, Is.Null);
    }

    [Test]
    public async Task InterceptAsync_WhenHandlerNotRegistered_DoesNotThrow()
    {
        _serviceProvider
            .Setup(s => s.GetService(typeof(IOutboxEntityNotificationHandler)))
            .Returns(() => null);

        var interceptor = new OutboxInterceptor(_serviceProvider.Object, _loggerMock.Object, TimeProvider.System);
        var ctx = BuildContext(new object()).Object;

        Assert.DoesNotThrowAsync(() => interceptor.InterceptAsync(ctx, CancellationToken.None));
        await Task.CompletedTask;
    }

    // ---------------------------------------------------------------------------
    // ScopedResources propagation
    // ---------------------------------------------------------------------------

    [Test]
    public async Task InterceptAsync_WithModel_PropagatesScopedResourcesToHandler()
    {
        var scopedResources = new Mock<IScopedResources>().Object;
        var factory = new Mock<IEntityInterceptorFactory>();
        factory.Setup(f => f.ScopedResources).Returns(scopedResources);

        _interceptor.Context = factory.Object;

        var ctx = BuildContext(new { Id = 1 }).Object;

        await _interceptor.InterceptAsync(ctx, CancellationToken.None);

        Assert.That(_handler.ScopedResources, Is.SameAs(scopedResources));
    }

    [Test]
    public async Task InterceptAsync_WithNullContext_PreservesExistingScopedResources()
    {
        _interceptor.Context = null;

        var ctx = BuildContext(new { Id = 1 }).Object;

        await _interceptor.InterceptAsync(ctx, CancellationToken.None);

        Assert.That(_handler.ScopedResources, Is.SameAs(_scopedResources));
    }

    // ---------------------------------------------------------------------------
    // OrderIndex
    // ---------------------------------------------------------------------------

    [Test]
    public void OrderIndex_IsSetToIntMaxValue()
    {
        Assert.That(_interceptor.OrderIndex, Is.EqualTo(int.MaxValue));
    }
}
