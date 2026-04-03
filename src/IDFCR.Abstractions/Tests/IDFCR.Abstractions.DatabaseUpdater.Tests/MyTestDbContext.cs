using Microsoft.EntityFrameworkCore;


namespace IDFCR.Abstractions.DatabaseUpdater.Tests;

internal class MyTestDbContext(DbContextOptions<MyTestDbContext> options) : DbContext(options);

// In ConfigureDatabaseUpdater or AddInjectableCommandServices
// after Scrutor scan — validate that every [FeatureCommand] key
// matches its base class constructor prefix argument

