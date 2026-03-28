namespace BuildTools.Cli.ManagedStreams;

public interface IIOReadableStream
{
    char ReadChar();
    Task<string?> ReadLineAsync(CancellationToken cancellationToken);
    
}
