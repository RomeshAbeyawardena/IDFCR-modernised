using IDFCR.Abstractions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IDFCR.Abstractions.Outbox.Handlers;

/// <summary>
/// Represents a base class for handling notifications related to outbox entities, allowing for the processing of outbox messages and the tracking of their status. This abstract class provides a foundation for creating custom notification handlers by defining common properties and methods that can be overridden by derived classes. The OutboxEntityNotificationHandlerBase class allows developers to specify the entity type and corresponding key type for outbox entities, as well as an implementation of the NotifyAsync method to handle notifications related to outbox entities. By inheriting from this base class, developers
/// </summary>
/// <typeparam name="TEntity">The type of the outbox entity.</typeparam>
/// <typeparam name="TKey">The type of the key for the outbox entity.</typeparam>
public abstract class OutboxEntityNotificationHandlerBase<TEntity, TKey>(ILogger logger) : IOutboxEntityNotificationHandler<TEntity, TKey>
    where TEntity : IOutboxEntity<TKey>
    where TKey : struct
{
    /// <summary>
    /// Logs a message with the specified log level and action, allowing for the tracking of events and issues related to outbox entity notifications. This method is responsible for logging messages based on the provided log level and action, enabling developers to implement custom logging logic for handling notifications related to outbox entities. By using this method, developers can ensure that relevant information is logged during the processing of outbox messages and the tracking of their status within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// <param name="logLevel">The log level at which the message should be logged.</param>
    /// <param name="logAction">The action that performs the logging using the provided ILogger instance.</param>
    protected void Log(LogLevel logLevel, Action<ILogger> logAction)
    {
        if (logger.IsEnabled(logLevel))
        {
            logAction(logger);
        }
    }

    /// <summary>
    /// Sets the metadata properties of the target outbox entity based on the source outbox entity. This method is responsible for copying the relevant metadata properties (AcknowledgedTimestampUtc, CompletedTimestampUtc, FailedTimestampUtc, ModifiedTimestampUtc) from the source entity to the target entity, allowing for the tracking of the status and timestamps associated with outbox messages. By using this method, developers can ensure that the metadata properties of outbox entities are properly updated and maintained during processing and notification handling within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// <param name="target">The target outbox entity whose metadata properties will be updated.</param>
    /// <param name="source">The source outbox entity from which metadata properties will be copied.</param>
    protected void SetMetaData(TEntity target, TEntity source)
    {
        target.AcknowledgedTimestampUtc = source.AcknowledgedTimestampUtc;
        target.CompletedTimestampUtc = source.CompletedTimestampUtc;
        target.FailedTimestampUtc = source.FailedTimestampUtc;
        target.ModifiedTimestampUtc = source.ModifiedTimestampUtc;
    }

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
    /// Updates the notification for the specified outbox entity, allowing for the processing of outbox messages and the tracking of their status. This method is responsible for handling updates to notifications related to outbox entities, enabling developers to implement custom logic for processing outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// <param name="key">The key identifying the outbox entity.</param>
    /// <param name="entity">The outbox entity that has changed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public abstract Task<TKey?> UpdateNotificationAsync(
        TKey key, 
        TEntity entity, 
        CancellationToken cancellationToken);

    /// <summary>
    /// Updates the notification for the specified outbox entity, allowing for the processing of outbox messages and the tracking of their status. This method is responsible for handling updates to notifications related to outbox entities, enabling developers to implement custom logic for processing outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status. The implementation of this method checks if the provided entity is of the expected type (TEntity) and if so, it retrieves the key from the scoped resources and calls the UpdateNotificationAsync method with the key and typed entity. If the entity is not of the expected type or if the key cannot be retrieved, it simply returns null, effectively ignoring updates for entities that do not match the expected type or do not have a valid key in the scoped resources.
    /// </summary>
    /// <param name="entity">The outbox entity that has changed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<object?> UpdateNotificationAsync(
        object entity, 
        CancellationToken cancellationToken)
    {
        string reason = entity is null
            ? "Entity is null"
            : $"Unexpected type {entity.GetType().Name}";

        if (entity is TEntity typedEntity)
        {
            if (ScopedResources?.TryGetScopedResource<TKey>(out var key) ?? false)
            {
                return await UpdateNotificationAsync(key, typedEntity, cancellationToken);
            }

            reason = "Key not found!";
        }

        Log(LogLevel.Warning, l => l.LogWarning("{key}: {reason}",
            nameof(UpdateNotificationAsync), reason));

        return null;
    }

    /// <summary>
    /// Notifies the handler of changes to the outbox entity, allowing for the processing of outbox messages and the tracking of their status. This method is responsible for handling notifications related to outbox entities, enabling developers to implement custom logic for processing outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status. The implementation of this method checks if the provided entity is of the expected type (TEntity) and if so, it calls the NotifyAsync method with the typed entity. If the entity is not of the expected type, it simply returns a completed task, effectively ignoring notifications for entities that do not match the expected type.
    /// </summary>
    /// <param name="entity">The outbox entity that has changed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<object?> NotifyAsync(object entity, CancellationToken cancellationToken)
    {
        string reason =
            entity is null
            ? "Entity is null"
            : $"Unexpected type {entity.GetType().Name}";

        if (entity is TEntity typedEntity)
        {
            var id = await NotifyAsync(typedEntity, cancellationToken);
            if (id.HasValue)
            {
                ScopedResources?.AddOrUpdate(id.Value);
                return id;
            }

            reason = "Key is null";
        }

        Log(LogLevel.Warning, l => l.LogWarning("{key}: {reason}",
            nameof(NotifyAsync), reason));

        return null;
    }
}
