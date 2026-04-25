namespace IDFCR.Abstractions.Interceptors;

/// <summary>
/// Represents a factory for creating and invoking entity interceptors based on the provided context. This interface defines the contract for implementing a factory that can retrieve applicable entity interceptors for a given context and execute them in the appropriate order. The IEntityInterceptorFactory allows developers to create flexible and reusable interceptor factories that can be integrated into applications and systems that utilize interception mechanisms for managing entity operations. By implementing this interface, developers can control the retrieval and execution of entity interceptors based on the specific context of the entity operation being performed, allowing for more precise control over the interception behavior and its effects on the entity operations within applications and systems that utilize interception mechanisms.
/// </summary>
public interface IEntityInterceptorFactory
{
    /// <summary>
    /// Gets a collection of shared context objects that can be used by entity interceptors during their execution. This property provides a way for interceptors to access and share common data or services that may be needed for their logic when processing entity operations. This is to prevent circular dependencies between the factory and the interceptors, allowing for a more flexible and decoupled design. By defining this property, developers can ensure that their interceptors have access to necessary resources or information without creating tight coupling between the factory and the interceptors, enabling better maintainability and extensibility within applications and systems that utilize interception mechanisms for managing entity operations.
    /// </summary>
    IDictionary<Type, object> SharedContextObjects { get; }
    /// <summary>
    /// Defines a method for retrieving applicable entity interceptors based on the provided context. This method should return a collection of entity interceptors that are relevant to the specific stage and behavior of the entity operation being intercepted, as well as any additional context provided in the IEntityInterceptorContext. The retrieved interceptors can then be executed in the appropriate order based on their OrderIndex property or other criteria defined by the implementation. By implementing this method, developers can control which interceptors are applied to specific entity operations within applications and systems that utilize interception mechanisms, allowing for more precise control over the interception behavior and its effects on the entity operations.
    /// </summary>
    /// <param name="context">The entity interceptor context.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of applicable entity interceptors.</returns>
    ValueTask<IEnumerable<IEntityInterceptor>> GetEntityInterceptorsAsync(IEntityInterceptorContext context, CancellationToken cancellationToken);
    /// <summary>
    /// Defines a method for invoking the provided entity interceptors based on the given context. This method should execute the logic of each interceptor in the collection, taking into account their OrderIndex property or other criteria defined by the implementation to determine the execution order. The invocation of the interceptors should be performed in a way that allows for proper handling of the entity operation being intercepted, ensuring that the interception logic is applied correctly based on the specific context of the entity operation. By implementing this method, developers can control how and when the retrieved entity interceptors are executed within applications and systems that utilize interception mechanisms, allowing for more precise control over the interception behavior and its effects on the entity operations.
    /// </summary>
    /// <param name="entityInterceptors">The collection of entity interceptors to be invoked.</param>
    /// <param name="context">The entity interceptor context.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InvokeAsync(IEnumerable<IEntityInterceptor> entityInterceptors, IEntityInterceptorContext context, CancellationToken cancellationToken);
}
