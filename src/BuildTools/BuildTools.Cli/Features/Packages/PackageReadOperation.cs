using Debug = BuildTools.Cli.Extensions;
using BuildTools.Infrastructure.Features.Packages;
using IDFCR.Abstractions.Results.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Cli.Extensions;

namespace BuildTools.Cli.Features.Packages;

[FeatureCommand(PackageRootOperation.Prefix, CommandName)]
public class PackageReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream,
    IPackageRepository packageRepository)
    : InjectableCommandOperationBase<PackageReadOperation>(serviceProvider, PackageRootOperation.Prefix,
        CommandName, typeof(PackageRootOperation))
{
    public const string CommandName = "read";
    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var @namespace = await this.GetOptionalField(managedStream, command, cancellationToken, false, "namespace");
        var name = await this.GetOptionalField(managedStream, command, cancellationToken, true, "name");
        var outputType = await this.GetOptionalField(managedStream, command, cancellationToken, true, "output-type");

        var pagedQuery = await this.GetPagingFields(managedStream, command, cancellationToken);

        if (string.IsNullOrWhiteSpace(@namespace) || string.IsNullOrEmpty(name))
        {
            var pagedRequest = new GetPagedPackagesQuery
            {
                Name = name,
                Namespace = @namespace
            };

            pagedRequest.MapQuery(pagedQuery);

            var pagedResult = await packageRepository.GetPagedAsync(pagedRequest, cancellationToken);

            if (pagedResult.HasValue)
            {
                if (outputType == "json")
                {
                    await managedStream.Out.WriteLineAsync(pagedResult.Result.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
                    return;
                }

                await Debug.ManagedStreamExtensions.DisplayPagedTable(managedStream, pagedResult, t => t.Map<PackageDto>(), cancellationToken,
                new TableField<PackageDto> { Field = s => s.Namespace, Title = "Namespace", RowWidth = 20 },
                new TableField<PackageDto> { Field = s => s.Name, Title = "Name", RowWidth = 40 },
                new TableField<PackageDto>
                {
                    Field = s => s.Description,
                    Title = "Description",
                    RowWidth = 80
                }
                );
            }

            return;
        }

        var valueResult = await packageRepository.GetPackageAsync(name, @namespace, cancellationToken);

        if (valueResult.HasValue)
        {
            if (outputType == "json")
            {
                await managedStream.Out.WriteLineAsync(
                    valueResult.Result.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
                return;
            }
            await managedStream.Error.WriteLineAsync("Unsupported format:  Please provide a valid output-type", cancellationToken);

            return;
        }

        await managedStream.Error.WriteLineAsync($"Unable to read package: {valueResult.Exception?.Message ?? "Unknown issue"}", cancellationToken);
    }
}