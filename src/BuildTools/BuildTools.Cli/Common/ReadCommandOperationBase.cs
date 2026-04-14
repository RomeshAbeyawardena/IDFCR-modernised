using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Common;

/// <summary>
/// Base class for read command operations. Handles the shared acquisition of
/// <c>output-type</c> and paging parameters, and exposes helpers for rendering
/// JSON or paged-table output. Subclasses provide bespoke field acquisition
/// and data retrieval logic only.
/// </summary>
public abstract class ReadCommandOperationBase<T>(
    IServiceProvider serviceProvider,
    IManagedStream managedStream,
    string prefix,
    string commandName,
    Type? memberOfType,
    params string[] aliases)
    : InjectableCommandOperationBase<T>(serviceProvider, prefix, commandName, memberOfType, aliases)
    where T : IInjectableCommandOperation
{
    protected IManagedStream ManagedStream { get; } = managedStream;

    /// <summary>Gets whether the caller requested JSON output.</summary>
    protected bool IsJson { get; private set; }

    /// <summary>Gets the resolved paging parameters.</summary>
    protected PagedQuery PagedQuery { get; private set; } = new();

    /// <summary>
    /// Sealed template: acquires bespoke fields via <see cref="AcquireFieldsAsync"/>,
    /// then resolves the shared <c>output-type</c> and paging parameters,
    /// before delegating to <see cref="InvokeReadAsync"/>.
    /// </summary>
    protected sealed override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        await AcquireFieldsAsync(command, cancellationToken);

        var outputType = await this.GetOptionalField(ManagedStream, command, cancellationToken, fields: "output-type");
        IsJson = outputType == "json";
        PagedQuery = await this.GetPagingFields(ManagedStream, command, cancellationToken);

        await InvokeReadAsync(command, cancellationToken);
    }

    /// <summary>
    /// Acquires operation-specific input fields and stores them as instance state.
    /// Do <b>not</b> acquire <c>output-type</c> or paging fields here — the base handles those.
    /// </summary>
    protected abstract Task AcquireFieldsAsync(IEnumerable<string> command, CancellationToken cancellationToken);

    /// <summary>
    /// Performs data acquisition and output using the already-resolved
    /// <see cref="IsJson"/>, <see cref="PagedQuery"/>, and rendering helpers.
    /// </summary>
    protected abstract Task InvokeReadAsync(IEnumerable<string> command, CancellationToken cancellationToken);

    /// <summary>
    /// Writes a paged result as either a JSON document or a formatted table
    /// depending on <see cref="IsJson"/>.
    /// </summary>
    protected Task WritePagedResultAsync<TSource, TDto>(
        IUnitPagedResult<TSource> result,
        Func<TSource, TDto> map,
        CancellationToken cancellationToken,
        params TableField<TDto>[] fields)
    {
        if (IsJson)
            return ManagedStream.Out.WriteLineAsync(result.Result.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);

        return ManagedStream.DisplayPagedTable(result, map, cancellationToken, fields);
    }

    /// <summary>Serialises <paramref name="value"/> as JSON to the output stream.</summary>
    protected Task WriteJsonAsync<TValue>(TValue value, CancellationToken cancellationToken)
        => ManagedStream.Out.WriteLineAsync(value.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
}