using System.Linq.Expressions;
using System.Reflection;

namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Represents a field in a table, including the expression to access the field, an optional formatting function, an optional title for the column, and an optional row width for display purposes.
/// </summary>
/// <typeparam name="T"></typeparam>
public class TableField<T>
{
    /// <summary>
    /// Gets or sets the expression to access the field value from an instance of type T. This is a required property that must be provided when creating an instance of TableField.
    /// </summary>
    public required Expression<Func<T, object?>> Field { get; init; }

    /// <summary>
    /// Gets or sets the function to format the field value for display. This is an optional property.
    /// </summary>
    public Func<object?, string?>? Format { get; init; }

    /// <summary>
    /// Gets or sets the title for the column in the table. This is an optional property. If not provided, the title can be derived from the field expression or left blank.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Gets or sets the row width for display purposes. This is an optional property that can be used to specify the width of the column when rendering the table. If not provided, a default width can be used or the width can be determined automatically based on the content.
    /// </summary>
    public int? RowWidth { get; set; }

    /// <summary>
    /// Gets or sets the metadata information for a property.
    /// </summary>
    public PropertyInfo? Property { get; set; }

    /// <summary>
    /// Gets a value indicating whether the TableField has a valid property associated with it. This is determined by checking if the Property property is not null. If the Property is null, it means that the field expression did not successfully resolve to a property, and therefore the TableField does not have a valid property to access for retrieving values.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(Property))]
    public bool HasProperty => Property is not null;
}
