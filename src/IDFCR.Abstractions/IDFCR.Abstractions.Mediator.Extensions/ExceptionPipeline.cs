using IDFCR.Abstractions.Results;
using MediatR.Pipeline;

namespace IDFCR.Abstractions.Mediator.Extensions;

/// <summary>
/// Represents a default exception handling pipeline for MediatR requests that return unit results. This pipeline is designed to catch exceptions of a specified type and convert them into a standardized unit result format, which indicates the failure of the operation along with the appropriate action and failure reason. The behavior for handling exceptions can be customized by implementing the IExceptionBehaviourManager interface, allowing for flexible and consistent error handling across different types of exceptions.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <typeparam name="TException"></typeparam>
public sealed class DefaultExceptionPipeline<TRequest, TResponse, TException>(IExceptionBehaviourManager exceptionBehaviourManager)
    : IRequestExceptionHandler<TRequest, IUnitResult<TResponse>, TException>
        where TRequest : IUnitResultRequest<TResponse>
        where TException : Exception
{
    /// <summary>
    /// Handles exceptions of type TException that occur during the processing of a MediatR request. When an exception is caught, this method retrieves the appropriate exception behavior from the IExceptionBehaviourManager and sets the handled result in the state to indicate the failure of the operation, along with the specified action and failure reason. This allows for a consistent and standardized way to handle exceptions and communicate failure information back to the caller.
    /// </summary>
    /// <param name="request">The request being processed when the exception occurred.</param>
    /// <param name="exception">The exception that was thrown during the processing of the request.</param>
    /// <param name="state">The state object used to set the handled result.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<IUnitResult<TResponse>> state, CancellationToken cancellationToken)
    {
        var exceptionBehaviour = exceptionBehaviourManager.GetExceptionBehaviour<TException>() 
            ?? exceptionBehaviourManager.DefaultExceptionBehaviour
            ?? ExceptionBehaviourManagerBuilder.Default;

        state.SetHandled(UnitResult.Failed<TResponse>(exception, exceptionBehaviour.UnitAction, exceptionBehaviour.FailureReason));
    }
}
