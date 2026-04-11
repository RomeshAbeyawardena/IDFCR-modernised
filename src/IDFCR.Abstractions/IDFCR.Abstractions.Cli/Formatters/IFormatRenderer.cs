using IDFCR.Abstractions.Cli.Extensions;
using System.Text;

namespace IDFCR.Abstractions.Cli.Formatters;

/// <summary>
/// Represents a base class for format renderers that provides common functionality for rendering data of type T based on the provided context. This abstract class implements the IFormatRenderer interface and serves as a foundation for specific format renderer implementations that can be used to render data in different formats (e.g., list, table, custom) based on the specified context and the structure of the data being rendered. The FormatTableRendererBase class provides a constructor that accepts an array of TableField instances, which define the fields to be included in the rendered output for the type T, and it also provides an implementation of the Fields property to access these fields. The CanRender and Render methods are left abstract for derived classes to implement their specific rendering logic based on the context and the model being rendered.
/// </summary>
/// <typeparam name="T">Type of the model to be rendered.</typeparam>
/// <param name="fields">An array of TableField instances that define the fields to be included in the rendered output for the type T.</param>
public abstract class FormatTableRendererBase<T>(params IEnumerable<TableField<T>> fields) : IFormatRenderer<T>
{
    private readonly List<TableField<T>> fields = [.. fields];
    /// <inheritdoc />
    public IEnumerable<TableField<T>> Fields => fields;
    /// <inheritdoc />
    public virtual bool CanRender(IFormatRendererContext context, T model)
    {
        return context.RenderStyle == FormatRenderStyle.Table && Fields.Any();
    }
    /// <inheritdoc />
    public string Render(IFormatRendererContext context, T model)
    {
        StringBuilder stringBuilder = new();
        if (context is not TableFormatterContext tableFormatterContext)
        {
            throw new InvalidOperationException("");
        }

        Func<object?, string?>? defaultFormat = x => x?.ToString();
        FieldVisitor visitor = new();

        for(var i=0;i< Fields.Count(); i++)
        {
            var tableField = Fields.ElementAt(i);

            if (!tableFormatterContext.TableFields.TryGetValue(i, out var tableFieldInfo))
            {
                visitor.Visit(tableField.Field);

                if (!visitor.HasProperty)
                {
                    continue;
                }

                var modelValue = visitor.Property.GetValue(model);
                var format = tableField.Format ?? defaultFormat;
                var formattedValue = format(modelValue);//.Limit(tableField.RowWidth.GetValueOrDefault(tableFormatterContext.MinimumColumnWidth.GetValueOrDefault()));

                tableFieldInfo = new(modelValue, format,  formattedValue, tableField.Title);

                tableFormatterContext.TableFields.TryAdd(i, tableFieldInfo);
            }

            stringBuilder.Append($"\t|\t {tableFieldInfo.FormattedValue}");
        }

        return stringBuilder.ToString();
    }
}

/// <summary>
/// Represents a format renderer that is responsible for rendering data of type T based on the provided context. The IFormatRenderer interface defines the contract for format renderers, including a property to access the table fields for the type T, a method to determine if the renderer can render the given model based on the context, and a method to perform the actual rendering of the model into a string representation. Implementations of this interface can provide custom logic for rendering data in different formats (e.g., list, table, custom) based on the specified context and the structure of the data being rendered.
/// </summary>
/// <typeparam name="T">Type of the model to be rendered.</typeparam>
public interface IFormatRenderer<T>
{
    /// <summary>
    /// Gets an array of TableField instances that define the fields to be included in the rendered output for the type T. Each TableField represents a specific property or attribute of the type T that should be included in the rendered output, along with any formatting options or specifications for how that field should be displayed. This property allows format renderers to access the necessary information about the structure of the data being rendered, enabling them to generate appropriate output based on the defined fields and their associated formatting rules.
    /// </summary>
    IEnumerable<TableField<T>> Fields { get; }
    /// <summary>
    /// Determines whether the format renderer can render the given model of type T based on the provided context. This method allows the format renderer to evaluate the context and the structure of the model to determine if it is capable of rendering the data in the desired format. The implementation of this method can include logic to check for specific conditions, such as the presence of required fields, compatibility with the specified rendering style, or any other criteria that may affect the ability of the renderer to generate a valid output for the given model and context.
    /// </summary>
    /// <param name="context">The context in which the format renderer operates, providing information about the custom style and rendering style to be used when formatting data for display.</param>
    /// <param name="model">The model of type T to be rendered.</param>
    /// <returns>True if the format renderer can render the given model based on the provided context; otherwise, false.</returns>
    bool CanRender(IFormatRendererContext context, T model);

    /// <summary>
    /// Renders the given model of type T into a string representation based on the provided context. This method is responsible for generating the formatted output for the model, taking into account the specified rendering style (Custom, List, Table) and any custom styles defined in the context. The implementation of this method can include logic to format the data according to the defined fields, apply any necessary formatting rules, and produce a string representation that is suitable for display based on the context and the structure of the model being rendered.
    /// </summary>
    /// <param name="context">The context in which the format renderer operates, providing information about the custom style and rendering style to be used when formatting data for display.</param>
    /// <param name="model">The model of type T to be rendered.</param>
    /// <returns>A string representation of the rendered model based on the provided context.</returns>
    string Render(IFormatRendererContext context, T model);
}
