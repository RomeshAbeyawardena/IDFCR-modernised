using BuildTools.Cli.Operations;

namespace BuildTools.Cli.Features.Settings;

public class SettingsRootOperation(IServiceProvider serviceProvider)
    : InjectableCommandOperationRootBase<SettingsRootOperation>(serviceProvider, Prefix, CommandName, null)
{
    public const string Prefix = "settings";
    public const string CommandName = "settings";
}
