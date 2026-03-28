namespace BuildTools.Cli.ManagedStreams
{
    public interface IManagedStream
    {
        bool IsInteractive { get; }
        IIOWriteableStream Error { get; }
        IIOReadableStream In { get; }
        IIOWriteableStream Out { get; }
        long Width { get; }
    }
}
