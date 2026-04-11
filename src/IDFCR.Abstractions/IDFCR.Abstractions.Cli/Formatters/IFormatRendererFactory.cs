namespace IDFCR.Abstractions.Cli.Formatters;

/// <summary>
/// Represents a factory for creating format renderers that can render data of various types based on a given context. The IFormatRendererFactory interface defines the contract for format renderer factories, including a method to render a model of type T into a string representation based on the provided context, and a method to retrieve all available format renderers for a specific type T. Implementations of this interface can provide logic to determine which format renderer to use based on the context and the structure of the data being rendered, allowing for flexible and customizable rendering of data in different formats (e.g., list, table, custom) based on the specified rendering style and custom styles defined in the context.
/// </summary>
public interface IFormatRendererFactory
{
    /// <summary>
    /// Renders a model of type T into a string representation based on the provided context. The method takes an IFormatRendererContext that contains information about the rendering style and any custom styles, and a model of type T that represents the data to be rendered. The implementation of this method should determine which format renderer to use based on the context and the structure of the data, and return a string representation of the rendered data according to the specified rendering style and custom styles defined in the context.
    /// </summary>
    /// <typeparam name="T">The type of the model to be rendered.</typeparam>
    /// <param name="context">The context that contains information about the rendering style and any custom styles.</param>
    /// <param name="model">The model of type T to be rendered.</param>
    /// <returns>A string representation of the rendered model.</returns>
    string Render<T>(IFormatRendererContext context, T model);
    /// <summary>
    /// Gets all available format renderers for a specific type T. This method returns an enumerable collection of <see cref="IFormatRenderer{T}"/> instances that can be used to render data of type T based on the provided context. The implementation of this method should return all format renderers that are applicable for the specified type T, allowing the caller to choose from a variety of rendering options based on the context and the structure of the data being rendered.
    /// </summary>
    /// <typeparam name="T">The type of the model for which to retrieve format renderers.</typeparam>
    /// <returns>An enumerable collection of <see cref="IFormatRenderer{T}"/> instances.</returns>
    IEnumerable<IFormatRenderer<T>> GetRenderProviders<T>();
}
