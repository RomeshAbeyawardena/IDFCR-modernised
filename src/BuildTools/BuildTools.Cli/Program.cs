using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services
        .AddSingleton(ConsoleStream.Std)
        .AddInjectableCommandServices(typeof(Program).Assembly);
}

using var host = new HostBuilder()
    .ConfigureServices(ConfigureServices)
    .Build();

await host.RunCommandsAsync(args, true);
