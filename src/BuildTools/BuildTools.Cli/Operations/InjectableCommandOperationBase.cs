using BuildTools.Cli.Extensions;
using BuildTools.Cli.ManagedStreams;
using Microsoft.Extensions.DependencyInjection;


namespace BuildTools.Cli.Operations;

public abstract class InjectableCommandOperationBase<T>(IServiceProvider serviceProvider, string prefix, string name, Type? memberOfType, params string[] aliases)
    : CommandOperationBase(name, aliases), IInjectableCommandOperation
    where T : IInjectableCommandOperation
{
    private IInjectableCommandOperation[]? cachedOperations = null;
    private IInjectableCommandOperation[] remainingOperations = [];

    private IInjectableCommandOperation[] LocateOperations(IEnumerable<string> commands)
    {
        var serviceLocator = string.Empty;
        List<IInjectableCommandOperation> operations = [];
        foreach (var command in commands.Skip(1))
        {
            serviceLocator = string.IsNullOrWhiteSpace(serviceLocator) ?
                serviceLocator = $"{prefix}-{command}"
                : serviceLocator = $"{serviceLocator}-{command}";
            var service = Services.GetKeyedService<IInjectableCommandOperation>(serviceLocator);

            if (service is not null)
            {
                operations.Add(service);
            }
        }

        return [.. operations];
    }

    protected override IEnumerable<ICommandOperation> GetOperations(IEnumerable<string> commands)
    {
        base.GetOperations(commands);
        var currentType = typeof(T);

        var operations = cachedOperations ??= LocateOperations(commands);

        remainingOperations = [.. operations.Skip(1)];

        foreach (var operation in operations)
        {
            operation.CachedOperations = remainingOperations;
            operation.ListOperations = ListOperations;
        }

        ICommandOperation[] ownedOperations = [..operations.Where(x => x.MemberOfType == currentType)];

        AddSupportedOperations(ownedOperations);

        if (ListOperations)
        {
            var managedStream = serviceProvider.GetRequiredService<IManagedStream>();

            managedStream.Out.WriteLineAsync($"Operation: {Name}, FQN: {QualifiedName}", CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        return ownedOperations;
    }

    protected virtual Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        //TODO: we should put a warning that owned context does nothing in a logger instance
        return Task.CompletedTask;
    }

    protected virtual bool CanBypass(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        return RemainingOperations.Any();
    }

    protected virtual Task<bool> CanBypassAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        return Task.FromResult(CanBypass(command, cancellationToken));
    }

    protected override async Task OnInvokeAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        if (await CanBypassAsync(command, cancellationToken))
        {
            //TODO: we can refine this later with injected behaviours if we need commands needing to do sequential work!
            return;
        }

        await InvokeWhenContextIsOwned(command, cancellationToken);
    }

    protected InjectableCommandOperationBase(IServiceProvider serviceProvider, string prefix, string name, params string[] aliases)
        : this(serviceProvider, prefix, name, null, aliases)
    {
        
    }

    protected IEnumerable<IInjectableCommandOperation> RemainingOperations => remainingOperations;

    public string QualifiedName => $"{prefix}-{Name}";
    public IServiceProvider Services => serviceProvider;
    public Type? MemberOfType => memberOfType;
    public IEnumerable<IInjectableCommandOperation> CachedOperations { set => cachedOperations = [.. value]; }
}
