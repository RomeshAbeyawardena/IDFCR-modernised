using IDFCR.DatabaseUpdater.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IDFCR.Abstractions.DatabaseUpdater.Tests;

internal class MyTestDbContext : DbContext;

internal class ApplyDatabaseMigrationCommandTests
{
    private IServiceProvider serviceProvider;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        //we provide the assembly name so we can provide the location of commands we want to extend into the CLI
        services = services.ConfigureDatabaseUpdater(TargetDatabaseConfiguration.Create<MyTestDbContext>(), typeof(MyTestDbContext).Assembly);

        serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        if (serviceProvider is not null && serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}

