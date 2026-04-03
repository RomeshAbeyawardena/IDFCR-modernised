using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using IDFCR.Abstractions.DatabaseUpdater.Commands;


namespace IDFCR.Abstractions.DatabaseUpdater.Tests;

[FeatureCommand(DatabaseRootCommand.Prefix, CommandName)]
public class ExtensionForDatabaseCommand(IServiceProvider service, IManagedStream managedStream)
    : InjectableCommandOperationBase<ExtensionForDatabaseCommand>(service, DatabaseRootCommand.Prefix, CommandName, typeof(DatabaseRootCommand))
{
    public const string CommandName = "extension-feature";

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        await managedStream.Out.WriteLineAsync("Extension for database command executed successfully.", cancellationToken);
    }
}

// In ConfigureDatabaseUpdater or AddInjectableCommandServices
// after Scrutor scan — validate that every [FeatureCommand] key
// matches its base class constructor prefix argument

