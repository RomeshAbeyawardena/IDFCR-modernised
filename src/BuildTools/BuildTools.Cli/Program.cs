using BuildTools.Application.Extensions;
using BuildTools.Cli;
using BuildTools.Infrastructure.SqlServer.Extensions;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    var currentAssembly = typeof(Program).Assembly;
    services
        .Configure<ApplicationConfiguration>(context.Configuration)
        .Configure<LockRetryConfiguration>(context.Configuration.GetSection(nameof(LockRetryConfiguration)))
        .AddSingleton(TimeProvider.System)
        .AddSingleton(ConsoleStream.Std)
        .AddInjectableCommandServices(currentAssembly)
        .AddRepositories(context.Configuration)
        .AddMediatorServices(context.Configuration);
}

using var host = new HostBuilder()
    .ConfigureServices(ConfigureServices)
    .ConfigureHostConfiguration(s => s.AddUserSecrets<Program>())
    .Build();

var options = host.Services.GetRequiredService<IOptions<ApplicationConfiguration>>();


var applicationConfiguration = options.Value;

await host.RunCommandsAsync(args, applicationConfiguration.ListOperations);
