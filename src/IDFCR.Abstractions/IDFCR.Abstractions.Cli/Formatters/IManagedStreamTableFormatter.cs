namespace IDFCR.Abstractions.Cli.Formatters;

/// <summary>
/// Represents a formatter that writes formatted output to a managed stream in a tabular format. This class inherits from ManagedStreamFormatterBase, which provides common functionality for managing the output stream and accumulating formatted output in a StringBuilder. The ManagedStreamTableFormatter specifically implements the logic to format collections of data as tables, allowing for structured display of information in the output stream. It checks if the provided value is an IEnumerable and uses reflection to determine the generic type of the collection, invoking the appropriate formatting method to generate the table representation of the data. If the value is not an IEnumerable or does not implement a generic IEnumerable interface, it throws an ArgumentException to indicate that the value cannot be formatted as a table.
/// </summary>
public interface IManagedStreamTableFormatter
{
    /// <summary>
    /// Gets the context for the format renderer, which contains information about the custom style and rendering style to be used when formatting data for display. The context allows the formatter to determine how to format the data based on the specified rendering style (e.g., Custom, List, Table) and any custom styles defined in the context. This property is essential for the formatter to generate appropriate output based on the context and the structure of the data being formatted.
    /// </summary>
    IFormatRendererContext Context { get; }
    /// <summary>
    /// Formats a collection of values as a table and accumulates the formatted output in the StringBuilder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="cancellationToken">A cancellation token to observe while formatting.</param>
    /// <returns>A task that represents the asynchronous formatting operation.</returns>
    Task FormatTableAsync<T>(IEnumerable<T> values, CancellationToken cancellationToken);
}
