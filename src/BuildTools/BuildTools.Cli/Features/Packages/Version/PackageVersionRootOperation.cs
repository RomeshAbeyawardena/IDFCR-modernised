using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;

namespace BuildTools.Cli.Features.Packages.Version;

[FeatureCommand(Prefix, CommandName)]
public class PackageVersionRootOperation(IServiceProvider serviceProvider)
    : InjectableCommandOperationRootBase<PackageVersionRootOperation>(serviceProvider, Prefix,
        CommandName, typeof(PackageVersionRootOperation))
{
    public const string Prefix = $"{PackageRootOperation.Prefix}-version-";
    public const string CommandName = "version";
}

[FeatureCommand(PackageVersionRootOperation.Prefix, CommandName)]
public class PackageVersionReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream) 
    : InjectableCommandOperationBase<PackageVersionReadOperation>(serviceProvider, PackageVersionRootOperation.Prefix,
        CommandName, typeof(PackageVersionRootOperation))
{
    public const string CommandName = "read";
    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var newVersion = await this.GetOptionalField(managedStream, command, cancellationToken, false, "new-version");
        
    }
}
