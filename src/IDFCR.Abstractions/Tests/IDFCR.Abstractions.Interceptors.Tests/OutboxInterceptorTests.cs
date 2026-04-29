using IDFCR.Abstractions.Interceptors.Factories;
using IDFCR.Abstractions.Interceptors.Handlers;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;
using Moq;
using NUnit.Framework;

namespace IDFCR.Abstractions.Interceptors.Tests;

public class OutboxEntity : MapperBase<IOutboxEntity>, IOutboxEntity<Guid>
{
    public Guid Id { get; set; }
    public string? Data { get; set; }
    public DateTimeOffset? CompletedTimestampUtc { get; set; }
    public DateTimeOffset? FailedTimestampUtc { get; set; }
    public DateTimeOffset? AcknowledgedTimestampUtc { get; set; }
    public DateTimeOffset CreatedTimestampUtc { get; set; }
    public DateTimeOffset? ModifiedTimestampUtc { get; set; }

    public override void Map(IOutboxEntity source)
    {
        Data = source.Data;
        CompletedTimestampUtc = source.CompletedTimestampUtc;
        FailedTimestampUtc = source.FailedTimestampUtc;
        AcknowledgedTimestampUtc = source.AcknowledgedTimestampUtc;
        CreatedTimestampUtc = source.CreatedTimestampUtc;
        ModifiedTimestampUtc = source.ModifiedTimestampUtc;
    }
}

internal class MockOutboxEntityNotificationHandler : OutboxEntityNotificationHandlerBase<OutboxEntity, Guid>
{
    public IOutboxEntity? LastMapped { get; private set; }
    public OutboxEntity? LastNotified { get; private set; }
    public Guid? NotifyResult { get; set; }

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

    public override Task<Guid?> NotifyAsync(Guid id, OutboxEntity entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

[TestFixture]
internal class OutboxInterceptorTests
{
    private OutboxInterceptor _interceptor;
    private Mock<IServiceProvider> _serviceProvider;
    private DefaultScopedResources _scopedResources;
    private MockOutboxEntityNotificationHandler _handler;

    [SetUp]
    public void Setup()
    {
        _scopedResources = new();
        
        _handler = new MockOutboxEntityNotificationHandler()
        {
            ScopedResources = _scopedResources
        };

        _serviceProvider = new Mock<IServiceProvider>();

        _serviceProvider
            .Setup(s => s.GetService(typeof(IOutboxEntityNotificationHandler)))
            .Returns(_handler);

        _interceptor = new OutboxInterceptor(_serviceProvider.Object);
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

        var interceptor = new OutboxInterceptor(_serviceProvider.Object);
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

    // ---------------------------------------------------------------------------
    // InterceptAsync — normal flow
    // ---------------------------------------------------------------------------

    [Test]
    public async Task InterceptAsync_WithModel_MapsAndNotifiesHandler()
    {
        var model = new { Name = "test-payload" };
        var ctx = BuildContext(model).Object;

        await _interceptor.InterceptAsync(ctx, CancellationToken.None);

        Assert.That(_handler.LastMapped, Is.Not.Null);
        Assert.That(_handler.LastMapped!.Data, Does.Contain("test-payload"));
        Assert.That(_handler.LastNotified, Is.Not.Null);
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
    }

    [Test]
    public async Task InterceptAsync_WhenHandlerNotRegistered_DoesNotThrow()
    {
        _serviceProvider
            .Setup(s => s.GetService(typeof(IOutboxEntityNotificationHandler)))
            .Returns(() => null);

        var interceptor = new OutboxInterceptor(_serviceProvider.Object);
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
    public async Task InterceptAsync_WithNullContext_SetsScopedResourcesToNull()
    {
        _interceptor.Context = null;

        var ctx = BuildContext(new { Id = 1 }).Object;

        await _interceptor.InterceptAsync(ctx, CancellationToken.None);

        Assert.That(_handler.ScopedResources, Is.Null);
    }

    // ---------------------------------------------------------------------------
    // OrderIndex
    // ---------------------------------------------------------------------------

    [Test]
    public void OrderIndex_IsSetTo99()
    {
        Assert.That(_interceptor.OrderIndex, Is.EqualTo(99));
    }

    [Test]
    public async Task InterceptAsync_WhenHandlerReturnsId_StoresIdentifiableInScopedResources()
    {
        var expectedId = Guid.NewGuid();
        _handler.NotifyResult = expectedId;

        var ctx = BuildContext(new { Id = 1 }).Object;
        var factory = new Mock<IEntityInterceptorFactory>();
        factory.Setup(f => f.ScopedResources).Returns(_scopedResources);

        _interceptor.Context = factory.Object;
        await _interceptor.InterceptAsync(ctx, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_scopedResources.TryGetScopedResource<IIdentifiable<Guid>>(out var identifiable), Is.True);
            Assert.That(identifiable, Is.Not.Null);
            Assert.That(identifiable!.Id, Is.EqualTo(expectedId));
        }
    }

    [Test]
    public async Task InterceptAsync_WhenHandlerReturnsNull_DoesNotStoreIdentifiableInScopedResources()
    {
        _handler.NotifyResult = null;

        var ctx = BuildContext(new { Id = 1 }).Object;

        await _interceptor.InterceptAsync(ctx, CancellationToken.None);

        Assert.That(_scopedResources.TryGetScopedResource<IIdentifiable<Guid>>(out _), Is.False);
    }

    [Test]
    public async Task InterceptAsync_WhenCalledMultipleTimes_ReplacesStoredIdentifiable()
    {
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();

        _handler.NotifyResult = firstId;

        var factory = new Mock<IEntityInterceptorFactory>();
        factory.Setup(f => f.ScopedResources).Returns(_scopedResources);

        _interceptor.Context = factory.Object;

        await _interceptor.InterceptAsync(BuildContext(new { Id = 1 }).Object, CancellationToken.None);

        _handler.NotifyResult = secondId;
        await _interceptor.InterceptAsync(BuildContext(new { Id = 2 }).Object, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_scopedResources.TryGetScopedResource<IIdentifiable<Guid>>(out var identifiable), Is.True);
            Assert.That(identifiable, Is.Not.Null);
            Assert.That(identifiable!.Id, Is.EqualTo(secondId));
        }
    }
}
