namespace BuildTools.Cli.ManagedStreams
{
    internal abstract class ReadableStreamBase(TextReader reader) : IIOReadableStream
    {
        public virtual char ReadChar()
        {
            return (char)reader.Read();
        }

        public async Task<string?> ReadLineAsync(CancellationToken cancellationToken)
        {
            return await reader.ReadLineAsync(cancellationToken);
        }
    }
}
