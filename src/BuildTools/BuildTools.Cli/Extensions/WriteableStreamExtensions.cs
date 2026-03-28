using BuildTools.Cli.ManagedStreams;

namespace BuildTools.Cli.Extensions;

public class ConsoleColourDecorator : IDisposable
{
    private readonly ConsoleColor? resetBackgroundColour;
    private readonly ConsoleColor? resetForegroundColour;

    public ConsoleColourDecorator(ConsoleColor? backgroundColour, ConsoleColor? foregroundColour)
    {
        if (backgroundColour.HasValue)
        {
            resetBackgroundColour = Console.BackgroundColor;
            Console.BackgroundColor = backgroundColour.Value;
        }

        if (foregroundColour.HasValue)
        {
            resetForegroundColour = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColour.Value;
        }
        
        
    }

    public void Dispose()
    {
        if (resetBackgroundColour.HasValue)
        {
            Console.BackgroundColor = resetBackgroundColour.Value;
        }

        if (resetForegroundColour.HasValue)
        {
            Console.ForegroundColor = resetForegroundColour.Value;
        }

        GC.SuppressFinalize(this);
    }
}

public static class WriteableStreamExtensions
{
    public static Task WriteAsync(this IIOWriteableStream writeableStream, string format, CancellationToken cancellationToken,
        params object[] parameters)
    {
        return writeableStream.WriteAsync(b => b.Append(string.Format(format, parameters)), cancellationToken);
    }

    public static Task WriteLineAsync(this IIOWriteableStream writeableStream, string format, CancellationToken cancellationToken,
        params object[] parameters)
    {
        return writeableStream.WriteAsync(b => b.AppendLine(string.Format(format, parameters)), cancellationToken);
    }
#pragma warning disable IDE0060 //writable stream isn't directly used
    public static IDisposable BeginError(this IManagedStream writeableStream, ConsoleColor? backgroundColour = null, ConsoleColor? foregroundColour = ConsoleColor.Red)
    {
        return new ConsoleColourDecorator(backgroundColour, foregroundColour);
    }

    public static IDisposable BeginWarning(this IManagedStream writeableStream, ConsoleColor? backgroundColour = null, ConsoleColor? foregroundColour = ConsoleColor.Yellow)
    {
        return new ConsoleColourDecorator(backgroundColour, foregroundColour);
    }
#pragma warning restore
}
