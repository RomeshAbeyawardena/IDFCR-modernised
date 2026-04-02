using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.DatabaseUpdater.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine.Parsing;
using NUnit.Framework;
using Microsoft.Extensions.Hosting;
using Moq;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.TestUtilities;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IDFCR.Abstractions.DatabaseUpdater.Tests;

internal class MyTestDbContext(DbContextOptions<MyTestDbContext> options) : DbContext(options);



internal class ApplyDatabaseMigrationCommandTests
{
    private IServiceProvider serviceProvider;
    private Mock<IHost> host;
    private StringReader sr;
    private StringWriter sw;
    private StringWriter esw;
    private Mock<IManagedStream> managedStream; 
    private Mock<IDatabaseFascade> databaseFascade;
    [SetUp]
    public void Setup()
    {
        host = new();
        sr = new(string.Empty);
        sw = new();
        esw = new();
        managedStream = new();
        managedStream.SetupGet(m => m.In).Returns(new StringReadableStream(sr));
        managedStream.SetupGet(m => m.Out).Returns(new StringWriteableStream(sw));
        managedStream.SetupGet(m => m.Error).Returns(new StringWriteableStream(esw));

        IServiceCollection services = new ServiceCollection();
        services.AddSingleton(managedStream.Object);
        //we provide the assembly name so we can provide the location of commands we want to extend into the CLI
        services.AddDbContext<MyTestDbContext>(options => options.UseInMemoryDatabase("TestDatabase"));
        services = services.ConfigureDatabaseUpdater(TargetDatabaseConfiguration.Create<MyTestDbContext>(), typeof(MyTestDbContext).Assembly);
        databaseFascade = new();

        var existingFascadeDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IDatabaseFascade));

        if (existingFascadeDescriptor is not null)
        {
            services.Remove(existingFascadeDescriptor);
            var fascadeService = new ServiceDescriptor(typeof(IDatabaseFascade), databaseFascade.Object);
            
            services.Add(fascadeService);
        }

        serviceProvider = services.BuildServiceProvider();
        host.Setup(h => h.Services).Returns(serviceProvider);
    }

    [Test]
    public async Task Test()
    {
        var args = CommandLineParser.SplitCommandLine("database migration list");

        //don't worry - I know what I'm doing by passing null to the host!
        await host.Object.RunCommandsAsync(args);

        Assert.That(sw.ToString(), Is.EqualTo("No pending migrations found. The database is already up to date.\r\n"));

        sw.GetStringBuilder().Clear();
        await host.Object.RunCommandsAsync(args);
        databaseFascade.Setup(d => d.GetPendingMigrationsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(["Migration1", "Migration2"]);
        Assert.That(sw.ToString(), Is.EqualTo("No pending migrations found. The database is already up to date.\r\n"));
    }

    [TearDown]
    public void TearDown()
    {
        if (serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}

