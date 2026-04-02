using IDFCR.Abstractions.Cli.ManagedStreams;

namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Represents extension methods for IIOWriteableStream to provide convenient ways to write formatted strings and manage console colors for error and warning messages. These extensions allow for easier and more consistent output formatting when writing to streams in a CLI application.
/// </summary>
public static class WriteableStreamExtensions
{
    /// <summary>
    /// Writes a formatted string to the provided IIOWriteableStream using the specified format and parameters. The method uses string.Format to format the string before writing it to the stream. This allows for easy and consistent formatting of output when writing to streams in a CLI application.
    /// </summary>
    /// <param name="writeableStream">The stream to write to.</param>
    /// <param name="format">The format string.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="parameters">The parameters to format into the string.</param>
    /// <returns></returns>
    public static Task WriteAsync(this IIOWriteableStream writeableStream, string format, CancellationToken cancellationToken,
        params object[] parameters)
    {
        return writeableStream.WriteAsync(b => b.Append(string.Format(format, parameters)), cancellationToken);
    }

    /// <summary>
    /// Writes a formatted string followed by a newline to the provided IIOWriteableStream using the specified format. This method is a convenient way to write a line of text to the stream without needing to manually append a newline character. It allows for easy and consistent formatting of output when writing to streams in a CLI application.
    /// </summary>
    /// <param name="writeableStream">The stream to write to.</param>
    /// <param name="format">The format string.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static Task WriteLineAsync(this IIOWriteableStream writeableStream, string format, CancellationToken cancellationToken)
    {
        return writeableStream.WriteAsync(b => b.AppendLine(format), cancellationToken);
    }

    /// <summary>
    /// Writes a formatted string followed by a newline to the provided IIOWriteableStream using the specified format and parameters. This method is a convenient way to write a line of text to the stream with formatted content without needing to manually append a newline character. It allows for easy and consistent formatting of output when writing to streams in a CLI application.
    /// </summary>
    /// <param name="writeableStream">The stream to write to.</param>
    /// <param name="format">The format string.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="parameters">The parameters to format into the string.</param>
    /// <returns></returns>
    public static Task WriteLineAsync(this IIOWriteableStream writeableStream, string format, CancellationToken cancellationToken,
        params object[] parameters)
    {
        return writeableStream.WriteAsync(b => b.AppendLine(string.Format(format, parameters)), cancellationToken);
    }
#pragma warning disable IDE0060 //writable stream isn't directly used
    /// <summary>
    /// Begins a console color scope for error messages. This method returns an IDisposable that, when disposed, will reset the console colors to their previous state. The method allows you to specify optional background and foreground colors for error messages, with the default foreground color set to red. This is useful for visually distinguishing error messages in the console output of a CLI application.
    /// </summary>
    /// <param name="writeableStream">The stream to write to.</param>
    /// <param name="backgroundColour">The background color for the error message.</param>
    /// <param name="foregroundColour">The foreground color for the error message.</param>
    /// <returns>An IDisposable that resets the console colors when disposed.</returns>
    public static IDisposable BeginError(this IManagedStream writeableStream, ConsoleColor? backgroundColour = null, ConsoleColor? foregroundColour = ConsoleColor.Red)
    {
        return new ConsoleColourDecorator(backgroundColour, foregroundColour);
    }

    /// <summary>
    /// Begins a console color scope for warning messages. This method returns an IDisposable that, when disposed, will reset the console colors to their previous state. The method allows you to specify optional background and foreground colors for warning messages, with the default foreground color set to yellow. This is useful for visually distinguishing warning messages in the console output of a CLI application.
    /// </summary>
    /// <param name="writeableStream">The stream to write to.</param>
    /// <param name="backgroundColour">The background color for the warning message.</param>
    /// <param name="foregroundColour">The foreground color for the warning message.</param>
    /// <returns>An IDisposable that resets the console colors when disposed.</returns>
    public static IDisposable BeginWarning(this IManagedStream writeableStream, ConsoleColor? backgroundColour = null, ConsoleColor? foregroundColour = ConsoleColor.Yellow)
    {
        return new ConsoleColourDecorator(backgroundColour, foregroundColour);
    }
#pragma warning restore
}
