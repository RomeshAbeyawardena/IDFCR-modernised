using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Interceptors.Handlers;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;

namespace BuildTools.Infrastructure.SqlServer.Features.Outbox;

public class OutboxEntity : MapperBase<IOutboxEntity>, IIdentifiable<Guid>, IOutboxEntity<Guid>
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

public class OutboxEntityNotificationHandler : OutboxEntityNotificationHandlerBase<OutboxEntity, Guid>
{
    public override IOutboxEntity Map(IOutboxEntity entity)
    {
        var outbox = new OutboxEntity();

        outbox.Map(entity);

        return outbox;
    }

    public override Task<Guid?> NotifyAsync(Guid id, OutboxEntity entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<Guid?> NotifyAsync(OutboxEntity entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
