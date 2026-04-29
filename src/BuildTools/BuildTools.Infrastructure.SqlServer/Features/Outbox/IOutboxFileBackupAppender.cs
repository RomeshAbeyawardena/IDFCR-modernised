namespace BuildTools.Infrastructure.SqlServer.Features.Outbox;

public interface IOutboxFileBackupAppender
{
    Task AppendAsync(OutboxEntity entity, CancellationToken cancellationToken);
}