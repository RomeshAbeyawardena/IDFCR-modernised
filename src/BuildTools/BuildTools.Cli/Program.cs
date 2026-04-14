using BuildTools.Infrastructure;
using IDFCR.Abstractions.Cli.ManagedStreams;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IDFCR.Abstractions.Cli.Extensions;
using BuildTools.Infrastructure.SqlServer.Extensions;
using BuildTools.Cli;
using Microsoft.Extensions.Options;

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    var dbSettings = context.Configuration.Get<DbSettings>();

    var currentAssembly = typeof(Program).Assembly;
    services
        .Configure<ApplicationConfiguration>(context.Configuration)
        .Configure<LockRetryConfiguration>(context.Configuration.GetSection(nameof(LockRetryConfiguration)))
        .AddSingleton(TimeProvider.System)
        .AddSingleton(ConsoleStream.Std)
        .AddInjectableCommandServices(currentAssembly)
        .AddRepositories(dbSettings ?? throw new InvalidOperationException("Unable to bind settings"));
}

using var host = new HostBuilder()
    .ConfigureServices(ConfigureServices)
    .ConfigureHostConfiguration(s => s.AddUserSecrets<Program>())
    .Build();

var options = host.Services.GetRequiredService<IOptions<ApplicationConfiguration>>();


var applicationConfiguration = options.Value;

await host.RunCommandsAsync(args, applicationConfiguration.ListOperations);
