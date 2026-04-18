using IDFCR.Abstractions.Results;
using MediatR.Pipeline;

namespace IDFCR.Abstractions.Mediator.Extensions;

/// <summary>
/// Represents a generic default exception handling pipeline for MediatR requests that return unit results. This pipeline is designed to catch exceptions of a specified type and convert them into a standardized unit result format, which indicates the failure of the operation along with the appropriate action and failure reason. The behavior for handling exceptions can be customized by implementing the IExceptionBehaviourManager interface, allowing for flexible and consistent error handling across different types of exceptions.
/// </summary>
/// <typeparam name="TRequest">The type of the request being processed.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the request.</typeparam>
/// <typeparam name="TException">The type of exception to be handled by this pipeline.</typeparam>
/// <param name="exceptionBehaviourManager">The exception behaviour manager used to determine how exceptions should be handled.</param>
public sealed class GenericDefaultExceptionPipeline<TRequest, TResponse, TException>(
    IExceptionBehaviourManager exceptionBehaviourManager)
    : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : notnull
    where TException : Exception
{
    /// <inheritdoc />
    public Task Handle(
        TRequest request,
        TException exception,
        RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        var behaviour = exceptionBehaviourManager.GetExceptionBehaviour<TException>()
            ?? exceptionBehaviourManager.DefaultExceptionBehaviour
            ?? ExceptionBehaviourManagerBuilder.Default;

        var responseType = typeof(TResponse);

        var genericTypes = responseType.GetGenericArguments();

        TResponse? result = default;
        if (genericTypes.Length == 1)
        {

            var emptyMethodInfo = typeof(Enumerable).GetMethod(nameof(Enumerable.Empty))?.MakeGenericMethod(genericTypes);
            var emptyArray = emptyMethodInfo?.Invoke(null, []);

            if (responseType.IsAssignableTo(typeof(IUnitPagedResult<>).MakeGenericType(genericTypes)))
            {
                var methodInfo = typeof(UnitPagedResult).GetMethod(nameof(UnitPagedResult.FromResult))?
                    .MakeGenericMethod(genericTypes) ?? throw new InvalidOperationException($"Unable to find generic method");

                IPagedQuery? pagedQuery = null;

                if (request is IPagedQuery paged)
                {
                    pagedQuery = paged;
                }

                result = (TResponse)methodInfo.Invoke(null, [emptyArray, 0, pagedQuery, behaviour.UnitAction, false, exception, behaviour.FailureReason])!;
                
                state.SetHandled(result);

                return Task.CompletedTask;
            }
            else if (responseType.IsAssignableTo(typeof(IUnitResultCollection<>).MakeGenericType(genericTypes)))
            {
                var methodInfo = typeof(UnitResultCollection).GetMethod(nameof(UnitResultCollection.Failed))?
                    .MakeGenericMethod(genericTypes) ?? throw new InvalidOperationException($"Unable to find generic method");

                result = (TResponse)methodInfo.Invoke(null, [exception, behaviour.UnitAction])!;

                state.SetHandled(result);

                return Task.CompletedTask;
            }
            else if (responseType.IsAssignableTo(typeof(IUnitResult<>).MakeGenericType(genericTypes)))
            {
                var methodInfo = typeof(UnitResult).GetMethods()
                    .FirstOrDefault(x => x.IsGenericMethod && x.Name.StartsWith(nameof(UnitResult.Failed)))?
                    .MakeGenericMethod(genericTypes) ?? throw new InvalidOperationException($"Unable to find generic method");

                result = (TResponse)methodInfo.Invoke(null, [exception, behaviour.UnitAction, behaviour.FailureReason])!;

                state.SetHandled(result);

                return Task.CompletedTask;
            }
        }
        else if (responseType.IsAssignableTo(typeof(IUnitResult)))
        {
            var methodInfo = typeof(UnitResult).GetMethods()
                    .FirstOrDefault(x => !x.IsGenericMethod && x.Name.StartsWith(nameof(UnitResult.Failed)))
                    ?? throw new InvalidOperationException($"Unable to find generic method");

            result = (TResponse)methodInfo.Invoke(null, [exception, behaviour.UnitAction, behaviour.FailureReason])!;

            state.SetHandled(result);

            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}

//public sealed class DefaultPagedExceptionPipeline<TRequest, TValue, TException>
//    : IRequestExceptionHandler<TRequest, IUnitPagedResult<TValue>, TException>
//    where TRequest : IPagedUnitResultRequest<TValue>
//    where TException : Exception
//{
//    public async Task Handle(
//        TRequest request,
//        TException exception,
//        RequestExceptionHandlerState<IUnitPagedResult<TValue>> state,
//        CancellationToken cancellationToken)
//    {
//        var behaviour = exceptionBehaviourManager.GetExceptionBehaviour<TException>()
//            ?? exceptionBehaviourManager.DefaultExceptionBehaviour
//            ?? ExceptionBehaviourManagerBuilder.Default;

//        state.SetHandled(
//            UnitPagedResult.FromResult<TValue>(
//                [],
//                0,
//                request,
//                behaviour.UnitAction,
//                false,
//                exception,
//                behaviour.FailureReason));
//    }
//}
