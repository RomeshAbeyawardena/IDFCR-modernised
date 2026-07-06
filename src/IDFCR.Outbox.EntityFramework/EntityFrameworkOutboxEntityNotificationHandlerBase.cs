using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Outbox;
using IDFCR.Abstractions.Outbox.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IDFCR.Outbox.EntityFramework;

/// <summary>
/// Represents an opinionated overrideable base class for handling outbox entity notifications using Entity Framework.
/// </summary>
/// <typeparam name="TDbContext">The type of the Entity Framework DbContext.</typeparam>
/// <typeparam name="TEntity">The type of the outbox entity.</typeparam>
/// <typeparam name="TKey">The type of the outbox entity key.</typeparam>
/// <param name="usesIdentityGeneration">Indicates whether the entity uses identity generation for its key.</param>
/// <param name="logger">The logger instance.</param>
public abstract class EntityFrameworkOutboxEntityNotificationHandlerBase<TDbContext, TEntity, TKey>(ILogger logger, bool usesIdentityGeneration = true)
    : OutboxEntityNotificationHandlerBase<TEntity, TKey>(logger)
    where TDbContext : DbContext
    where TEntity : class, IMapper<IOutboxEntity>, IOutboxEntity<TKey>, new()
    where TKey : struct
{
    /// <summary>
    /// Sets additional fields from the source entity to the target entity. This method can be overridden in derived classes to customize the behavior.
    /// </summary>
    /// <param name="target">The target entity to update.</param>
    /// <param name="source">The source entity to copy values from.</param>
    protected virtual void SetAdditionalFields(TEntity target, TEntity source)
    {
        
    }

    /// <summary>
    /// Gets the DbSet for the outbox entity from the provided DbContext.
    /// </summary>
    /// <param name="context">The DbContext instance.</param>
    /// <returns>The DbSet for the outbox entity.</returns>
    protected virtual DbSet<TEntity> GetOutboxEntity(TDbContext context)
    {
        return context.Set<TEntity>();
    }

    /// <inheritdoc />
    public override IOutboxEntity Map(IOutboxEntity entity)
    {
        var outboxEntity = new TEntity();
        outboxEntity.Map(entity);
        return outboxEntity;
    }

    /// <summary>
    /// Generates a new ID for the outbox entity. This method should be implemented by derived classes to provide a specific ID generation strategy.
    /// </summary>
    /// <returns></returns>
    protected virtual TKey GenerateId()
    {
        if (typeof(TKey) == typeof(Guid))
        {
            return (TKey)(object)Guid.NewGuid();
        }

        throw new NotImplementedException("ID generation not implemented for the specified key type.");
    }

    /// <inheritdoc />
    public override async Task<TKey?> NotifyAsync(TEntity entity, CancellationToken cancellationToken)
    {
        if (!(ScopedResources?.TryGetScopedResource<TDbContext>(out var context) ?? false))
        {
            LogMethod(LogLevel.Warning, "Scoped DbContext not found. Unable to notify outbox item.");
            return null;
        }

        
        if (usesIdentityGeneration && EqualityComparer<TKey>.Default.Equals(entity.Id, default))
        {
            LogMethod(LogLevel.Information, "Entity ID is default. Generating new ID for outbox item.");
            entity.Id = GenerateId();
        }

        var outboxEntitySet = GetOutboxEntity(context);

        await outboxEntitySet.AddAsync(entity, cancellationToken);
        LogMethod(LogLevel.Debug,
            $"Created outbox notification '{entity.Id}'.");
        return entity.Id;
    }

    /// <inheritdoc />
    public override async Task<TKey?> UpdateNotificationAsync(TKey key, TEntity entity, CancellationToken cancellationToken)
    {
        if (!(ScopedResources?.TryGetScopedResource<TDbContext>(out var context) ?? false))
        {
            LogMethod(LogLevel.Warning, "Scoped DbContext not found. Unable to notify outbox item.");
            return null;
        }

        var outboxEntitySet = GetOutboxEntity(context);

        var foundEntity = await outboxEntitySet.FindAsync([key], cancellationToken);

        if (foundEntity is null)
        {
            LogMethod(LogLevel.Warning, $"Outbox item '{key}' not found. Unable to update notification.");
            return null;
        }

        SetMetaData(foundEntity, entity);
        SetAdditionalFields(foundEntity, entity);

        LogMethod(LogLevel.Debug,
            $"Updated outbox notification '{foundEntity.Id}'.");

        return foundEntity?.Id;
    }
}
