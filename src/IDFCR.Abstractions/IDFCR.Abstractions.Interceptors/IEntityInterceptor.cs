namespace IDFCR.Abstractions.Interceptors;

/// <summary>
/// Represents an interceptor that can be applied to specific stages and behaviors of an entity context. This interface defines the contract for implementing custom interceptors that can be executed during the processing of entity operations, such as inserting, updating, or deleting entities. The IEntityInterceptor interface allows developers to specify the conditions under which the interceptor should be applied and to define the logic for handling the interception of entity operations based on the context provided. By implementing this interface, developers can create flexible and reusable interceptors that can be integrated into applications and systems that utilize interception mechanisms for managing entity operations.
/// </summary>
public interface IEntityInterceptor
{
    /// <summary>
    /// Gets an optional index to control the execution order of multiple interceptors. This property can be used to specify the order in which interceptors should be executed when multiple interceptors are applicable to the same stage and behavior of an entity context. Interceptors with lower OrderIndex values will be executed before those with higher values. If OrderIndex is null, the execution order of the interceptor will be determined by the order in which they are registered or discovered within the application or system that utilizes interception mechanisms. By defining this property, developers can ensure that their interceptors are executed in a specific sequence when necessary, allowing for more precise control over the interception logic and its effects on entity operations.
    /// </summary>
    int? OrderIndex { get; }
    /// <summary>
    /// Determines whether the interceptor should be applied to the given entity interceptor context. This method can be overridden by implementing classes to provide custom logic for deciding whether the interceptor should be executed based on the specific context of the entity operation. By default, this method returns true, indicating that the interceptor should be applied to all contexts that match the specified stage and behavior. However, developers can implement additional checks or conditions in their interceptor implementations to control when the interceptor logic is executed within applications and systems that utilize interception mechanisms.
    /// </summary>
    /// <param name="context">The entity interceptor context.</param>
    /// <returns>True if the interceptor should be applied; otherwise, false.</returns>
    bool CanIntercept(IEntityInterceptorContext context);
    /// <summary>
    /// Executes against the given entity interceptor context. This method must be implemented by implementing classes to provide the specific logic for handling the interception of the entity operation based on the context. The context contains information about the stage and behavior of the entity operation, as well as any relevant data or state that may be needed for the interception logic. By implementing this method, developers can define custom behavior for their interceptors that will be executed when the CanIntercept method returns true for a given context within applications and systems that utilize interception mechanisms.
    /// </summary>
    /// <param name="context">The entity interceptor context.</param>
    void Intercept(IEntityInterceptorContext context);
    /// <summary>
    /// Determines asynchronously whether the interceptor can be applied to the given entity interceptor context. This method checks if the context matches the specified stage and behavior, and also considers the result of the CanIntercept method. By default, this method returns a completed task with the result of the CanIntercept method. Developers can override this method to provide custom asynchronous logic for determining if the interceptor can be applied.
    /// </summary>
    /// <param name="context">The entity interceptor context.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if the interceptor can be applied; otherwise, false.</returns>
    Task<bool> CanInterceptAsync(IEntityInterceptorContext context, CancellationToken cancellationToken) => Task.FromResult(CanIntercept(context));

    /// <summary>
    /// Executes asynchronously against the given entity interceptor context. This method must be implemented by implementing classes to provide the specific asynchronous logic for handling the interception of the entity operation based on the context. The context contains information about the stage and behavior of the entity operation, as well as any relevant data or state that may be needed for the interception logic. By implementing this method, developers can define custom asynchronous behavior for their interceptors that will be executed when the CanInterceptAsync method returns true for a given context within applications and systems that utilize interception mechanisms.
    /// </summary>
    /// <param name="context">The entity interceptor context.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InterceptAsync(IEntityInterceptorContext context, CancellationToken cancellationToken)
    {
        Intercept(context);
        return Task.CompletedTask;
    }
}
