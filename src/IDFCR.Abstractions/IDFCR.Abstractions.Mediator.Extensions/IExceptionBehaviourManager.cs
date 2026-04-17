namespace IDFCR.Abstractions.Mediator.Extensions;

/// <summary>
/// Represents a manager that provides the behavior to be applied when exceptions of specific types are caught during the handling of MediatR requests. This interface defines methods for retrieving the exception behavior for a given exception type, as well as a default exception behavior that can be applied when no specific behavior is defined for a particular exception type. Implementing this interface allows for flexible and consistent handling of exceptions across different types of exceptions in the application.
/// </summary>
public interface IExceptionBehaviourManager
{
    /// <summary>
    /// Gets the configured exception behavior for the specified exception type, if one is defined.
    /// </summary>
    /// <typeparam name="TException">The type of exception for which to retrieve the behavior.</typeparam>
    /// <returns>An <see cref="ExceptionBehaviour"/> value representing the behavior for the specified exception type, or <see
    /// langword="null"/> if no behavior is configured.</returns>
    ExceptionBehaviour? GetExceptionBehaviour<TException>();
    /// <summary>
    /// Gets the default exception behavior to be applied when no specific behavior is defined for a particular exception type. This default behavior can be used as a fallback to ensure that all exceptions are handled in a consistent manner, even if they do not have a specific behavior defined.
    /// </summary>
    ExceptionBehaviour? DefaultExceptionBehaviour { get; }
}
