namespace IDFCR.Abstractions.Cli.ManagedStreams;

internal sealed class NullReadableStream : IIOReadableStream
{
    public static IIOReadableStream Instance { get; } = new NullReadableStream();

    private NullReadableStream()
    {
    }

    public Task<string?> ReadLineAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(null);
    }

    public char ReadChar()
    {
        return char.MinValue;
    }
}
