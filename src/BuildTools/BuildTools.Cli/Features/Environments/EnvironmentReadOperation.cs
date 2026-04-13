using IDFCR.Abstractions.Cli.Operations;
using BuildTools.Infrastructure.Features.Environments;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Features.Environments;

[FeatureCommand(EnvironmentRootOperation.Prefix, CommandName)]
public class EnvironmentReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream,
    IEnvironmentRepository environmentRepository)
    : InjectableCommandOperationBase<EnvironmentReadOperation>(serviceProvider, EnvironmentRootOperation.Prefix,
        CommandName, typeof(EnvironmentRootOperation))
{
    public const string CommandName = "read";

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var name = await this.GetOptionalField(managedStream, command, cancellationToken, false, "name");
        var externalReference = await this.GetOptionalField(managedStream, command, cancellationToken, false, "external-reference");
        var outputType = await this.GetOptionalField(managedStream, command, cancellationToken, true, "output-type");

        var pagedQuery = await this.GetPagingFields(managedStream, command, cancellationToken);

        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(externalReference))
        {
            var pagedRequest = new GetPagedEnvironmentQuery
            {
                NameContains = name,
                ExternalReference = externalReference
            };

            pagedRequest.MapQuery(pagedQuery);

            var pagedResult = await environmentRepository.GetPagedAsync(pagedRequest, cancellationToken);

            if (pagedResult.HasValue)
            {
                if (outputType == "json")
                {
                    await managedStream.Out.WriteLineAsync(pagedResult.Result.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
                    return;
                }

                await managedStream.DisplayPagedTable(pagedResult, t => t.Map<EnvironmentDto>(), cancellationToken,
                new TableField<EnvironmentDto> { Field = s => s.ExternalReference, Title = "External Reference", RowWidth = 30 },
                new TableField<EnvironmentDto> { Field = s => s.Name, Title = "Name", RowWidth = 40 },
                new TableField<EnvironmentDto>
                {
                    Field = s => s.DisplayName,
                    Title = "Display Name",
                    RowWidth = 50
                }
                );
            }

            return;
        }

        var environmentQuery = new GetPagedEnvironmentQuery
        {
            Name = name,
            ExternalReference = externalReference
        };

        var valueResult = await environmentRepository.GetEnvironmentAsync(environmentQuery, cancellationToken);

        if (valueResult.HasValue)
        {
            if (outputType == "json")
            {
                await managedStream.Out.WriteLineAsync(
                    valueResult.Result.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
                return;
            }
            await managedStream.Error.WriteLineAsync("Unsupported format: Please provide a valid output-type", cancellationToken);

            return;
        }

        await managedStream.Error.WriteLineAsync($"Unable to read environment: {valueResult.Exception?.Message ?? "Unknown issue"}", cancellationToken);
    }
}