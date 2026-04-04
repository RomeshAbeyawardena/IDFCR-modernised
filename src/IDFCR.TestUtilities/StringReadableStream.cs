using IDFCR.Abstractions.Cli.ManagedStreams;

namespace IDFCR.TestUtilities;

/// <summary>
/// Represents a readable stream that wraps a StringReader, allowing it to be used as an IReadableStream in testing scenarios. This class inherits from ReadableStreamBase, which provides the necessary implementation to read from the underlying StringReader and expose it through the IReadableStream interface. It is designed to facilitate testing of components that depend on reading input from a stream, enabling developers to easily provide string input for testing purposes without needing to interact with actual file streams or other I/O sources.
/// </summary>
/// <param name="reader">The StringReader to wrap.</param>
public class StringReadableStream(StringReader reader) : ReadableStreamBase(reader)
{

}
