using BuildTools.Cli.Common;
using BuildTools.Infrastructure.Features.Packages.Version;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.Cli.Features.Packages.Version;

/// <summary>
/// Lists available package versions for the given namespace or package name.
/// If a namespace is provided, it lists all versions for all packages in that namespace.
/// If a package name is provided, it lists all versions for that specific package within all namespaces.
/// </summary>
[FeatureCommand(PackageVersionRootOperation.Prefix, CommandName)]
public class PackageVersionReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream, IPackageVersionRepository packageVersionRepository)
    : ReadCommandOperationBase<PackageVersionReadOperation>(serviceProvider, managedStream, PackageVersionRootOperation.Prefix,
        CommandName, typeof(PackageVersionRootOperation))
{
    public const string CommandName = "read";

    private string? _packageNamespace;
    private string? _packageName;

    protected override async Task AcquireFieldsAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        _packageNamespace = await this.GetOptionalField(ManagedStream, command, cancellationToken, false, "namespace");
        _packageName = await this.GetOptionalField(ManagedStream, command, cancellationToken, true, "name");
    }

    protected override async Task InvokeReadAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_packageNamespace) || !string.IsNullOrWhiteSpace(_packageName))
        {
            var pagedRequest = new GetPackageVersionPagedRequest
            {
                PackageName = _packageName,
                PackageNamespace = _packageNamespace
            };

            pagedRequest.MapQuery(PagedQuery);

            var pagedResult = await packageVersionRepository.GetPagedAsync(pagedRequest, cancellationToken);

            if (pagedResult.HasValue)
                await WritePagedResultAsync(pagedResult, t => t.Map<PackageVersionDto>(), cancellationToken,
                    new TableField<PackageVersionDto> { Field = pv => pv.Version, Title = "Version", RowWidth = 5 },
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
    }
}
