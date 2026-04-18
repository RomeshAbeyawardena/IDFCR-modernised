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
    /// 
    /// </summary>
    /// <param name="source"></param>
    public abstract void Map(TSource source);
}
