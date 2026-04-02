using IDFCR.Abstractions.Cli.Dispatchers;
using Microsoft.Extensions.Hosting;

namespace IDFCR.Abstractions.Cli.Extensions;

internal static class HostExtensions
{
    public static Task RunCommandsAsync(this IHost host, IEnumerable<string> args, bool listOperations = false, CancellationToken? cancellationToken = null)
    {
        using var commandDispatcher = new CommandRouteDispatcher(host)
        {
            ListOperations = listOperations
        };

        return commandDispatcher.BeginAsync(args, cancellationToken);
    }
}
