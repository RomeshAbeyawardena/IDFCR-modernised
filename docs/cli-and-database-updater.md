# CLI and database updater

IDFCR provides two related CLI capabilities:

1. **CLI framework** — `ICommandOperation` and related types for building interactive or batch command-line tools.
2. **Database updater** — a self-contained CLI host for running EF Core migrations from the command line.

**Packages:** `IDFCR.Abstractions.Cli`, `IDFCR.Abstractions.Cli.Extensions`, `IDFCR.DatabaseUpdater`, `IDFCR.Abstractions.DatabaseUpdater`

---

## CLI framework

### Core types

| Type | Purpose |
|---|---|
| `ICommandOperation` | Base contract for a CLI command |
| `IInjectableCommandOperation` | Command that receives services from DI |
| `InjectableCommandOperationBase` | Convenience base class for DI-enabled commands (`InjectableCommandOperationBase<T>`) |
| `ICommandRouteDispatcher` | Routes parsed arguments to the correct command |
| `DefaultCommandRouteDispatcher` | Default implementation backed by assembly scanning |
| `IArgumentParameters` | Parsed key-value argument bag |
| `IManagedStream` | Abstraction over console input/output (testable) |
| `IPromptGreeter` | Configurable greeting message for interactive prompts |

### Writing a command

Commands derive from `InjectableCommandOperationBase<T>` (where `T` is your own command type) and override `InvokeWhenContextIsOwned` to run command logic. Arguments are available through the `Parameters` dictionary populated by the base class.

```csharp
using IDFCR.Abstractions.Cli.Operations;

[FeatureCommand("orders", "create")]   // prefix + key matched by the router
public sealed class CreateOrderCliCommand(IServiceProvider serviceProvider, IMediator mediator)
    : InjectableCommandOperationBase<CreateOrderCliCommand>(serviceProvider, "orders", "create")
{
    protected override async Task InvokeWhenContextIsOwned(
        IEnumerable<string> command,
        CancellationToken cancellationToken)
    {
        // Parameters is populated by the base class from the command tokens.
        var reference = Parameters?["reference"].Value?.ToString()
            ?? throw new ArgumentException("--reference is required");

        var result = await mediator.Send(
            new CreateOrderCommand(reference), cancellationToken);

        // Use IManagedStream to write output. It is typically available via
        // the scoped IInjectableCommandOperation context or DI.
        if (!result.IsSuccess)
        {
            Console.Error.WriteLine($"Failed: {result.FailureReason}");
            return;
        }

        Console.WriteLine($"Created order {result.Result!.Id}");
    }
}
```

`[FeatureCommand("orders", "create")]` sets the prefix and key used by `DefaultCommandRouteDispatcher` to route arguments to this command.

### Registering commands

```csharp
using IDFCR.Abstractions.Cli.Extensions;

services.AddInjectableCommandServices(typeof(Program).Assembly);
```

This scans for all `IInjectableCommandOperation` implementations and registers them in DI.

### Running the CLI

```csharp
using IDFCR.Abstractions.Cli.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddInjectableCommandServices(typeof(Program).Assembly);
        // ... other services
    })
    .Build();

await host.RunCommandsAsync(args, listOperations: false, cancellationToken);
```

`RunCommandsAsync` reads the first argument as the command name, dispatches to the matching `IInjectableCommandOperation`, and exits.

### Managed streams

`IManagedStream` wraps console I/O so commands remain testable without depending on `Console.Out` directly:

```csharp
// In production: ConsoleStream wraps Console
// In tests: use a string-backed or mocked stream
await output.WriteLineAsync("Processing...", cancellationToken);
string? input = await inputStream.ReadLineAsync(cancellationToken);
```

---

## Database updater

`IDFCR.DatabaseUpdater` hosts a CLI specifically for running EF Core database migrations. It builds a `HostBuilder`, registers your `DbContext`, and exposes built-in commands (`db migrations list`, `db migrations apply`).

### ITargetDatabaseConfiguration

Implement `ITargetDatabaseConfiguration` to tell the updater which `DbContext` type to target:

```csharp
public sealed class AppDatabaseConfiguration : ITargetDatabaseConfiguration
{
    public Type DbContextType => typeof(AppDbContext);
}
```

Or use `TargetDatabaseConfiguration` from the package directly:

```csharp
var config = new TargetDatabaseConfiguration(typeof(AppDbContext));
```

### Running migrations from a console app

```csharp
using IDFCR.DatabaseUpdater.Extensions;

// In the entry point of a dedicated migrations console project:
var config = new TargetDatabaseConfiguration(typeof(AppDbContext));

await HostExtensions.ConfigureDatabaseUpdaterHost(
    configurationInstance: config,
    args: args,
    configureServices: (ctx, services) =>
    {
        services.AddDbContext<AppDbContext>(o =>
            o.UseSqlServer(ctx.Configuration.GetConnectionString("Default")));
    },
    assembliesToScan: typeof(Program).Assembly);
```

### Built-in commands

| Command | What it does |
|---|---|
| `db migrations list` | Lists pending and applied migrations |
| `db migrations apply` | Applies pending migrations to the target database |

### Adding custom commands

Create a class implementing `IInjectableCommandOperation` and pass its assembly to `assembliesToScan`. The updater discovers it alongside the built-in commands.

```csharp
[FeatureCommand("db", "seed")]
public sealed class SeedDatabaseCommand(IServiceProvider serviceProvider, AppDbContext db)
    : InjectableCommandOperationBase<SeedDatabaseCommand>(serviceProvider, "db", "seed")
{
    protected override async Task InvokeWhenContextIsOwned(
        IEnumerable<string> command,
        CancellationToken cancellationToken)
    {
        // seed logic
        await db.SaveChangesAsync(cancellationToken);
        Console.WriteLine("Seed complete.");
    }
}
```

### ConfigureDatabaseUpdater (in-process)

If you need to embed database update logic inside a larger host (e.g., running migrations at startup), use:

```csharp
services.ConfigureDatabaseUpdater(config, typeof(Program).Assembly);
```

This registers the updater services without creating a dedicated host.

---

## Further reading

- [Getting started](getting-started.md) — registering CLI services in a host.
- [Architecture overview](architecture-overview.md) — where the CLI fits in the overall package map.
