using IDFCR.Abstractions.Results;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.Mediator.Extensions.Pipelines;

/// <summary>
/// Represents a generic default exception handling pipeline for MediatR requests that return unit results. This pipeline is designed to catch exceptions of a specified type and convert them into a standardized unit result format, which indicates the failure of the operation along with the appropriate action and failure reason. The behavior for handling exceptions can be customized by implementing the IExceptionBehaviourManager interface, allowing for flexible and consistent error handling across different types of exceptions.
/// </summary>
/// <typeparam name="TRequest">The type of the request being processed.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the request.</typeparam>
/// <typeparam name="TException">The type of exception to be handled by this pipeline.</typeparam>
/// <param name="exceptionBehaviourManager">The exception behaviour manager used to determine how exceptions should be handled.</param>
/// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
public sealed class GenericDefaultExceptionPipeline<TRequest, TResponse, TException>(
    IExceptionBehaviourManager exceptionBehaviourManager, IServiceProvider serviceProvider)
    : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : notnull
    where TException : Exception
{
    const string GenericExceptionMessage = "Unable to find generic method";
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

        var saferExceptionProvider = serviceProvider.GetService<ISaferExceptionProvider>(); // we don't care if this is registered, if it isn't it will just expose the raw exceptions
        Exception finalException = exception; //will be either the converted to a safer exception OR just expose the raw exception
        SaferException? _saferException = null;

        if (saferExceptionProvider is not null && saferExceptionProvider.TryGetImplementation(exception, out var saferException))
        {
            finalException = (Exception?)saferException ?? exception;
            _saferException = saferException;
        }

        TResponse? result = default;
        if (genericTypes.Length == 1)
        {

            var emptyMethodInfo = typeof(Enumerable).GetMethod(nameof(Enumerable.Empty))?.MakeGenericMethod(genericTypes);
            var emptyArray = emptyMethodInfo?.Invoke(null, []);

            if (responseType.IsAssignableTo(typeof(IPagedUnitResult<>).MakeGenericType(genericTypes)))
            {
                var methodInfo = typeof(PagedUnitResult).GetMethod(nameof(PagedUnitResult.FromResult))?
                    .MakeGenericMethod(genericTypes) ?? throw new InvalidOperationException($"{GenericExceptionMessage}");

                IPagedQuery? pagedQuery = null;

                if (request is IPagedQuery paged)
                {
                    pagedQuery = paged;
                }

                result = (TResponse)methodInfo.Invoke(null, [emptyArray, 0, pagedQuery, behaviour.UnitAction, false, finalException, _saferException?.FailureReason ?? behaviour.FailureReason, string.Empty])!;

                state.SetHandled(result);

                return Task.CompletedTask;
            }
            else if (responseType.IsAssignableTo(typeof(IUnitResultCollection<>).MakeGenericType(genericTypes)))
            {
                var methodInfo = typeof(UnitResultCollection).GetMethod(nameof(UnitResultCollection.Failed))?
                    .MakeGenericMethod(genericTypes) ?? throw new InvalidOperationException($"{GenericExceptionMessage}");

                result = (TResponse)methodInfo.Invoke(null, [finalException, behaviour.UnitAction])!;

                state.SetHandled(result);

                return Task.CompletedTask;
            }
            else if (responseType.IsAssignableTo(typeof(IUnitResult<>).MakeGenericType(genericTypes)))
            {
                var methodInfo = typeof(UnitResult).GetMethods()
                    .FirstOrDefault(x => x.IsGenericMethod && x.Name.StartsWith(nameof(UnitResult.Failed)))?
                    .MakeGenericMethod(genericTypes) ?? throw new InvalidOperationException($"{GenericExceptionMessage}");

                result = (TResponse)methodInfo.Invoke(null, [finalException, behaviour.UnitAction, _saferException?.FailureReason ?? behaviour.FailureReason, FailureOrigin.Internal, string.Empty])!;

                state.SetHandled(result);

                return Task.CompletedTask;
            }
        }
        else if (responseType.IsAssignableTo(typeof(IUnitResult)))
        {
            var methodInfo = typeof(UnitResult).GetMethods()
                    .FirstOrDefault(x => !x.IsGenericMethod && x.Name.StartsWith(nameof(UnitResult.Failed)))
                    ?? throw new InvalidOperationException($"{GenericExceptionMessage}");

            result = (TResponse)methodInfo.Invoke(null, [finalException, behaviour.UnitAction, _saferException?.FailureReason ?? behaviour.FailureReason, FailureOrigin.Internal])!;

            state.SetHandled(result);

            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}
