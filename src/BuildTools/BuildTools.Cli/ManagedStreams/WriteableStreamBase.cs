using System.Text;

namespace BuildTools.Cli.ManagedStreams
{
    internal class WriteableStreamBase(TextWriter stream) : IIOWriteableStream
    {
        public Task WriteAsync(Action<StringBuilder> builderAction, CancellationToken cancellationToken)
        {
            var builder = new StringBuilder();
            builderAction(builder);
            return stream.WriteAsync(builder, cancellationToken);
        }
    }
}
