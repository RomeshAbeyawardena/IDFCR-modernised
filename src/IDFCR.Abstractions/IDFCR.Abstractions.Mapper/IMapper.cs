namespace IDFCR.Abstractions.Mapper;

/// <summary>
/// Represents a mapping interface that defines methods for mapping between a source type and a target type. This interface provides a contract for implementing classes to perform mapping operations, allowing for the transformation of data from one type to another. The Map methods allow for both parameterized and parameterless mapping, while the Map method with a source parameter allows for mapping from a specific source instance. By implementing this interface, developers can create flexible and reusable mapping logic that can be used across different parts of an application or system, facilitating data transformation and improving code maintainability.
/// </summary>
/// <typeparam name="TSource">The type of the source object to be mapped.</typeparam>
public interface IMapper<TSource>
{
    /// <summary>
    /// Maps the source object to an instance of the specified target type T, using the provided parameters for the mapping operation. This method allows for flexible mapping scenarios where additional parameters may be needed to perform the mapping correctly. The target type T must implement the <see cref="IMapper{TSource}"/> interface, ensuring that it can be mapped from the source type. By using this method, developers can create complex mapping logic that can handle various mapping scenarios, improving the versatility and reusability of mapping operations within applications and systems that utilize this interface.
    /// </summary>
    /// <typeparam name="T">The type of the target object to map to.</typeparam>
    /// <param name="parameters">An array of parameters to be used during the mapping operation.</param>
    /// <returns>An instance of the target type T, or null if the mapping could not be performed.</returns>
    T? Map<T>(params object[] parameters)
        where T : class, IMapper<TSource>;
    
    /// <summary>
    /// Maps the source object to an instance of the specified target type T, using the default constructor of the target type. This method allows for parameterless mapping scenarios where no additional parameters are needed to perform the mapping. The target type T must implement the <see cref="IMapper{TSource}"/> interface and have a parameterless constructor, ensuring that it can be mapped from the source type. By using this method, developers can create simple mapping logic that can handle basic mapping scenarios, improving the versatility and reusability of mapping operations within applications and systems that utilize this interface.
    /// </summary>
    /// <typeparam name="T">The type of the target object to map to.</typeparam>
    /// <returns>An instance of the target type T, or null if the mapping could not be performed.</returns>
    T Map<T>()
        where T : IMapper<TSource>, new();
    /// <summary>
    /// Maps the specified source object to the current instance. This method allows for updating the current instance with values from the source object, enabling in-place mapping scenarios. By using this method, developers can efficiently update existing instances with new data from the source object, improving the versatility and reusability of mapping operations within applications and systems that utilize this interface.
    /// </summary>
    void Map(TSource source);
}
