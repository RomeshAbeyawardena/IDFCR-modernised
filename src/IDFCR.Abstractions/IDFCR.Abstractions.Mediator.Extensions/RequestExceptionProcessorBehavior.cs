using MediatR.Pipeline;

namespace IDFCR.Abstractions.Mediator.Extensions;

public class DefaultRequestExceptionProcessorBehavior<TRequest, TException>(IServiceProvider serviceProvider) : IRequestExceptionAction<TRequest, TException>
    where TRequest : notnull
    where TException : Exception
{
    public Task Execute(TRequest request, TException exception, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
