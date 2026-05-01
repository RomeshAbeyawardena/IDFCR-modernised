using IDFCR.Abstractions.Interceptors.Handlers;
using IDFCR.Abstractions.Persistence.Extensions;
using Microsoft.Extensions.FileProviders;

namespace BuildTools.Infrastructure.SqlServer.Features.Outbox;

public class OutboxEntityNotificationHandler(IOutboxFileBackupAppender backupAppender)
    : OutboxEntityNotificationHandlerBase<OutboxEntity, Guid>
{
    private async Task<Guid?> UpsertOutboxEntityAsync(PackageManagerDbContext context, OutboxEntity entity, bool commitChanges, Guid? id, CancellationToken cancellationToken)
    {
        var foundEntity = id.HasValue
            ? await context.OutboxEntities.FindAsync([id.Value], cancellationToken)
            : entity;

        if (id.HasValue && foundEntity is not null)
        {
            SetMetaData(foundEntity, entity);
        }
        else
        {
            entity.Id = Guid.NewGuid();
            await context.OutboxEntities.AddAsync(entity, cancellationToken);
        }

        if (commitChanges)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        return entity.Id;
    }

    public override IOutboxEntity Map(IOutboxEntity entity)
    {
        var outbox = new OutboxEntity();
        outbox.Map(entity);
        return outbox;
    }

    public override async Task<Guid?> NotifyAsync(Guid id, OutboxEntity entity, CancellationToken cancellationToken)
    {
        if (ScopedResources is null || !ScopedResources.TryGetScopedResource<PackageManagerDbContext>(out var context))
        {
            return null;
        }

        //entity.Id = id;
        await backupAppender.AppendAsync(entity, cancellationToken); // write-ahead durable backup
        return await UpsertOutboxEntityAsync(context, entity, true, id, cancellationToken);
    }

    public override async Task<Guid?> NotifyAsync(OutboxEntity entity, CancellationToken cancellationToken)
    {
        if (ScopedResources is null || !ScopedResources.TryGetScopedResource<PackageManagerDbContext>(out var context))
        {
            return null;
        }

        await backupAppender.AppendAsync(entity, cancellationToken); // write-ahead durable backup
        return await UpsertOutboxEntityAsync(context, entity, false, null, cancellationToken);
    }

    public override Task<object?> NotifyAsync(object id, object entity, CancellationToken cancellationToken)
    {
        return base.NotifyAsync(id, entity, cancellationToken);
    }
}
