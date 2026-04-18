namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Defines a static class that provides extension methods for formatting various types of data in a command-line interface (CLI) context. These formatters can be used to convert raw data into human-readable strings, making it easier for users to understand and interpret the output of CLI commands. For example, the FormatDate method can be used to format date and time values in a consistent way, providing default placeholders for null or invalid inputs. By centralizing formatting logic in this class, developers can ensure that all CLI output is presented in a clear and standardized manner across different commands and operations within applications and systems that utilize CLI interfaces.
/// </summary>
public static class Formatters
{
    /// <summary>
    /// Formats a date value for display in a CLI context. This method takes a default placeholder string to be used when the input value is null, and an optional invalid placeholder string to be used when the input value is not a valid date format. The method returns a function that takes an object as input and returns a formatted string representation of the date if the input is a valid DateTime or DateTimeOffset, or the appropriate placeholder if the input is null or invalid. By using this formatter, developers can ensure that date values are consistently formatted and displayed in a user-friendly manner within CLI applications and systems.
    /// </summary>
    /// <param name="defaultPlaceholder">The placeholder string to use when the input value is null.</param>
    /// <param name="invalidPlaceholder">The placeholder string to use when the input value is not a valid date format.</param>
    /// <returns>A function that formats date values for display in a CLI context.</returns>
    public static Func<object?, string?>? FormatDate(string defaultPlaceholder, string invalidPlaceholder = "Invalid format")
    {
        return rd => rd is null
                            ? defaultPlaceholder
                            : rd is DateTimeOffset offset
                            ? offset.DateTime.ToLocalTime().ToShortDateString()
                            : rd is DateTime date
                                ? date.ToLocalTime().ToShortDateString()
                                    : invalidPlaceholder;
    }
}
