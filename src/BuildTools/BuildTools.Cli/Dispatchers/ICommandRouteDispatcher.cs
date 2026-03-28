using BuildTools.Cli.Operations;

namespace BuildTools.Cli.Dispatchers;

public interface ICommandRouteDispatcher
{
    bool ListOperations { set; }
    IEnumerable<IInjectableCommandOperation> GetOperations(IEnumerable<string> commands);
    Task ExecuteAsync(IEnumerable<string> command, CancellationToken cancellationToken);
}
