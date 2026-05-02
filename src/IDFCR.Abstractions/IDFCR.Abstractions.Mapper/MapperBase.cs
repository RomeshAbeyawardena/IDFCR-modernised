namespace IDFCR.Abstractions.Mapper;

/// <summary>
/// Provides a base class for mapping objects of type TSource using a standardized mapping interface.
/// </summary>
/// <remarks>This abstract class defines the core mapping contract for types that implement <see cref="IMapper{TSource}"/>.
/// Derived classes should implement the mapping logic specific to their target type. The class provides helper methods
/// to facilitate mapping with or without additional parameters, and supports both parameterless and parameterized
/// construction of mapper instances.</remarks>
/// <typeparam name="TSource">The type of the source object to be mapped.</typeparam>
public abstract class MapperBase<TSource>() : IMapper<TSource>
{
    /// <summary>
    /// Gets the current instance cast to the specified source type.
    /// </summary>
    /// <remarks>Use this property to access the current object as the generic source type parameter. This is
    /// useful in scenarios where the derived class needs to expose itself as the generic type.</remarks>
    protected TSource Source
    {
        get
        {
            return (TSource)(object)this;
        }
    }

    /// <summary>
    /// Creates an instance of the specified mapper type and maps the provided source object using that mapper.
    /// </summary>
    /// <remarks>The method uses reflection to instantiate the mapper type. If the mapper cannot be created or
    /// does not implement the required interface, the method returns null.</remarks>
    /// <typeparam name="T">The type of mapper to use for mapping the source object. Must implement <see cref="IMapper{TSource}"/> and be a reference
    /// type.</typeparam>
    /// <param name="parameters">An array of arguments to pass to the constructor of the mapper type.</param>
    /// <returns>An instance of the mapper type with the source object mapped, or null if the mapper could not be created.</returns>
    public T? Map<T>(params object[] parameters) where T : class, IMapper<TSource>
    {
        var instance = Activator.CreateInstance(typeof(T), parameters);

        if (instance is null || instance is not T mapper)
        {
            return null;
        }

        mapper.Map(Source);
        return mapper;
    }

    /// <summary>
    /// Creates a new instance of type T and maps the values from the specified source object to it.
    /// </summary>
    /// <remarks>This method requires that T implements the <see cref="IMapper{TSource}"/> interface to perform the mapping
    /// operation. The method creates a new instance of T using its parameterless constructor and invokes the Map method
    /// to copy values from the source object.</remarks>
    /// <typeparam name="T">The type of the object to create and map to. Must implement <see cref="IMapper{TSource}"/> and have a parameterless
    /// constructor.</typeparam>
    /// <returns>A new instance of type T with values mapped from the source object.</returns>
    public T Map<T>() where T : IMapper<TSource>, new()
    {
        var result = new T();
        result.Map(Source);

        return result;
    }

    /// <summary>
    /// Maps the values from the specified source object to the current instance. This method must be implemented by derived classes to define the specific mapping logic for the source type. The implementation should copy relevant properties and values from the source object to the current instance, allowing for transformation and mapping of data as needed. The Map method serves as the core mapping function that enables derived classes to perform custom mapping operations based on their specific requirements and use cases.
    /// </summary>
    /// <param name="source">The source object from which values will be mapped to the current instance.</param>
    public abstract void Map(TSource source);
}


/// <summary>
/// Represents a base class for implementing the <see cref="IMapper{TSource, TSource1}"/> interface, providing a foundation for mapping between two source types. This abstract class implements the mapping logic for both source types, allowing derived classes to focus on the specific mapping implementation. The Map method with two source parameters is implemented to call the individual Map methods for each source type, enabling a structured approach to mapping from multiple sources. By inheriting from this base class, developers
/// </summary>
/// <typeparam name="TSource">The type of the first source object to be mapped.</typeparam>
/// <typeparam name="TSource1">The type of the second source object to be mapped.</typeparam>
public abstract class MapperBase<TSource, TSource1> : MapperBase<TSource>, IMapper<TSource, TSource1>
    where TSource : IMapper<TSource>
    where TSource1 : IMapper<TSource1>
{
    /// <inheritdoc />
    public abstract void Map(TSource1 secondSource);

    /// <summary>
    /// Maps the specified source objects to the current instance. This method allows for mapping from both source types to the current instance by calling the individual Map methods for each source type. By using this method, developers can efficiently combine or transform data from multiple sources, improving the versatility and reusability of mapping operations within applications and systems that utilize this interface.
    /// </summary>
    /// <param name="source">The first source object to be mapped.</param>
    /// <param name="secondSource">The second source object to be mapped.</param>
    public void Map(TSource source, TSource1 secondSource)
    {
        Map(source);
        Map(secondSource);
    }
}
