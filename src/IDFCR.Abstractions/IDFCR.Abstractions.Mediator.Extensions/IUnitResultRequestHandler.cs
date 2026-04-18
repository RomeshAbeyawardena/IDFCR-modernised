using IDFCR.Abstractions.Results;
using MediatR;

namespace IDFCR.Abstractions.Mediator.Extensions;

public interface IUnitResultRequestHandler<TRequest> : IRequestHandler<TRequest, IUnitResult>
    where TRequest : IUnitResultRequest
{
}

public interface IUnitResultRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, IUnitResult<TResponse>>
    where TRequest : IUnitResultRequest<TResponse>
{
}
