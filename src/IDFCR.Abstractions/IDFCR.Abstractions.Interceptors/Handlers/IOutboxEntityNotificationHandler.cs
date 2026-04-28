namespace IDFCR.Abstractions.Interceptors.Handlers;

/// <summary>
/// Represents an interface for handling notifications related to outbox entities, allowing for the processing of outbox messages and the tracking of their status. This interface defines a contract for implementing custom notification handlers that can process notifications related to outbox entities based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status. The IOutboxEntityNotificationHandler interface provides a method for handling notifications related to outbox entities, enabling developers to implement custom logic for processing outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
/// </summary>
/// <typeparam name="TEntity">The type of the outbox entity.</typeparam>
/// <typeparam name="TKey">The type of the key for the outbox entity.</typeparam>
public interface IOutboxEntityNotificationHandler<TEntity, TKey> : IOutboxEntityNotificationHandler
    where TEntity : IOutboxEntity<TKey>
    where TKey: struct
{
    /// <summary>
    /// Notifies
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task NotifyAsync(TEntity entity, CancellationToken cancellationToken);
}

/// <summary>
/// Represents an interface for handling notifications related to outbox entities, allowing for the processing of outbox messages and the tracking of their status. This interface defines a contract for implementing custom notification handlers that can process notifications related to outbox entities based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status. The IOutboxEntityNotificationHandler interface provides a method for handling notifications related to outbox entities, enabling developers to implement custom logic for processing outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
/// </summary>
public interface IOutboxEntityNotificationHandler
{
    /// <summary>
    /// Notifies the handler of changes to an outbox entity, allowing for the processing of outbox messages and the tracking of their status. This method is responsible for handling notifications related to outbox entities, enabling developers to implement custom logic for processing outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// <param name="entity">The outbox entity that has changed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task NotifyAsync(object entity, CancellationToken cancellationToken);
}
