namespace IDFCR.Abstractions.Cli.Formatters;

/// <summary>
/// Enumerates the different styles of rendering formats that can be used when formatting data for display. The List style represents a simple list format, while the Table style represents a tabular format with rows and columns. This enumeration can be used to specify the desired rendering style when implementing format renderers that support multiple output formats, allowing for flexible and customizable data presentation based on the context in which the data is being displayed.
/// </summary>
public enum FormatRenderStyle
{
    /// <summary>
    /// Represents a custom rendering style that can be defined by the user or the implementation of the format renderer. This style allows for maximum flexibility in how data is formatted and displayed, as it does not adhere to any predefined structure or format. When this style is selected, the format renderer can implement custom logic to determine how to render the data based on specific requirements or preferences, allowing for unique and tailored presentations of the data that may not fit into standard list or table formats.
    /// </summary>
    Custom,
    /// <summary>
    /// Represents a simple list rendering style where each item is displayed on a separate line or in a sequential manner. This style is suitable for displaying collections of items in a straightforward and linear format, making it easy to read and understand the individual elements of the collection. When this style is selected, the format renderer can implement logic to iterate through the collection and render each item accordingly, without the need for complex formatting or structuring.
    /// </summary>
    List,
    /// <summary>
    /// Represents a tabular rendering style where data is organized into rows and columns, similar to a table. This style is ideal for displaying structured data with multiple attributes or properties, allowing for easy comparison and analysis of the data. When this style is selected, the format renderer can implement logic to create a table-like structure, with headers for each column and rows for each item in the collection, providing a clear and organized presentation of the data.
    /// </summary>
    Table
}
