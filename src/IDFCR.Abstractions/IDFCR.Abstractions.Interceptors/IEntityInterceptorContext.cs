namespace IDFCR.Abstractions.Interceptors;

/// <summary>
/// Represents the context for an entity interceptor, providing information about the stage and behavior of the entity operation being intercepted, as well as any relevant data or state that may be needed for the interception logic. This interface defines the contract for accessing the context of an entity operation during interception, allowing developers to implement custom logic based on the specific details of the operation being performed. By using this interface, developers can create flexible and reusable interceptors that can be integrated into applications and systems that utilize interception mechanisms for managing entity operations.
/// </summary>
public interface IEntityInterceptorContext
{
    /// <summary>
    /// Gets the stage at which the interceptor is being applied. This property indicates whether the interceptor is being executed before (Pre) or after (Post) the main operation on the entity. By providing this information, developers can implement different logic in their interceptors based on the stage of the entity operation, allowing for more precise control over the interception behavior and its effects on the entity operations within applications and systems that utilize interception mechanisms.
    /// </summary>
    EntityContextBehaviorStage Stage { get; }
    /// <summary>
    /// Gets the behavior of the entity operation being intercepted. This property indicates whether the interceptor is being executed for an insert, update, or delete operation on the entity. By providing this information, developers can implement different logic in their interceptors based on the specific behavior of the entity operation, allowing for more precise control over the interception behavior and its effects on the entity operations within applications and systems that utilize interception mechanisms.
    /// </summary>
    EntityContextBehavior Behavior { get; }
    /// <summary>
    /// Gets an optional model associated with the entity operation being intercepted. This property can be used to provide additional context or data related to the entity operation, which may be needed for the interception logic. The model can contain any relevant information or state that may be useful for the interceptor to make decisions or perform actions based on the specific details of the entity operation. By including this property, developers can enhance the flexibility and functionality of their interceptors by allowing them to access additional context when handling entity operations within applications and systems that utilize interception mechanisms.
    /// </summary>
    object? Model { get; }

    /// <summary>
    /// Gets a read-only dictionary containing additional data or context that may be relevant to the entity operation being intercepted. This property allows developers to provide extra information or state that may be needed for the interception logic, which can be accessed by the interceptors when handling the entity operation. The data in this dictionary can be used to store any relevant details or context that may assist the interceptor in making decisions or performing actions based on the specific circumstances of the entity operation. By including this property, developers can further enhance the flexibility and functionality of their interceptors by allowing them to access a wide range of additional context when managing entity operations within applications and systems that utilize interception mechanisms.
    /// </summary>
    IReadOnlyDictionary<string, object> Data { get; }
}
