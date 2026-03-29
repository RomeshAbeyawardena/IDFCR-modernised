using BuildTools.Cli.Operations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildTools.Cli.Dispatchers;

public class CommandRouteDispatcher(IHost host) : IDisposable
{
    private IServiceScope? _serviceScope;

    public bool ListOperations { private get; set; }

    public Task BeginAsync(IEnumerable<string> commands, CancellationToken? cancellationToken = null)
    {
        if (_serviceScope is not null)
        {
            _serviceScope?.Dispose();
        }

        _serviceScope = host.Services.CreateScope();
        var dispatcher = _serviceScope.ServiceProvider.GetRequiredService<ICommandRouteDispatcher>();
        dispatcher.ListOperations = ListOperations;
        return dispatcher.ExecuteAsync(commands, cancellationToken.GetValueOrDefault(CancellationToken.None));
    }

    public void Dispose() 
    {
        _serviceScope?.Dispose();
        GC.SuppressFinalize(this);
    }
}

public sealed class DefaultCommandRouteDispatcher(IServiceProvider services) : ICommandRouteDispatcher
{
    public bool ListOperations { private get; set; }

    public async Task ExecuteAsync(IEnumerable<string> commands, CancellationToken cancellationToken)
    {
        var operations = GetOperations(commands);
        foreach (var operation in operations)
        {
            operation.ListOperations = ListOperations;
            await operation.InvokeAsync(commands, cancellationToken);
        }
    }

    public IEnumerable<IInjectableCommandOperation> GetOperations(IEnumerable<string> commands)
    {
#pragma warning disable CA2263 //extension methods aren't supported by MOQ
        var s = services.GetServices(typeof(IInjectableCommandOperationRoot))
            .Select(x => x as IInjectableCommandOperationRoot)
            .ToArray();

        List<IInjectableCommandOperation> operations = [];

        foreach (var command in commands)
        {
            operations.AddRange(s.Where(x => x != null && x.CanExecute(command))!);
        }
        return operations;
#pragma warning restore
    }
}
