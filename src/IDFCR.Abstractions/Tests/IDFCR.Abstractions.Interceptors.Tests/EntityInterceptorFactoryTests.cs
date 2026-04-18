using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Interceptors.Tests.Assets;
using Microsoft.Extensions.Time.Testing;
using NUnit.Framework;

namespace IDFCR.Abstractions.Interceptors.Tests;

[TestFixture]
public class EntityInterceptorFactoryTests
{
    private DefaultEntityInterceptorFactory _entityInterceptorFactory;
    private List<IEntityInterceptor> _entityInterceptorList;
    private FakeTimeProvider _timeProvider;
    [SetUp]
    public void SetUp()
    {
        _timeProvider = new(new DateTimeOffset(2025, 03, 1, 10, 40, 0, TimeSpan.Zero));
        _entityInterceptorList = [
            new AuditCreatedTimestampEntityInterceptor(_timeProvider),
            new AuditModifiedTimestampEntityInterceptor(_timeProvider)];
        _entityInterceptorFactory = new(_entityInterceptorList);
    }

    [Test]
    public async Task GetEntityInterceptorsAsync_PreInsert_ReturnsCreatedTimestampInterceptor()
    {
        var subject = new Customer();
        var context = new TestEntityInterceptContext(EntityContextBehaviorStage.Pre, EntityContextBehavior.Insert, subject);

        var interceptors = (await _entityInterceptorFactory
            .GetEntityInterceptorsAsync(context, CancellationToken.None)).ToArray();

        Assert.That(interceptors, Has.Length.EqualTo(1));
        var interceptor = interceptors.FirstOrDefault();
        Assert.That(interceptor, Is.Not.Null);
        Assert.That(interceptor, Is.AssignableTo<AuditCreatedTimestampEntityInterceptor>());
    }

    [Test]
    public async Task GetEntityInterceptorsAsync_PreUpdate_ReturnsModifiedTimestampInterceptor()
    {
        var subject = new Customer();
        var context = new TestEntityInterceptContext(EntityContextBehaviorStage.Pre, EntityContextBehavior.Update, subject);

        var interceptors = (await _entityInterceptorFactory
            .GetEntityInterceptorsAsync(context, CancellationToken.None)).ToArray();

        Assert.That(interceptors, Has.Length.EqualTo(1));
        var interceptor = interceptors.FirstOrDefault();
        Assert.That(interceptor, Is.Not.Null);
        Assert.That(interceptor, Is.AssignableTo<AuditModifiedTimestampEntityInterceptor>());
    }

    [Test]
    public async Task GetEntityInterceptorsAsync_PreDelete_ReturnsNoInterceptors()
    {
        var subject = new Customer();
        var context = new TestEntityInterceptContext(EntityContextBehaviorStage.Pre, EntityContextBehavior.Delete, subject);

        var interceptors = await _entityInterceptorFactory
            .GetEntityInterceptorsAsync(context, CancellationToken.None);

        Assert.That(interceptors, Is.Empty);
    }

    [Test]
    public async Task InvokeAsync_PreInsert_SetsCreatedTimestamp()
    {
        var subject = new Customer();
        var context = new TestEntityInterceptContext(EntityContextBehaviorStage.Pre, EntityContextBehavior.Insert, subject);

        var interceptors = await _entityInterceptorFactory
            .GetEntityInterceptorsAsync(context, CancellationToken.None);

        await _entityInterceptorFactory.InvokeAsync(interceptors, context, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(subject.CreatedTimestampUtc, Is.EqualTo(new DateTimeOffset(2025, 03, 1, 10, 40, 0, TimeSpan.Zero)));
            Assert.That(subject.ModifiedTimestampUtc, Is.Null);
        }
    }

    [Test]
    public async Task InvokeAsync_PreUpdate_SetsModifiedTimestamp()
    {
        var subject = new Customer();
        var context = new TestEntityInterceptContext(EntityContextBehaviorStage.Pre, EntityContextBehavior.Update, subject);

        var interceptors = await _entityInterceptorFactory
            .GetEntityInterceptorsAsync(context, CancellationToken.None);

        await _entityInterceptorFactory.InvokeAsync(interceptors, context, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(subject.CreatedTimestampUtc, Is.Default);
            Assert.That(subject.ModifiedTimestampUtc, Is.EqualTo(new DateTimeOffset(2025, 03, 1, 10, 40, 0, TimeSpan.Zero)));
        }
    }

    [Test]
    public async Task InvokeAsync_PreInsert_DoesNotOverrideExistingCreatedTimestamp()
    {
        var existingTimestamp = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var subject = new Customer { CreatedTimestampUtc = existingTimestamp };
        var context = new TestEntityInterceptContext(EntityContextBehaviorStage.Pre, EntityContextBehavior.Insert, subject);

        var interceptors = await _entityInterceptorFactory
            .GetEntityInterceptorsAsync(context, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(interceptors, Is.Empty, "Should not return interceptor when timestamp already set");
            Assert.That(subject.CreatedTimestampUtc, Is.EqualTo(existingTimestamp), "Should preserve existing timestamp");
        }
    }

    [Test]
    public async Task InvokeAsync_PreUpdate_AlwaysUpdatesModifiedTimestamp()
    {
        var existingTimestamp = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var subject = new Customer { ModifiedTimestampUtc = existingTimestamp };
        var context = new TestEntityInterceptContext(EntityContextBehaviorStage.Pre, EntityContextBehavior.Update, subject);

        var interceptors = (await _entityInterceptorFactory
            .GetEntityInterceptorsAsync(context, CancellationToken.None)).ToArray();

        await _entityInterceptorFactory.InvokeAsync(interceptors, context, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(interceptors, Has.Length.EqualTo(1), "Should always run for updates to track latest modification");
            Assert.That(subject.ModifiedTimestampUtc, Is.EqualTo(new DateTimeOffset(2025, 03, 1, 10, 40, 0, TimeSpan.Zero)),
                "Should always update to current time to track latest modification, not preserve old value");
        }
    }

    [Test]
    public async Task GetEntityInterceptorsAsync_NullModel_ReturnsNoInterceptors()
    {
        var context = new TestEntityInterceptContext(EntityContextBehaviorStage.Pre, EntityContextBehavior.Insert, null);

        var interceptors = await _entityInterceptorFactory
            .GetEntityInterceptorsAsync(context, CancellationToken.None);

        Assert.That(interceptors, Is.Empty);
    }

    [Test]
    public async Task GetEntityInterceptorsAsync_ModelWithoutAuditInterfaces_ReturnsNoInterceptors()
    {
        var subject = new { Id = 1, Name = "Test" };
        var context = new TestEntityInterceptContext(EntityContextBehaviorStage.Pre, EntityContextBehavior.Insert, subject);

        var interceptors = await _entityInterceptorFactory
            .GetEntityInterceptorsAsync(context, CancellationToken.None);

        Assert.That(interceptors, Is.Empty);
    }

    [Test]
    public async Task GetEntityInterceptorsAsync_PostStage_ReturnsNoInterceptors()
    {
        var subject = new Customer();
        var context = new TestEntityInterceptContext(EntityContextBehaviorStage.Post, EntityContextBehavior.Insert, subject);

        var interceptors = await _entityInterceptorFactory
            .GetEntityInterceptorsAsync(context, CancellationToken.None);

        Assert.That(interceptors, Is.Empty, "Current interceptors only work on Pre stage");
    }
}
