namespace IDFCR.Abstractions.Mapper.Extensions;

/// <summary>
/// Represents a base class for mapping objects of type TSource using a standardized mapping interface, with support for automatic property mapping. This class extends the functionality of <see cref="MapperBase{TSource}"/> by providing a helper method, <see cref="SingularMap(TSource)"/>, which utilizes the FastMember library to efficiently copy properties from the source object to the target object. The SingularMap method is designed to be used in scenarios where a straightforward copy of properties is desired between objects of the same type, allowing developers to easily implement mapping logic without needing to manually map each property. By inheriting from this base class, developers
/// </summary>
/// <typeparam name="TSource"></typeparam>
public abstract class AutoMapperBase<TSource> : MapperBase<TSource>
{
    /// <summary>
    /// Maps properties from the source object to the target object using the FastMember library. This method is intended for use in scenarios where the source and target objects are of the same type and a straightforward copy of properties is desired. The method utilizes a thread-safe cache to store mappable member names for each type, ensuring efficient mapping operations. For more complex mapping scenarios, such as when the source and target types differ or when custom mapping logic is required, it is recommended to add your custom mappings below this method inside the <see cref="IMapper{TSource}.Map(TSource)"/> implementation.
    /// </summary>
    /// <param name="source"></param>
    protected virtual void SingularMap(TSource source)
    {
        ((TSource)(object)this).SingularMap(source);
    }
}
