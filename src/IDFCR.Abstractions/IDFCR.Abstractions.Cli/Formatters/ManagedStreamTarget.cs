namespace IDFCR.Abstractions.Cli.Formatters;

/// <summary>
/// Enumerates the possible target streams for a formatter that writes to a managed stream. The values indicate whether the formatted output should be directed to the error stream (Error) or the standard output stream (Out) of the managed stream. This allows formatters to categorize their output appropriately, such as sending error messages to the error stream while sending regular output to the standard output stream.
/// </summary>
public enum ManagedStreamTarget
{
    /// <summary>
    /// Represents the error stream of the managed stream. When a formatter's Target is set to Error, it indicates that the formatted output should be written to the error stream, which is typically used for logging errors, warnings, or other important messages that require attention.
    /// </summary>
    Error,
    /// <summary>
    /// Represents the standard output stream of the managed stream. When a formatter's Target is set to Out, it indicates that the formatted output should be written to the standard output stream, which is typically used for regular output, informational messages, or any content that does not represent an error condition.
    /// </summary>
    Out
}
