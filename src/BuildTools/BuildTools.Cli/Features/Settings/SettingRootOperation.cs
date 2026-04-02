using IDFCR.Abstractions.Cli.Operations;

namespace BuildTools.Cli.Features.Settings;

public class SettingRootOperation(IServiceProvider serviceProvider)
    : InjectableCommandOperationRootBase<SettingRootOperation>(serviceProvider, Prefix, CommandName, null)
{
    public const string Prefix = "setting";
    public const string CommandName = "setting";
}
