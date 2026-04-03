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
using Microsoft.Extensions.Time.Testing;


namespace IDFCR.Abstractions.DatabaseUpdater.Tests;

internal class ListDatabaseMigrationsCommandTests
{
    private IServiceProvider serviceProvider;
    private Mock<IHost> host;
    private StringReader sr;
    private StringWriter sw;
    private StringWriter esw;
    private Mock<IManagedStream> managedStream; 
    private Mock<IDatabaseFascade> databaseFascade;
    private FakeTimeProvider fakeTimeProvider;
    private bool isDbContextFascadeRegistered = false;

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
        fakeTimeProvider = new(new DateTimeOffset(2026, 04, 03, 13, 30, 00, TimeSpan.Zero));
        IServiceCollection services = new ServiceCollection();
        services
            .AddSingleton<TimeProvider>(fakeTimeProvider)
            .AddSingleton(managedStream.Object);
        //we provide the assembly name so we can provide the location of commands we want to extend into the CLI
        services.AddDbContext<MyTestDbContext>(options => options.UseInMemoryDatabase("TestDatabase"));
        services = services
            .ConfigurePromptGreeterOptions(opt => opt.UseDefault(Cli.PromptGreeterDefaults.Western).Build())
            .ConfigureDatabaseUpdater(TargetDatabaseConfiguration.Create<MyTestDbContext>(), typeof(MyTestDbContext).Assembly);
        databaseFascade = new();

        var existingFascadeDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IDatabaseFascade));

        if (existingFascadeDescriptor is not null)
        {
            isDbContextFascadeRegistered = true;
            services.Remove(existingFascadeDescriptor);
            var fascadeService = new ServiceDescriptor(typeof(IDatabaseFascade), databaseFascade.Object);
            
            services.Add(fascadeService);
        }

        serviceProvider = services.BuildServiceProvider();
        host.Setup(h => h.Services).Returns(serviceProvider);
    }

    [Test]
    public void Ensure_database_fascade_is_registered()
    {
        Assert.That(isDbContextFascadeRegistered, Is.True);
    }

    [Test]
    public async Task Test_to_affirm_that_no_pending_migrations_are_found()
    {
        var args = CommandLineParser.SplitCommandLine("database migration list");

        await host.Object.RunCommandsAsync(args);

        Assert.That(sw.ToString(), Is.EqualTo("Good afternoon. The current time is 14:30.\r\nNo pending migrations found. The database is already up to date.\r\n"));
    }

    [Test]
    public async Task Test_to_affirm_that_pending_migrations_are_found()
    {
        var args = CommandLineParser.SplitCommandLine("database migration list");
        databaseFascade.Setup(d => d.GetPendingMigrationsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(["Migration1", "Migration2"]);
        await host.Object.RunCommandsAsync(args);

        Assert.That(sw.ToString(), Is.EqualTo("Good afternoon. The current time is 14:30.\r\nPending Migrations:\r\n\t- Migration1\r\n\t- Migration2\r\n"));
    }

    [Test]
    public async Task Test_to_affirm_that_an_error_is_written_on_failure()
    {
        databaseFascade.Setup(d => d.GetPendingMigrationsAsync(It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new InvalidOperationException("DB unavailable"));

        var args = CommandLineParser.SplitCommandLine("database migration list");
        await host.Object.RunCommandsAsync(args);

        Assert.That(esw.ToString(), Does.Contain("An error occurred during database migration: DB unavailable"));
        Assert.That(sw.ToString(), Is.EqualTo("Good afternoon. The current time is 14:30.\r\n"));
    }

    [Test]
    public async Task Test_to_affirm_that_custom_commands_are_invoked()
    {
        var args = CommandLineParser.SplitCommandLine("extension feature");
        await host.Object.RunCommandsAsync(args);

        Assert.That(sw.ToString(), Is.EqualTo("Featured command executed successfully.\r\n"));
    }

    [Test]
    public async Task Test_to_affirm_that_custom_commands_extending_existing_commands_are_invoked()
    {
        var args = CommandLineParser.SplitCommandLine("database extension-feature");
        await host.Object.RunCommandsAsync(args);

        Assert.That(sw.ToString(), Is.EqualTo("Good afternoon. The current time is 14:30.\r\nExtension for database command executed successfully.\r\n"));
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

// In ConfigureDatabaseUpdater or AddInjectableCommandServices
// after Scrutor scan — validate that every [FeatureCommand] key
// matches its base class constructor prefix argument

