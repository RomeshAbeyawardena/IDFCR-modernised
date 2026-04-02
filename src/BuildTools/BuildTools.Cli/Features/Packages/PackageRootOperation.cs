
using IDFCR.Abstractions.Cli.Operations;

namespace BuildTools.Cli.Features.Packages;

public class PackageRootOperation(IServiceProvider serviceProvider)
    : InjectableCommandOperationRootBase<PackageRootOperation>(serviceProvider, Prefix, CommandName, null)
{
    public const string Prefix = "package";
    public const string CommandName = "package";
}