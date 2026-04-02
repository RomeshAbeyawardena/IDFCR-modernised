using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Infrastructure.Features.Packages.Version;
using IDFCR.Abstractions.Results.Extensions;
using Microsoft.SqlServer.Server;

namespace BuildTools.Cli.Features.Packages.Version;

//lists available package versions for the given namespace or package name. If a namespace is provided, it lists all versions for all packages in that namespace. If a package name is provided, it lists all versions for that specific package within all namespaces.
[FeatureCommand(PackageVersionRootOperation.Prefix, CommandName)]
public class PackageVersionReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream, IPackageVersionRepository packageVersionRepository)
    : InjectableCommandOperationBase<PackageVersionReadOperation>(serviceProvider, PackageVersionRootOperation.Prefix,
        CommandName, typeof(PackageVersionRootOperation))
{
    public const string CommandName = "read";
    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var packageNamespace = await this.GetOptionalField(managedStream, command, cancellationToken, false, "namespace");
        var packageName = await this.GetOptionalField(managedStream, command, cancellationToken, false, "name");
        var outputType = await this.GetOptionalField(managedStream, command, cancellationToken, true, "output-type");
        var pagedQuery = await this.GetPagingFields(managedStream, command, cancellationToken);

        if (!string.IsNullOrWhiteSpace(packageNamespace) || !string.IsNullOrWhiteSpace(packageName))
        {
            var pagedRequest = new GetPackageVersionPagedRequest
            {
                PackageName = packageName,
                PackageVersion = packageNamespace
            };

            pagedRequest.MapQuery(pagedQuery);

            var pagedResult = await packageVersionRepository.GetPagedAsync(pagedRequest, cancellationToken);

            if (pagedResult.HasValue)
            {
                if (outputType == "json")
                {
                    await managedStream.Out.WriteLineAsync(pagedResult.Result.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
                    return;
                }

                await managedStream.DisplayPagedTable(pagedResult, t => t.Map<PackageVersionDto>(), cancellationToken,
                    new TableField<PackageVersionDto> { Field = pv => pv.VersionPrefix, Title = "Version prefix", RowWidth = 5 },
                    new TableField<PackageVersionDto> { Field = pv => pv.Version, Title = "Version prefix", RowWidth = 20 },
                    new TableField<PackageVersionDto>
                    {
                        Field = pv => pv.ReleaseDateTimestampUtc,
                        Title = "Release date",
                        Format = Formatters.FormatDate("Not released", "-"),
                        RowWidth = 10
                    },
                    new TableField<PackageVersionDto>
                    {
                        Field = pv => pv.PublishedTimestampUtc,
                        Title = "Publish date",
                        Format = Formatters.FormatDate("Not published", "-"),
                        RowWidth = 10
                    });
            }

            return;
        }
    }
}
