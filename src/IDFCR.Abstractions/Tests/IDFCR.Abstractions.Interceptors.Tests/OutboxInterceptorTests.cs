using IDFCR.Abstractions.Interceptors.Handlers;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Mapper;
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
    public override IOutboxEntity Map(IOutboxEntity entity)
    {
        var outboxEntity = new OutboxEntity();

        outboxEntity.Map(entity);
        return outboxEntity;
    }

    public override Task<Guid?> NotifyAsync(OutboxEntity entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

[TestFixture]
internal class OutboxInterceptorTests
{
    private OutboxInterceptor _interceptor;
    private Mock<IServiceProvider> _serviceProvider;

    [SetUp]
    public void Setup()
    {
        _serviceProvider = new();

        _serviceProvider.Setup(s => s.GetService(typeof(IOutboxEntityNotificationHandler)))
            .Returns(new MockOutboxEntityNotificationHandler());
        _interceptor = new(_serviceProvider.Object);
    }

    [Test]
    public void Test1()
    {

    }
}
