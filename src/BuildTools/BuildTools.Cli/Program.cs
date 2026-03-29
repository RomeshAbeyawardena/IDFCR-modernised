using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Infrastructure;
using BuildTools.Infrastructure.SqlServer.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    var dbSettings = context.Configuration.Get<DbSettings>();

    var currentAssembly = typeof(Program).Assembly;
    services
        .AddSingleton(TimeProvider.System)
        .AddSingleton(ConsoleStream.Std)
        .AddInjectableCommandServices(currentAssembly)
        .AddRepositories(dbSettings ?? throw new InvalidOperationException("Unable to bind settings"));
}

using var host = new HostBuilder()
    .ConfigureServices(ConfigureServices)
    .ConfigureHostConfiguration(s => s.AddUserSecrets<Program>())
    .Build();

await host.RunCommandsAsync(args);
