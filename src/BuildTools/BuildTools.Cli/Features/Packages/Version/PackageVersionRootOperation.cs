using BuildTools.Cli.Operations;

namespace BuildTools.Cli.Features.Packages.Version;

[FeatureCommand(PackageVersionRootOperation.Prefix, CommandName)]
public class PackageVersionRootOperation(IServiceProvider serviceProvider)
    : InjectableCommandOperationRootBase<PackageVersionRootOperation>(serviceProvider, Prefix,
        CommandName, typeof(PackageVersionRootOperation))
{
    public const string Prefix = Packages.PackageRootOperation.Prefix + "-version";
    public const string CommandName = "read";
}
