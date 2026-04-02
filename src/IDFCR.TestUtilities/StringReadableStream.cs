using IDFCR.Abstractions.Cli.ManagedStreams;

namespace IDFCR.TestUtilities
{
    internal class StringReadableStream(StringReader reader) : ReadableStreamBase(reader)
    {

    }
}
