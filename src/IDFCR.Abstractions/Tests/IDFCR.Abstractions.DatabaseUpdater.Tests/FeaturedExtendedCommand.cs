using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;


namespace IDFCR.Abstractions.DatabaseUpdater.Tests;

[FeatureCommand(ExtendedCommandRoot.Prefix, CommandName)]
public class FeaturedExtendedCommand(IServiceProvider serviceProvider, IManagedStream managedStream) 
    : InjectableCommandOperationBase<ExtendedCommandRoot>(serviceProvider, ExtendedCommandRoot.Prefix, CommandName, typeof(ExtendedCommandRoot))
{
    public const string CommandName = "feature";

    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        await managedStream.Out.WriteLineAsync("Featured command executed successfully.", cancellationToken);
    }
}

// In ConfigureDatabaseUpdater or AddInjectableCommandServices
// after Scrutor scan — validate that every [FeatureCommand] key
// matches its base class constructor prefix argument

