using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Infrastructure.Features.Packages.Version;

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

        if (!string.IsNullOrWhiteSpace(packageNamespace) || !string.IsNullOrWhiteSpace(packageName))
        {
            var pagedResult = await packageVersionRepository.GetPagedAsync(new GetPackageVersionPagedRequest { 
                PackageName = packageName, 
                PackageVersion = packageNamespace }, cancellationToken);

            if (pagedResult.HasValue)
            {
                if (outputType == "json")
                {
                    await managedStream.Out.WriteLineAsync(pagedResult.Result.Jsonify(System.Text.Json.JsonSerializerOptions.Web), cancellationToken);
                    return;
                }

                await managedStream.DisplayPagedTable(pagedResult, t => t.Map<PackageVersionDto>(), cancellationToken,
                    new TableField<PackageVersionDto> { Field = pv => pv.VersionPrefix, Title = "Version prefix", RowWidth= 5 },
                    new TableField<PackageVersionDto> { Field = pv => pv.Version, Title = "Version prefix", RowWidth = 20 },
                    new TableField<PackageVersionDto> { Field = pv => pv.ReleaseDateTimestampUtc, Title = "Release date", 
                        Format = rd => rd is null 
                            ? "Not released" 
                            : rd is DateTimeOffset date 
                                ? date.ToLocalTime().DateTime.ToShortDateString() 
                                    : "Invalid format", RowWidth = 10 },
                    new TableField<PackageVersionDto>
                    {
                        Field = pv => pv.PublishedTimestampUtc,
                        Title = "Publish date",
                        Format = rd => rd is null
                            ? "Not published"
                            : rd is DateTimeOffset date
                                ? date.ToLocalTime().DateTime.ToShortDateString()
                                    : "Invalid format",
                        RowWidth = 10
                    });
            }

                /*

                    await managedStream.DisplayPagedTable(pagedResult, t => t.Map<PackageDto>(), cancellationToken,
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
                 * 
                 */

                return;
        }



    }
}
