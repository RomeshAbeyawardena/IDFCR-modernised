namespace IDFCR.Abstractions.Cli.Formatters;

/// <summary>
/// Represents the context in which a format renderer operates, providing information about the custom style and rendering style to be used when formatting data for display. The CustomStyle property allows for specifying a custom rendering style that can be defined by the user or the implementation of the format renderer, while the RenderStyle property indicates the desired rendering style (Custom, List, or Table) to be applied when formatting the data. This context can be used by format renderers to determine how to render the data based on the specified styles, allowing for flexible and customizable data presentation that can adapt to different requirements and preferences.
/// </summary>
public interface IFormatRendererContext
{
    /// <summary>
    /// Gets a string representing a custom rendering style that can be defined by the user or the implementation of the format renderer. This property allows for maximum flexibility in how data is formatted and displayed, as it does not adhere to any predefined structure or format. When this property is set, the format renderer can implement custom logic to determine how to render the data based on specific requirements or preferences, allowing for unique and tailored presentations of the data that may not fit into standard list or table formats.
    /// </summary>
    string? CustomStyle { get; }
    /// <summary>
    /// Gets a value from the FormatRenderStyle enumeration that indicates the desired rendering style (Custom, List, or Table) to be applied when formatting data for display. This property allows format renderers to determine how to render the data based on the specified rendering style, enabling flexible and customizable data presentation that can adapt to different requirements and preferences. Depending on the value of this property, the format renderer can implement logic to render the data in a custom format, as a simple list, or in a tabular format with rows and columns.
    /// </summary>
    FormatRenderStyle RenderStyle { get; }
}
