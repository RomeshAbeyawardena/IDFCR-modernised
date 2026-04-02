using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;

namespace BuildTools.Cli.Features.Packages.Version;

//increments package version by adding a new version with a new revision number, and making it the latest version. If the package doesn't exist, it will be created with version 1.0.0
[FeatureCommand(PackageVersionRootOperation.Prefix, CommandName)]
public class PackageVersionIncrementOperation(IServiceProvider serviceProvider, IManagedStream managedStream)
    : InjectableCommandOperationBase<PackageVersionIncrementOperation>(serviceProvider, PackageVersionRootOperation.Prefix,
        CommandName, typeof(PackageVersionRootOperation))
{
    public const string CommandName = "increment";

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var packageNamespace = await this.GetRequiredField(managedStream, command, 0, "Package namespace", cancellationToken, false, "namespace");
        var packageVersionPrefix = await this.GetRequiredField(managedStream, command, 0, "Package version prefix", cancellationToken, false, "version-prefix");
        var packageName = await this.GetOptionalField(managedStream, command, cancellationToken, false, "name");
        
    }
}
