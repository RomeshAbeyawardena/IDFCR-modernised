using Microsoft.Extensions.Logging;
using System.Text;

namespace IDFCR.Abstractions.Cli.ManagedStreams;

internal class LoggerWriteableStream(ILogger logger, LogLevel logLevel) : IIOWriteableStream
{
    public Task WriteAsync(Action<StringBuilder> builder, CancellationToken cancellationToken)
    {
        StringBuilder stringBuilder = new();
        builder(stringBuilder);

        var content = stringBuilder.ToString();

        if (!string.IsNullOrWhiteSpace(content) && logger.IsEnabled(logLevel))
        {
            logger.Log(logLevel, "{content}", content.Trim());
        }

        return Task.CompletedTask;
    }
}

internal class LoggerDerivedManagedStream(ILogger<LoggerDerivedManagedStream> logger) : IManagedStream
{
    public bool IsInteractive { get; }
    public IIOWriteableStream Error { get; } = new LoggerWriteableStream(logger, LogLevel.Error);
    public IIOReadableStream In => NullReadableStream.Instance;
    public IIOWriteableStream Out { get; } = new LoggerWriteableStream(logger, LogLevel.Information);
    public long Width { get; } = 0;
}