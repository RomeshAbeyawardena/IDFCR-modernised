namespace BuildTools.Cli.ManagedStreams;

public sealed class ConsoleStream : ManagedStreamBase
{
    private static readonly Lazy<IManagedStream> _consoleManagedStream = new(() => new ConsoleStream(), true);
    public static IManagedStream Std => _consoleManagedStream.Value;

    private class ConsoleInputStream(TextReader stream) : ReadableStreamBase(stream)
    {
        internal IManagedStream? ManagedStream { get; set; }

        public override char ReadChar()
        {
            if (ManagedStream?.IsInteractive ?? false)
            {
                return (char)Console.Read();
            }

            return char.MinValue;
        }
    }

    public ConsoleStream() : base(Console.BufferWidth, Console.Error, Console.In, Console.Out, str => new ConsoleInputStream(str))
    {
        if (In is ConsoleInputStream consoleInputStream)
        {
            consoleInputStream.ManagedStream = this;
        }
    }
}
