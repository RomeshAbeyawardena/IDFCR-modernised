using FluentValidation;
using MediatR;

namespace IDFCR.Abstractions.Mediator.Extensions.Pipelines;

public class ValidationPipeline<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        IValidator<TRequest>[] validatorsArray = [.. validators];

        if (validatorsArray.Length == 0)
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var errors = results.Where(x => !x.IsValid)
            .SelectMany(x => x.Errors);

        if (errors.Any())
        {
            throw new ValidationException("Validation errors have occurred", errors);
        }

        return await next(cancellationToken);
    }
}