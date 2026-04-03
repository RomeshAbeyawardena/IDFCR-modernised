using BuildTools.Infrastructure;
using BuildTools.Infrastructure.SqlServer;
using BuildTools.Infrastructure.SqlServer.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.DatabaseUpdater.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

using var _ = await HostExtensions
    .ConfigureDatabaseUpdaterHost(TargetDatabaseConfiguration.Create<PackageManagerDbContext>(), args, ConfigureServices);

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    var dbSettings = context.Configuration.Get<DbSettings>();

    var currentAssembly = typeof(Program).Assembly;
    services
        .AddSingleton(TimeProvider.System)
        .AddSingleton(ConsoleStream.Std)
        .AddRepositories(dbSettings ?? throw new InvalidOperationException("Unable to bind settings"));
}