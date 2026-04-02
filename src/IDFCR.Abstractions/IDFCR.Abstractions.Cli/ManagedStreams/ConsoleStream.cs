namespace IDFCR.Abstractions.Cli.ManagedStreams;

/// <summary>
/// Represents a managed stream that interacts with the console, providing access to standard input, output, and error streams. This class encapsulates the functionality of the console streams and allows for reading from and writing to the console in a structured manner. The ConsoleStream class inherits from ManagedStreamBase and provides a singleton instance through the Std property, enabling easy access to the console streams throughout the application. The implementation includes a nested ConsoleInputStream class that handles reading characters from the console input stream, allowing for interactive input when necessary.
/// </summary>
public sealed class ConsoleStream : ManagedStreamBase
{
    private static readonly Lazy<IManagedStream> _consoleManagedStream = new(() => new ConsoleStream(), true);

    /// <summary>
    /// Gets a singleton instance of the ConsoleStream, providing access to the console's standard input, output, and error streams. This property allows for easy retrieval of the console stream instance, ensuring that only one instance is created and shared across the application. The ConsoleStream instance can be used to read from and write to the console in a structured manner, facilitating interaction with the user through the console interface.
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleStream"/> class, setting up the console's standard input, output, and error streams. The constructor also initializes the nested <see cref="ConsoleInputStream"/> class for handling interactive input, allowing the console stream to read characters from the console input stream when necessary.
    /// </summary>
    public ConsoleStream() : base(Console.BufferWidth, Console.Error, Console.In, Console.Out, str => new ConsoleInputStream(str))
    {
        if (In is ConsoleInputStream consoleInputStream)
        {
            consoleInputStream.ManagedStream = this;
        }
    }
}
