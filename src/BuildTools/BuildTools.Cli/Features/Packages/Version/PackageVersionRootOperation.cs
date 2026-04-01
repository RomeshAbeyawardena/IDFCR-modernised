using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Infrastructure.Features.Packages;

namespace BuildTools.Cli.Features.Packages.Version;

[FeatureCommand(Prefix, CommandName)]
public class PackageVersionRootOperation(IServiceProvider serviceProvider)
    : InjectableCommandOperationRootBase<PackageVersionRootOperation>(serviceProvider, Prefix,
        CommandName, typeof(PackageVersionRootOperation))
{
    public const string Prefix = $"{PackageRootOperation.Prefix}-version-";
    public const string CommandName = "version";
}


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

//lists available package versions for the given namespace or package name. If a namespace is provided, it lists all versions for all packages in that namespace. If a package name is provided, it lists all versions for that specific package within all namespaces.
[FeatureCommand(PackageVersionRootOperation.Prefix, CommandName)]
public class PackageVersionReadOperation(IServiceProvider serviceProvider, IManagedStream managedStream, IPackageRepository packageRepository) 
    : InjectableCommandOperationBase<PackageVersionReadOperation>(serviceProvider, PackageVersionRootOperation.Prefix,
        CommandName, typeof(PackageVersionRootOperation))
{
    public const string CommandName = "read";
    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var packageNamespace = await this.GetRequiredField(managedStream, command, 0, "Package namespace", cancellationToken, false, "namespace");
        var packageName = await this.GetOptionalField(managedStream, command, cancellationToken, false, "name");

    }
}
