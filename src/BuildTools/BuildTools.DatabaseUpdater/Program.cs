using BuildTools.Infrastructure;
using BuildTools.Infrastructure.SqlServer;
using BuildTools.Infrastructure.SqlServer.Extensions;
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Prompts;
using IDFCR.DatabaseUpdater.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var currentAssembly = typeof(Program).Assembly;

using var _ = await IDFCR.DatabaseUpdater.Extensions.HostExtensions
    .ConfigureDatabaseUpdaterHost(TargetDatabaseConfiguration.Create<PackageManagerDbContext>(), args, ConfigureHostConfiguration, ConfigureServices, assembliesToScan: currentAssembly);


static void ConfigureHostConfiguration(IConfigurationBuilder builder)
{
    builder.AddUserSecrets<Program>();
}

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    var dbSettings = context.Configuration.Get<DbSettings>();
    services
        .AddSingleton(TimeProvider.System)
        .AddSingleton(ConsoleStream.Std)
        .ConfigurePromptGreeterOptions(opt => opt.UseDefault(PromptGreeterDefaults.Western)
                .Configure(defaultPromptTemplate: $"Build Tools - Migration Assistant v.1.0{Environment.NewLine}{{OriginalTemplate}}")
                .Build())
        .AddRepositories(dbSettings ?? throw new InvalidOperationException("Unable to bind settings"));
}