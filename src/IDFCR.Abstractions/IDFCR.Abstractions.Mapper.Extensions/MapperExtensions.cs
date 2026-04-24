using FastMember;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace IDFCR.Abstractions.Mapper.Extensions;
/// <summary>
/// Defines a set of extension methods for mapping objects that implement the <see cref="IMapper{TSource}"/> interface. These methods provide a standardized way to perform mapping operations, allowing for efficient copying of properties between source and target objects of the same type. The extension methods utilize the FastMember library to access and copy properties, ensuring high performance during mapping operations. This class is intended
/// </summary>
public static class MapperExtensions
{
    // A thread-safe cache to store the specific members we care about for each type
    private static readonly ConcurrentDictionary<Type, string[]> _mappableMemberCache = [];
    /// <summary>
    /// Defines a mapping operation that copies values from a source object to a target object of the same type. This method is intended for use with types that implement the <see cref="IMapper{TSource}"/> interface, allowing for a standardized way to perform mapping operations without needing to manually map each property. The method uses the FastMember library to efficiently access and copy properties between the source and target objects.
    /// <para>
    /// It is ideal for scenarios where the source and target objects are of the same type and a straightforward copy of properties is desired. For more complex mapping scenarios, such as when the source and target types differ or when custom mapping logic is required, it is recommended to add your custom mappings below this method inside the <see cref="IMapper{TSource}.Map(TSource)"/>. implementation.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the source and target objects. This type must implement the <see cref="IMapper{TSource}"/> interface.</typeparam>
    /// <param name="target">The target object to which values will be copied.</param>
    /// <param name="source">The source object from which values will be copied.</param>
    public static void SingularMap<TSource, TDestination>(this TDestination target, TSource source)
    {
        if (source == null || target == null)
        {
            return;
        }

        var type = source.GetType();
        var sourceAccessor = TypeAccessor.Create(type);
        var sourceTypeNames = _mappableMemberCache.GetOrAdd(type, (t) => [.. sourceAccessor.GetMembers().Where(m => m.CanWrite && m.CanRead).Select(m => m.Name)]);

        var destinationType = target.GetType();

        var destinationAccessor = TypeAccessor.Create(destinationType);
        var destinationTypeNames = _mappableMemberCache.GetOrAdd(destinationType, (t) => [.. destinationAccessor.GetMembers().Where(m => m.CanWrite && m.CanRead).Select(m => m.Name)]);

        var destinationObjectAccessor = ObjectAccessor.Create(target);
        var sourceObjectAccessor = ObjectAccessor.Create(source);

        foreach (var name in destinationTypeNames.Where(x => sourceTypeNames.Contains(x)))
        {
            destinationAccessor[target, name] = sourceAccessor[source, name];
        }
    }
}
