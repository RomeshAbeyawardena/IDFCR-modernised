using IDFCR.Abstractions.Cli.Operations;

namespace BuildTools.Cli.Features.Packages.Version;

[FeatureCommand(Prefix, CommandName)]
public class PackageVersionRootOperation(IServiceProvider serviceProvider)
    : InjectableCommandOperationRootBase<PackageVersionRootOperation>(serviceProvider, Prefix,
        CommandName, typeof(PackageVersionRootOperation))
{
    public const string Prefix = $"{PackageRootOperation.Prefix}-version-";
    public const string CommandName = "version";
}
