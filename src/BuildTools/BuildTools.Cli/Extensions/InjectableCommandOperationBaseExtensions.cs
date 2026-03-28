using BuildTools.Cli;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using IDFCR.Abstractions.Results;
using System.Diagnostics.CodeAnalysis;

namespace BuildTools.Cli.Extensions;

public static class InjectableCommandOperationBaseExtensions
{
    public static async Task<FieldResult> GetField<T>(
        this InjectableCommandOperationBase<T> operation,
        IManagedStream stream,
        IEnumerable<string> arguments,
        int position,
        string prompt,
        CancellationToken cancellationToken,
        bool wasLastFieldAParameter = false,
        bool promptInput = false,
        bool outputErrors = false,
        params string[] fields)
        where T : IInjectableCommandOperation
    {
        var fieldValue = position == Fields.IgnorePositionalParameter ? string.Empty : arguments.ElementAtOrDefault(wasLastFieldAParameter ? position + 1 : position);
        bool hasValue;
        bool isParameter = false;
        if (string.IsNullOrWhiteSpace(fieldValue) || fieldValue.StartsWith('-'))
        {
            isParameter = !string.IsNullOrWhiteSpace(fieldValue = operation.Parameters?.TryGetRawValue(fields));
            if (!isParameter && promptInput)
            {
                fieldValue = await stream.PromptAsync(prompt, cancellationToken);
            }
            
        }

        hasValue = !string.IsNullOrWhiteSpace(fieldValue);

        if (!hasValue && outputErrors)
        {
            await stream.Error.WriteLineAsync($"{prompt} is not specified", cancellationToken);
        }

        return new(hasValue, isParameter, fieldValue);
    }

    public static async Task<string?> GetOptionalField<T>(
        this InjectableCommandOperationBase<T> operation,
        IManagedStream stream,
        IEnumerable<string> arguments,
        CancellationToken cancellationToken,
        bool wasLastFieldAParameter = false,
        params string[] fields)
        where T : IInjectableCommandOperation
    {
        return (await GetField(operation, stream, arguments, Fields.IgnorePositionalParameter, string.Empty, cancellationToken, wasLastFieldAParameter, false, false, fields)).Value;
    }


    public static async Task<FieldResult<TLookup>> LookUpFieldAsync<T, TLookup>(this InjectableCommandOperationBase<T> operation,
        IManagedStream stream,
        IEnumerable<string> arguments,
        Func<string, CancellationToken, Task<TLookup?>> lookupFactory,
        CancellationToken cancellationToken,
        int? position = null,
        string? prompt = null,
        bool isRequired = false,
        bool wasLastFieldAParameter = false,
        params string[] fields)
        where T : IInjectableCommandOperation
    {
        async Task<FieldResult<TLookup>> LookupValue(string? value, bool hasValue, bool isParameter)
        {
            if (!hasValue || string.IsNullOrWhiteSpace(value))
            {
                return new FieldResult<TLookup>(hasValue, isParameter, default);
            }

            var result = await lookupFactory(value, cancellationToken);

            return result switch
            {
                IUnitResult { IsSuccess: true } => new(true, isParameter, result),
                IUnitResult { Exception: var ex } => new(false, isParameter, result, ex),
                not null => new(true, isParameter, result),
                _ => new(false, isParameter, default)
            };
        }

        if (isRequired)
        {
            var (hasValue, value) = (await GetRequiredField(operation, stream, arguments, position.GetValueOrDefault(Fields.IgnorePositionalParameter), prompt ?? "Lookup value", cancellationToken, wasLastFieldAParameter, fields))
                .AsValueOrDefault(out var isParameter);

            return await LookupValue(value, hasValue, isParameter);
        }

        var optionalValue = await GetOptionalField(operation, stream, arguments, cancellationToken, wasLastFieldAParameter, fields);

        return await LookupValue(optionalValue, !string.IsNullOrWhiteSpace(optionalValue), true);
    }

    public static Task<FieldResult> GetRequiredField<T>(
        this InjectableCommandOperationBase<T> operation, 
        IManagedStream stream, 
        IEnumerable<string> arguments, 
        int position, 
        string prompt,
        CancellationToken cancellationToken,
        bool wasLastFieldAParameter = false,
        params string[] fields)
        where T : IInjectableCommandOperation
    {
        return GetField(operation, stream, arguments, position, prompt, cancellationToken, wasLastFieldAParameter, true, true, fields);
    }
}
