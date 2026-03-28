namespace BuildTools.Cli.ManagedStreams;

public abstract class ManagedStreamBase(long width, TextWriter error, TextReader @in, TextWriter @out, Func<TextReader, IIOReadableStream> newInstance, bool isInteractive = false) : IManagedStream
{
    public bool IsInteractive { get; } = isInteractive;
    public IIOWriteableStream Error { get; } = new WriteableStreamBase(error);
    public IIOReadableStream In { get; } = newInstance(@in);
    public IIOWriteableStream Out { get; } = new WriteableStreamBase(@out);
    public long Width { get; } = width;
}
