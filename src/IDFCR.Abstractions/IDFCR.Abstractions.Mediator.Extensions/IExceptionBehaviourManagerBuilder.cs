namespace IDFCR.Abstractions.Mediator.Extensions;

/// <summary>
/// Represents a builder for configuring and creating an instance of IExceptionBehaviourManager, which is responsible for managing the behavior of exceptions in a MediatR request handling pipeline. This builder allows you to specify how different types of exceptions should be handled by associating them with specific ExceptionBehaviour configurations. You can also set a default exception behavior that will be used when no specific behavior is defined for a particular exception type. Once the desired configurations are set, the Build method can be called to create an instance of IExceptionBehaviourManager that can be used in the exception handling pipeline.
/// </summary>
public interface IExceptionBehaviourManagerBuilder
{
    /// <summary>
    /// Sets the exception behavior for a specific type of exception. This method allows you to define how exceptions of type TException should be handled by associating them with a specific ExceptionBehaviour configuration. The ExceptionBehaviour typically includes information about the action to take when the exception occurs and the reason for the failure. By calling this method, you can customize the handling of different types of exceptions in your MediatR request processing pipeline.
    /// </summary>
    /// <typeparam name="TException">The type of exception for which the behavior is being set.</typeparam>
    /// <param name="exceptionBehaviour">The behavior to apply when the specified exception type is encountered.</param>
    /// <returns>The builder instance for chaining.</returns>
    IExceptionBehaviourManagerBuilder Set<TException>(ExceptionBehaviour exceptionBehaviour);
    /// <summary>
    /// Sets the default exception behavior that will be used when no specific behavior is defined for a particular exception type. This method allows you to specify a fallback ExceptionBehaviour that will be applied to any exceptions that do not have a specific configuration set using the <see cref="Set{TException}(ExceptionBehaviour)"/> method. The default exception behavior typically includes information about the action to take and the reason for the failure, providing a consistent way to handle unexpected exceptions in your MediatR request processing pipeline.
    /// </summary>
    /// <param name="exceptionBehaviour">The default behavior to apply when no specific behavior is defined for a particular exception type.</param>
    /// <returns>The builder instance for chaining.</returns>
    IExceptionBehaviourManagerBuilder SetDefault(ExceptionBehaviour exceptionBehaviour);
    /// <summary>
    /// Builds and returns an instance of IExceptionBehaviourManager based on the configurations that have been set using the <see cref="Set{TException}(ExceptionBehaviour)"/> and <see cref="SetDefault(ExceptionBehaviour)"/> methods. This method finalizes the configuration of the exception behavior manager and creates an instance that can be used in the MediatR request handling pipeline to manage how exceptions are handled according to the defined behaviors. Once this method is called, the resulting IExceptionBehaviourManager will be ready to provide exception handling behavior for the specified exception types and the default behavior as needed.
    /// </summary>
    /// <returns>The configured IExceptionBehaviourManager instance.</returns>
    IExceptionBehaviourManager Build();
}
