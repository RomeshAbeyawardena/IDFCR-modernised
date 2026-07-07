using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace IDFCR.Utilities.Extensions;

/// <summary>
/// Defines extension methods for logging, providing additional functionality for logging messages with specified log levels and actions.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Logs a message with the specified log level and action, allowing for the tracking of events and issues related to outbox entity notifications. This method is responsible for logging messages based on the provided log level and action, enabling developers to implement custom logging logic for handling notifications related to outbox entities. By using this method, developers can ensure that relevant information is logged during the processing of outbox messages and the tracking of their status within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// /// <param name="logger">The ILogger instance used for logging.</param>
    /// <param name="logLevel">The log level at which the message should be logged.</param>
    /// <param name="logAction">The action that performs the logging using the provided ILogger instance.</param>
    public static void Log(this ILogger logger, LogLevel logLevel, Action<ILogger> logAction)
    {
        if (logger.IsEnabled(logLevel))
        {
            logAction(logger);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="logLevel"></param>
    /// <param name="message"></param>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
    public static void LogMethod(this ILogger logger, LogLevel logLevel, string message, [CallerMemberName] string methodName = "", params object?[] args)
    {
        List<object> arguments = [methodName];
        arguments.AddRange(args!);
#pragma warning disable CA2254, CA1873 //The LogLevel enabled check is done inside the invoked method
        logger.Log(logLevel, l => l.Log(logLevel, $"{{methodName}}: {message}", [.. arguments]));
#pragma warning restore CA1873
    }

    /// <summary>
    /// Logs a message with the specified log level and method name, allowing for the tracking of events and issues related to outbox entity notifications. This method is responsible for logging messages based on the provided log level and method name, enabling developers to implement custom logging logic for handling notifications related to outbox entities. By using this method, developers can ensure that relevant information is logged during the processing of outbox messages and the tracking of their status within applications and systems that utilize an outbox pattern for reliable message delivery and tracking of message status.
    /// </summary>
    /// <param name="logger">The ILogger instance used for logging.</param>
    /// <param name="logLevel">The log level at which the message should be logged.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="methodName">The name of the method from which the log is being generated.</param>
    public static void LogMethod(this ILogger logger, LogLevel logLevel, string message, [CallerMemberName] string methodName = "")
    {
#pragma warning disable CA1873 //The LogLevel enabled check is done inside the invoked method
        logger.Log(logLevel, l => l.Log(logLevel, "{methodName}: {message}", methodName, message));
#pragma warning restore CA1873
    }
}
