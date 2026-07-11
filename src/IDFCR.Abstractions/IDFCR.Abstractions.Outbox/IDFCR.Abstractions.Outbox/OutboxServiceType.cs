namespace IDFCR.Abstractions.Outbox;

/// <summary>
/// Defines the types of services that can be used in the outbox pattern for message processing.
/// </summary>
public enum OutboxServiceType
{
    /// <summary>
    /// Represents an unknown or unspecified service type. This value can be used as a default or fallback when the specific service type is not determined.
    /// </summary>
    Unknown,
    /// <summary>
    /// Represents a dispatcher service type, which is responsible for dispatching messages from the outbox to their intended recipients. This service type is typically used to manage the flow of messages and ensure they are delivered to the appropriate handlers or endpoints.
    /// </summary>
    Dispatcher,
    /// <summary>
    /// Represents a pipeline service type, which is responsible for processing messages through a series of steps or stages in a defined order. This service type is typically used to implement complex message processing workflows, where each stage in the pipeline can perform specific operations on the messages before they are dispatched or published.
    /// </summary>
    Pipeline,
    /// <summary>
    /// Represents a publisher service type, which is responsible for publishing messages to external systems or message brokers. This service type is typically used to facilitate the delivery of messages to subscribers or consumers that are interested in receiving specific types of messages.
    /// </summary>
    Publisher,
    /// <summary>
    /// Represents a reader service type, which is responsible for reading messages from the outbox and making them available for processing. This service type is typically used to retrieve messages that have been stored in the outbox, allowing other services or components to access and process them as needed.
    /// </summary>
    Reader
}
