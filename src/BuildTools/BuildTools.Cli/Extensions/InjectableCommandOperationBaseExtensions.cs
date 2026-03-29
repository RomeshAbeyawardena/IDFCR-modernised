using BuildTools.Cli;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using IDFCR.Abstractions.Results;
using System.Diagnostics.CodeAnalysis;

namespace BuildTools.Cli.Extensions;

public static class InjectableCommandOperationBaseExtensions
{
    /// <summary>
    /// Searches the dictionary for the first parameter whose key matches any of the specified values.
    /// </summary>
    /// <param name="parameters">The dictionary of parameters to search, where each key is a parameter name and each value is a corresponding
    /// parameter object. Cannot be null.</param>
    /// <param name="values">The collection of parameter names to search for in the dictionary. The method returns the first matching
    /// parameter found, if any.</param>
    /// <returns>The first matching parameter from the dictionary if a key matches any value in the collection; otherwise, null.</returns>
    private static Parameter? SearchParameters(this IDictionary<string, Parameter> parameters, IEnumerable<string> values)
    {
        if (!values.Any(parameters.ContainsKey))
        {
            return null;
        }

        foreach(var value in values)
        {
            if (parameters.TryGetValue(value, out var parameter))
            {
                return parameter;
            }
        }

        return null;
    }

    public static async Task<FieldResult> GetField<T>(
        this InjectableCommandOperationBase<T> operation,
        IManagedStream stream,
        IDictionary<string, Parameter> arguments,
        int position,
        string prompt,
        CancellationToken cancellationToken,
        bool wasLastFieldAParameter = false,
        bool promptInput = false,
        bool outputErrors = false,
        params string[] fields)
        where T : IInjectableCommandOperation
    {
        
        var field = 
            position == Fields.IgnorePositionalParameter 
            ? arguments.SearchParameters(fields)
            : arguments.Values.ElementAtOrDefault(position);

        bool hasValue = field is not null && !field.IsFlag;
        bool isParameter = false;
        if (hasValue && !string.IsNullOrEmpty(field!.Value))
        {
            return new(true, isParameter, field.Value);
        }

        if (outputErrors)
        {
            await stream.Error.WriteLineAsync($"{prompt} is not specified", cancellationToken);
        }

        return new(false, isParameter, null);
    }

    public static async Task<string?> GetOptionalField<T>(
        this InjectableCommandOperationBase<T> operation,
        IManagedStream stream,
        IDictionary<string, Parameter> arguments,
        CancellationToken cancellationToken,
        bool wasLastFieldAParameter = false,
        params string[] fields)
        where T : IInjectableCommandOperation
    {
        return (await GetField(operation, stream, arguments, Fields.IgnorePositionalParameter, string.Empty, cancellationToken, wasLastFieldAParameter, false, false, fields)).Value;
    }


    public static async Task<FieldResult<TLookup>> LookUpFieldAsync<T, TLookup>(this InjectableCommandOperationBase<T> operation,
        IManagedStream stream,
        IDictionary<string, Parameter> arguments,
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
        IDictionary<string, Parameter> arguments,
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
