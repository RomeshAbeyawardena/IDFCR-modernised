using BuildTools.Infrastructure.SqlServer;
using IDFCR.DatabaseUpdater.Extensions;

using var _ = await HostExtensions
    .ConfigureDatabaseUpdaterHost(TargetDatabaseConfiguration.Create<PackageManagerDbContext>(), args);