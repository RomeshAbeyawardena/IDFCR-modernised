using IDFCR.Abstractions.Interceptors.Factories;

namespace IDFCR.Abstractions.Interceptors.Interceptors;

/// <summary>
/// Represents a base class for implementing entity interceptors that can be applied to specific stages and behaviors of an entity context. This abstract class provides a foundation for creating custom interceptors by defining common properties and methods that can be overridden by derived classes. The EntityInterceptorBase class allows developers to specify the stage and behavior for which the interceptor should be applied, as well as an optional order index to control the execution order of multiple interceptors. By inheriting from this base class, developers
/// </summary>
/// <param name="stage">The stage at which the interceptor should be applied.</param>
/// <param name="behavior">The behavior for which the interceptor should be applied.</param>
/// <param name="orderIndex">An optional index to control the execution order of multiple interceptors.</param>
public abstract class EntityInterceptorBase(EntityContextBehaviorStage stage, EntityContextBehavior behavior, int? orderIndex = null) : IEntityInterceptor
{
    /// <summary>
    /// Defines a helper method for applying an action to a specific type of entity interceptor context. This method checks if the provided context is of the specified type and, if so, applies the given action to it. This allows for type-safe handling of different interceptor contexts without the need for explicit casting in the derived interceptor implementations. By using this method, developers can easily work with specific types of entity interceptor contexts while maintaining clean and readable code in their interceptor implementations.
    /// </summary>
    /// <typeparam name="TEntityInterceptorContext">The type of the entity interceptor context.</typeparam>
    /// <param name="context">The entity interceptor context.</param>
    /// <param name="apply">The action to apply to the context if it is of the specified type.</param>
    protected static void WithContext<TEntityInterceptorContext>(IEntityInterceptorContext context,
        Action<TEntityInterceptorContext> apply)
        where TEntityInterceptorContext : IEntityInterceptorContext
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        if (context is TEntityInterceptorContext interceptorContext)
        {
            apply(interceptorContext);
        }
    }

    /// <summary>
    /// Gets the stage at which the interceptor should be applied. This property is used to determine when the interceptor logic should be executed in relation to the main operation being performed on the entity. The stage can be set to either Pre or Post, indicating whether the interceptor should be applied before or after the main operation, respectively. By defining this property, developers can control the timing of their interceptor logic and ensure that it is executed at the appropriate stage of the entity operation within applications and systems that utilize interception mechanisms.
    /// </summary>
    public int? OrderIndex { get; } = orderIndex;
    
    /// <inheritdoc />
    public IEntityInterceptorFactory? Context { get; set; }

    /// <summary>
    /// Determines whether the interceptor should be applied to the given entity interceptor context. This method can be overridden by derived classes to provide custom logic for deciding whether the interceptor should be executed based on the specific context of the entity operation. By default, this method returns true, indicating that the interceptor should be applied to all contexts that match the specified stage and behavior. However, developers can implement additional checks or conditions in their derived interceptor implementations to control when the interceptor logic is executed within applications and systems that utilize interception mechanisms.
    /// </summary>
    /// <param name="context">The entity interceptor context.</param>
    /// <returns>True if the interceptor should be applied; otherwise, false.</returns>
    public virtual bool ShouldIntercept(IEntityInterceptorContext context) => true;

    /// <summary>
    /// Determines whether the interceptor can be applied to the given entity interceptor context. This method checks if the context matches the specified stage and behavior, and also considers the result of the ShouldIntercept method. By default, this method returns true if the context matches the stage and behavior, and ShouldIntercept returns true. Developers can override this method to provide custom logic for determining if the interceptor can be applied.
    /// </summary>
    /// <param name="context">The entity interceptor context.</param>
    /// <returns>True if the interceptor can be applied; otherwise, false.</returns>
    public virtual bool CanIntercept(IEntityInterceptorContext context)
    {
        return context.Stage == stage
            && behavior.HasFlag(context.Behavior)
            && ShouldIntercept(context);
    }

    /// <summary>
    /// Executes against the given entity interceptor context. This method must be implemented by derived classes to provide the specific logic for handling the interception of the entity operation based on the context. The context contains information about the stage and behavior of the entity operation, as well as any relevant data or state that may be needed for the interception logic. By implementing this method, developers can define custom behavior for their interceptors that will be executed when the CanIntercept method returns true for a given context within applications and systems that utilize interception mechanisms.
    /// </summary>
    /// <param name="context">The entity interceptor context</param>
    public abstract void Intercept(IEntityInterceptorContext context);

    /// <summary>
    /// Executes asynchronously against the given entity interceptor context. This method can be overridden by derived classes to provide asynchronous logic for handling the interception of the entity operation based on the context. By default, this method calls the synchronous Intercept method and returns a completed task. Developers can implement custom asynchronous behavior in their derived interceptor implementations if needed, allowing for more flexible and efficient handling of entity operations within applications and systems that utilize interception mechanisms.
    /// </summary>
    /// <param name="context">The entity interceptor context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task InterceptAsync(IEntityInterceptorContext context, CancellationToken cancellationToken)
    {
        Intercept(context);
        return Task.CompletedTask;
    }
}
