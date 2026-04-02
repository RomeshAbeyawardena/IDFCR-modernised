using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Provides extension methods for <see cref="InjectableCommandOperationBase{T}"/> to facilitate field retrieval and lookup operations in command-line interfaces.
/// </summary>
public static class InjectableCommandOperationBaseExtensions
{
    /// <summary>
    /// Defines an extension method for retrieving a field value from command arguments or parameters, with support for prompting the user if the value is not provided. It returns a <see cref="FieldResult"/> indicating whether the value was successfully retrieved, whether it was a parameter, and the value itself.
    /// </summary>
    /// <typeparam name="T">The type of the injectable command operation.</typeparam>
    /// <param name="operation">The injectable command operation instance.</param>
    /// <param name="stream">The managed stream for input/output operations.</param>
    /// <param name="arguments">The command arguments.</param>
    /// <param name="position">The position of the field in the arguments.</param>
    /// <param name="prompt">The prompt message to display if the field value is not provided.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <param name="wasLastFieldAParameter">Indicates whether the last field was a parameter.</param>
    /// <param name="promptInput">Indicates whether to prompt the user for input if the field value is not provided.</param>
    /// <param name="outputErrors">Indicates whether to output errors if the field value is not provided.</param>
    /// <param name="fields">The names of the fields to retrieve.</param>
    /// <returns>A <see cref="FieldResult"/> indicating the result of the field retrieval.</returns>
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

    /// <summary>
    /// Defines an extension method for retrieving an optional field value from command arguments or parameters. It returns the field value as a string if it exists, or null if it does not. This method is useful for cases where the field is not required and can be omitted without causing an error.
    /// </summary>
    /// <typeparam name="T">The type of the injectable command operation.</typeparam>
    /// <param name="operation">The injectable command operation instance.</param>
    /// <param name="stream">The managed stream for input/output operations.</param>
    /// <param name="arguments">The command arguments.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <param name="wasLastFieldAParameter">Indicates whether the last field was a parameter.</param>
    /// <param name="fields">The names of the fields to retrieve.</param>
    /// <returns>The value of the optional field, or null if it does not exist.</returns>
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

    /// <summary>
    /// Defines a generic extension method for performing a lookup operation based on a field value retrieved from command arguments or parameters. It uses a provided lookup factory function to attempt to retrieve a value of type <typeparamref name="TLookup"/> based on the field value. The method returns a <see cref="FieldResult{TLookup}"/> indicating whether the lookup was successful, whether the field was a parameter, the retrieved value (if any), and any exception that occurred during the lookup process.
    /// </summary>
    /// <typeparam name="T">The type of the injectable command operation.</typeparam>
    /// <typeparam name="TLookup">The type of the value to be looked up.</typeparam>
    /// <param name="operation">The injectable command operation instance.</param>
    /// <param name="stream">The managed stream for input/output operations.</param>
    /// <param name="arguments">The command arguments.</param>
    /// <param name="lookupFactory">The factory function to perform the lookup based on the field value.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <param name="position">The position of the field in the arguments.</param>
    /// <param name="prompt">The prompt message to display if input is required.</param>
    /// <param name="isRequired">Indicates whether the field is required.</param>
    /// <param name="wasLastFieldAParameter">Indicates whether the last field was a parameter.</param>
    /// <param name="fields">The names of the fields to retrieve.</param>
    /// <returns>A <see cref="FieldResult{TLookup}"/> representing the result of the lookup operation.</returns>
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

    /// <summary>
    /// Defines an extension method for retrieving a required field value from command arguments or parameters. It ensures that the field value is provided and not empty, and can optionally prompt the user for input if the value is not initially provided. The method returns a <see cref="FieldResult"/> indicating whether the value was successfully retrieved, whether it was a parameter, and the value itself. If the field is required and not provided, it can also output an error message to the stream.
    /// </summary>
    /// <typeparam name="T">The type of the command operation.</typeparam>
    /// <param name="operation">The command operation instance.</param>
    /// <param name="stream">The managed stream for input/output operations.</param>
    /// <param name="arguments">The command arguments.</param>
    /// <param name="position">The position of the field in the arguments.</param>
    /// <param name="prompt">The prompt message to display if the field value is not provided.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="wasLastFieldAParameter">Indicates whether the last field was a parameter.</param>
    /// <param name="fields">The names of the fields to retrieve.</param>
    /// <returns>A <see cref="FieldResult"/> representing the result of the field retrieval operation.</returns>
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

    /// <summary>
    /// Defines an extension method for retrieving pagination-related fields (such as page index and page size) from command arguments or parameters. It uses the <see cref="GetOptionalField{T}"/> method to attempt to retrieve the page index and page size values, and then constructs a <see cref="PagedQuery"/> object based on the retrieved values. The method also supports overriding the default keys for page index and page size, and ensures that the page size does not exceed a specified maximum value (e.g., 20).
    /// </summary>
    /// <typeparam name="T">The type of the command operation.</typeparam>
    /// <param name="operation">The command operation instance.</param>
    /// <param name="managedStream">The managed stream for input/output operations.</param>
    /// <param name="arguments">The command arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="wasLastFieldAParameter">Indicates whether the last field was a parameter.</param>
    /// <param name="overridePageIndexKey">The key to override the default page index key.</param>
    /// <param name="overridePageSizeKey">The key to override the default page size key.</param>
    /// <returns>A <see cref="PagedQuery"/> object representing the pagination information.</returns>
    public static async Task<PagedQuery> GetPagingFields<T>(this InjectableCommandOperationBase<T> operation, IManagedStream managedStream,
        IEnumerable<string> arguments,
        CancellationToken cancellationToken,
        bool wasLastFieldAParameter = false,
        string? overridePageIndexKey = null,
        string? overridePageSizeKey = null)
        where T : IInjectableCommandOperation
    {
        var pageIndex = await operation.GetOptionalField(managedStream, arguments, cancellationToken, wasLastFieldAParameter, overridePageIndexKey ?? "page-index");
        var pageSize = await operation.GetOptionalField(managedStream, arguments, cancellationToken, wasLastFieldAParameter, overridePageIndexKey ?? "page-size");

        var pagedQuery = new PagedQuery();

        if (!string.IsNullOrWhiteSpace(pageIndex) && int.TryParse(pageIndex, out var index) && index > -1)
        {
            pagedQuery.PageIndex = index;
        }

        if (!string.IsNullOrWhiteSpace(pageSize) && int.TryParse(pageSize, out var size) && size > -1)
        {
            pagedQuery.PageSize = size > 20 ? 20 : size;
        }

        return pagedQuery;
    }
}
