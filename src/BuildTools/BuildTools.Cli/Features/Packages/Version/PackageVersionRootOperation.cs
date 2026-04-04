using IDFCR.Abstractions.Cli.Operations;

namespace BuildTools.Cli.Features.Packages.Version;

[FeatureCommand(PackageRootOperation.Prefix, CommandName)]
public class PackageVersionRootOperation(IServiceProvider serviceProvider)
    : InjectableCommandOperationBase<PackageVersionRootOperation>(serviceProvider, PackageRootOperation.Prefix,
        CommandName, typeof(PackageRootOperation))
{
    public const string Prefix = $"{PackageRootOperation.Prefix}-version";
    public const string CommandName = "version";
}
