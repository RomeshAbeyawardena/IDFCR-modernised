using IDFCR.Abstractions.Cli.Operations;

namespace BuildTools.Cli.Features.Environments;

public class EnvironmentRootOperation(IServiceProvider serviceProvider)
    : InjectableCommandOperationRootBase<EnvironmentRootOperation>(serviceProvider, Prefix, CommandName, null)
{
    public const string Prefix = "env";
    public const string CommandName = "env";
}
