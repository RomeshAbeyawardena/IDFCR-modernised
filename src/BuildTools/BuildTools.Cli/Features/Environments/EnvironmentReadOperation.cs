using BuildTools.Cli.Common;
using BuildTools.Infrastructure.Features.Environments;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Features.Environments;

[FeatureCommand(EnvironmentRootOperation.Prefix, CommandName)]
public class EnvironmentReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream,
    IEnvironmentRepository environmentRepository)
    : ReadCommandOperationBase<EnvironmentReadOperation>(serviceProvider, managedStream, EnvironmentRootOperation.Prefix,
        CommandName, typeof(EnvironmentRootOperation))
{
    public const string CommandName = "read";

    private string? _name;
    private string? _externalReference;

    protected override async Task AcquireFieldsAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        _name = await this.GetOptionalField(ManagedStream, command, cancellationToken, false, "name");
        _externalReference = await this.GetOptionalField(ManagedStream, command, cancellationToken, false, "external-reference");
    }

    protected override async Task InvokeReadAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_name) && string.IsNullOrWhiteSpace(_externalReference))
        {
            var pagedRequest = new GetPagedEnvironmentQuery
            {
                NameContains = _name,
                ExternalReference = _externalReference
            };

            pagedRequest.MapQuery(PagedQuery);

            var pagedResult = await environmentRepository.GetPagedAsync(pagedRequest, cancellationToken);

            if (pagedResult.HasValue)
                await WritePagedResultAsync(pagedResult, t => t.Map<EnvironmentDto>(), cancellationToken,
                    new TableField<EnvironmentDto> { Field = s => s.ExternalReference, Title = "External Reference", RowWidth = 30 },
                    new TableField<EnvironmentDto> { Field = s => s.Name, Title = "Name", RowWidth = 40 },
                    new TableField<EnvironmentDto> { Field = s => s.DisplayName, Title = "Display Name", RowWidth = 50 });
            return;
        }

        var environmentQuery = new GetPagedEnvironmentQuery
        {
            Name = _name,
            ExternalReference = _externalReference
        };

        var valueResult = await environmentRepository.GetEnvironmentAsync(environmentQuery, cancellationToken);

        if (valueResult.HasValue)
        {
            if (IsJson) { await WriteJsonAsync(valueResult.Result, cancellationToken); return; }
            await ManagedStream.Error.WriteLineAsync("Unsupported format: Please provide a valid output-type", cancellationToken);
            return;
        }

        await ManagedStream.Error.WriteLineAsync($"Unable to read environment: {valueResult.Exception?.Message ?? "Unknown issue"}", cancellationToken);
    }
}