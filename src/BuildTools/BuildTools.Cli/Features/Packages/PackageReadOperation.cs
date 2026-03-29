using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Infrastructure.Features.Packages;

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

        if (string.IsNullOrWhiteSpace(@namespace) || string.IsNullOrEmpty(name))
        {
            var pagedResult = await packageRepository.GetPagedAsync(new GetPagedPackagesQuery
            {
                PageSize = 20,
                Name = name,
                Namespace = @namespace
            }, cancellationToken);

            if (pagedResult.HasValue)
            {
                if (outputType == "json")
                {
                    await managedStream.Out.WriteLineAsync(pagedResult.Result.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
                    return;
                }

                await managedStream.DisplayPagedTable(pagedResult, t => t.Map<PackageDto>(t), cancellationToken,
                new TableField<PackageDto> { Field = s => s.Namespace, Title = "Namespace", RowWidth = 20 },
                new TableField<PackageDto> { Field = s => s.Name, Title = "Name", RowWidth = 20 },
                new TableField<PackageDto>
                {
                    Field = s => s.Description,
                    Title = "Description",
                    RowWidth = 40
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
        }

        await managedStream.Error.WriteLineAsync($"Unable to read package: {valueResult.Exception?.Message ?? "Unknown issue"}", cancellationToken);
    }
}