using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Interceptors.Handlers;

internal record DefaultIdentifiable<TKey>() : IIdentifiable<TKey>
    where TKey : struct
{
    public TKey Id { get; set; }
}

/// <summary>
/// Represents a base class for handling notifications related to outbox entities, allowing for the processing of outbox messages and the tracking of their status. This abstract class provides a foundation for creating custom notification handlers by defining common properties and methods that can be overridden by derived classes. The OutboxEntityNotificationHandlerBase class allows developers to specify the entity type and corresponding key type for outbox entities, as well as an implementation of the NotifyAsync method to handle notifications related to outbox entities. By inheriting from this base class, developers
/// </summary>
/// <typeparam name="TEntity">The type of the outbox entity.</typeparam>
/// <typeparam name="TKey">The type of the key for the outbox entity.</typeparam>
public abstract class OutboxEntityNotificationHandlerBase<TEntity, TKey>(IScopedResources scopedResources) : IOutboxEntityNotificationHandler<TEntity, TKey>
    where TEntity : IOutboxEntity<TKey>
    where TKey : struct
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public IScopedResources? ScopedResources { get; set; }

    /// <inheritdoc />
    public abstract IOutboxEntity Map(IOutboxEntity entity);

    /// <summary>
    /// Notifies the handler of changes to the outbox entity, allowing for the processing of outbox messages and the tracking of their status. This method is responsible for handling notifications related to outbox entities, enabling developers to implement custom logic for processing outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// <param name="entity">The outbox entity that has changed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public abstract Task<TKey?> NotifyAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Notifies the handler of changes to the outbox entity, allowing for the processing of outbox messages and the tracking of their status. This method is responsible for handling notifications related to outbox entities, enabling developers to implement custom logic for processing outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status. The implementation of this method checks if the provided entity is of the expected type (TEntity) and if so, it calls the NotifyAsync method with the typed entity. If the entity is not of the expected type, it simply returns a completed task, effectively ignoring notifications for entities that do not match the expected type.
    /// </summary>
    /// <param name="entity">The outbox entity that has changed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<object?> NotifyAsync(object entity, CancellationToken cancellationToken)
    {
        if (entity is TEntity typedEntity)
        {
            var id = await NotifyAsync(typedEntity, cancellationToken);

            if (id.HasValue)
            {
                scopedResources.AddOrUpdate<IIdentifiable<TKey>>(new DefaultIdentifiable<TKey> { Id = id.Value });
            }
        }

        return null;
    }
}
