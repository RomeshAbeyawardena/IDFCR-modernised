using BuildTools.Cli.Dispatchers;
using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddInjectableCommandServices(typeof(Program).Assembly);
}

using var host = new HostBuilder()
    .ConfigureServices(ConfigureServices)
    .Build();

var services = host.Services;

var dispatcher = services.GetRequiredService<ICommandRouteDispatcher>();

CancellationTokenSource tokenSource = new();

await dispatcher.ExecuteAsync(args, tokenSource.Token);


public class SettingsRootOperation(IServiceProvider serviceProvider)
    : InjectableCommandOperationRootBase<SettingsRootOperation>(serviceProvider, Prefix, CommandName, null)
{
    public const string Prefix = "settings";
    public const string CommandName = "settings";
}

public class GetSettingsRootOperation(IServiceProvider serviceProvider, IManagedStream managedStream) 
    : InjectableCommandOperationBase<GetSettingsRootOperation>(serviceProvider, Prefix, "get")
{
    public const string Prefix = $"{SettingsRootOperation.Prefix}-get";

    protected override Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        return base.InvokeWhenContextIsOwned(command, cancellationToken);
    }
}