using IDFCR.Abstractions.Results;
using MediatR;

namespace IDFCR.Abstractions.Mediator.Extensions;

/// <summary>
/// Represents a request handler for requests that return unit results. This interface extends the MediatR IRequestHandler interface, allowing you to handle requests that indicate the success or failure of an operation without returning any data. By implementing this interface, you can create handlers that process specific types of requests and return standardized unit results, which can be useful for scenarios where you want to indicate the outcome of an operation without providing additional information. The generic version of this interface allows you to handle requests that return unit results with associated data, providing flexibility in how you structure your request handlers based on the needs of your application.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled.</typeparam>
public interface IUnitResultRequestHandler<TRequest> : IRequestHandler<TRequest, IUnitResult>
    where TRequest : IUnitResultRequest
{
}

/// <summary>
/// Represents a request handler for requests that return unit results with associated data. This interface extends the MediatR IRequestHandler interface, allowing you to handle requests that indicate the success or failure of an operation along with the associated data. By implementing this interface, you can create handlers that process specific types of requests and return standardized unit results with values, which can be useful for scenarios where you want to indicate the outcome of an operation while also providing relevant data. The generic version of this interface provides flexibility in how you structure your request handlers based on the needs of your application, allowing you to work with different types of data while maintaining a consistent approach to handling unit results.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the request.</typeparam>
public interface IUnitResultRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, IUnitResult<TResponse>>
    where TRequest : IUnitResultRequest<TResponse>
{
}
