using FluentValidation;
using IDFCR.Utilities.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text;

namespace IDFCR.Abstractions.Mediator.Extensions.Pipelines;

internal class ValidationPipeline;

internal class ValidationPipeline<TRequest, TResponse>(
    ILogger<ValidationPipeline> logger,
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest: notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogMethod(LogLevel.Information, "Validating request of type {RequestType}", args: typeof(TRequest).Name);
        IValidator<TRequest>[] validatorsArray = [.. validators];

        if (validatorsArray.Length == 0)
        {
            logger.LogMethod(LogLevel.Information, "No validators found for request of type {RequestType}", args: typeof(TRequest).Name);
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var errors = results.Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .ToArray();

        if (errors.Length > 0)
        {
            logger.LogMethod(LogLevel.Information, "Validation failed for request of type {RequestType}: Total {count} errors", args: [typeof(TRequest).Name, errors.Length]);

            StringBuilder errorMessageBuilder = new();

            foreach(var error in errors)
            {
                errorMessageBuilder.AppendLine($"\tProperty {error.PropertyName} failed validation. Error: {error.ErrorMessage}");
                logger.LogMethod(LogLevel.Information, "Validation error for request of type {RequestType}: Property {PropertyName} failed validation. Error: {ErrorMessage}", 
                    args: [typeof(TRequest).Name, error.PropertyName, error.ErrorMessage]);
            }

            var newLine = Environment.NewLine;
            throw new ValidationException($"The following validation errors have occurred:{newLine}{errorMessageBuilder}", errors);
        }

        logger.LogMethod(LogLevel.Information, "Validation completed for request of type {RequestType}: No errors found.", args: typeof(TRequest).Name);
        return await next(cancellationToken);
    }
}