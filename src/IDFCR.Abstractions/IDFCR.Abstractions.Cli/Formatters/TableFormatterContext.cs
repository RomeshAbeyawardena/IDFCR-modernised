using IDFCR.Abstractions.Cli.Extensions;

namespace IDFCR.Abstractions.Cli.Formatters;

public record TableFieldInfo(object? Model, Func<object?, string?>? Format, string? FormattedValue, string? Title)
{
}

/// <summary>
/// Provides contextual information for formatting operations that render output in a table format.
/// </summary>
/// <remarks>This context is typically used by formatters or renderers that require additional style or rendering
/// options when producing table-based output. It encapsulates style preferences and rendering modes relevant to table
/// formatting scenarios.</remarks>
public class TableFormatterContext : IFormatRendererContext
{
    public int? MinimumColumnWidth { get; init; }

    /// <inheritdoc />
    public string? CustomStyle { get; init; }
    /// <inheritdoc />
    public FormatRenderStyle RenderStyle { get; init; } = FormatRenderStyle.Table;

    public IDictionary<int, TableFieldInfo> TableFields { get; set; }
}
