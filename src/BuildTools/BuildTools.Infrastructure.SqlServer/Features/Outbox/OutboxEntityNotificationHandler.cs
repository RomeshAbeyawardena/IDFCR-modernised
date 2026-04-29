using IDFCR.Abstractions.Interceptors.Handlers;
using IDFCR.Abstractions.Persistence.Extensions;

namespace BuildTools.Infrastructure.SqlServer.Features.Outbox;

public class OutboxEntityNotificationHandler : OutboxEntityNotificationHandlerBase<OutboxEntity, Guid>
{
    private static async Task<Guid?> UpsertOutboxEntityAsync(PackageManagerDbContext context, OutboxEntity entity, bool commitChanges, Guid? id, CancellationToken cancellationToken)
    {
        var foundEntity = id.HasValue 
            ? await context.OutboxEntities.FindAsync([id.Value], cancellationToken)
            : entity;

        if (id.HasValue && foundEntity is not null)
        {
            foundEntity.Apply(entity);
        }
        else
        {
            //add it anyway, at least it won't be lost!
            var entry = await context.OutboxEntities.AddAsync(entity, cancellationToken);

            id = entry.Property(x => x.Id).CurrentValue;
        }

        if (commitChanges)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        return id;
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

        return await UpsertOutboxEntityAsync(context, entity, true, id, cancellationToken);
    }

    public override async Task<Guid?> NotifyAsync(OutboxEntity entity, CancellationToken cancellationToken)
    {
        if (ScopedResources is null || !ScopedResources.TryGetScopedResource<PackageManagerDbContext>(out var context))
        {
            return null;
        }

        return await UpsertOutboxEntityAsync(context, entity, false, null, cancellationToken);
    }
}
