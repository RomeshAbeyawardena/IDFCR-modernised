using System.Text;

namespace BuildTools.Cli.ManagedStreams;

public interface IIOWriteableStream
{
    Task WriteAsync(Action<StringBuilder> builder, CancellationToken cancellationToken);
}
