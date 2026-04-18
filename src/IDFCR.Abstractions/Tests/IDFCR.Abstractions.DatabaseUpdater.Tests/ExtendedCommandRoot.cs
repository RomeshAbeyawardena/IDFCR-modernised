using IDFCR.Abstractions.Cli.Operations;


namespace IDFCR.Abstractions.DatabaseUpdater.Tests;

public class ExtendedCommandRoot(IServiceProvider serviceProvider)
    : InjectableCommandOperationRootBase<ExtendedCommandRoot>(serviceProvider, Prefix, CommandName, null)
{
    public const string Prefix = "extension";
    public const string CommandName = "extension";
}

// In ConfigureDatabaseUpdater or AddInjectableCommandServices
// after Scrutor scan — validate that every [FeatureCommand] key
// matches its base class constructor prefix argument

