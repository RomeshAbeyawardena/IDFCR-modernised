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
    /// Notifies the handler of changes to an outbox entity, allowing for the processing of outbox messages and the tracking of their status. This method is responsible for handling notifications related to outbox entities, enabling developers to implement custom logic for processing outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// <param name="entity">The outbox entity that has changed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<TKey?> NotifyAsync(TEntity entity, CancellationToken cancellationToken);
}

/// <summary>
/// Represents an interface for handling notifications related to outbox entities, allowing for the processing of outbox messages and the tracking of their status. This interface defines a contract for implementing custom notification handlers that can process notifications related to outbox entities based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status. The IOutboxEntityNotificationHandler interface provides a method for handling notifications related to outbox entities, enabling developers to implement custom logic for processing outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
/// </summary>
public interface IOutboxEntityNotificationHandler
{
    /// <summary>
    /// Converts the provided outbox entity to a specific type or format, allowing for the processing of outbox messages and the tracking of their status. This method is responsible for mapping the given outbox entity to a specific type or format that can be used for further processing or handling of outbox messages based on specific requirements and use cases related to message processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status. By implementing this method, developers can ensure that the outbox entity is properly transformed or mapped to a format that is suitable for processing and tracking within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// <param name="entity">The outbox entity to be mapped.</param>
    /// <returns>The mapped outbox entity.</returns>
    IOutboxEntity Map(IOutboxEntity entity);
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
    Task<object?> NotifyAsync(object entity, CancellationToken cancellationToken);
}
