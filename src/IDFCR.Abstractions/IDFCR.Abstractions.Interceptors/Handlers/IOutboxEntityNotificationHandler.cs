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
    /// Gets or sets a reference to the scoped resources, allowing for the management and access of resources that are scoped to the lifetime of a specific operation or context. This property provides a way for notification handlers to access and manage resources that are specific to the processing of outbox entity notifications, enabling developers to implement custom logic for handling notifications related to outbox entities based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status. By utilizing scoped resources, developers can ensure that resources are properly managed and disposed of within the context of processing outbox entity notifications, allowing for efficient resource management and improved performance within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    IScopedResources? ScopedResources { get; set; }
    /// <summary>
    /// Notifies the handler of changes to an outbox entity, allowing for the processing of outbox messages and the tracking of their status. This method is responsible for handling notifications related to outbox entities, enabling developers to implement custom logic for processing outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// <param name="entity">The outbox entity that has changed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task NotifyAsync(object entity, CancellationToken cancellationToken);
}
