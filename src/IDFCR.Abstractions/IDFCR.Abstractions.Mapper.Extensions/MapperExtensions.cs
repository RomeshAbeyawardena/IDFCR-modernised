using FastMember;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace IDFCR.Abstractions.Mapper.Extensions;

/// <summary>
/// Defines a set of extension methods for mapping objects that implement the <see cref="IMapper{TSource}"/> interface. These methods provide a standardized way to perform mapping operations, allowing for efficient copying of properties between source and target objects of the same type. The extension methods utilize the FastMember library to access and copy properties, ensuring high performance during mapping operations. This class is intended
/// </summary>
public static class MapperExtensions
{
    /// <summary>
    /// Sets the value of a property on the target object if the corresponding property on the source entity is not null or its default value. This method uses an expression to specify which property to get from the source entity and sets it on the target object if the conditions are met. The operation is conditional based on the <paramref name="canSet"/> parameter, allowing for flexible control over when the mapping should occur.
    /// </summary>
    /// <typeparam name="T">The type of the source entity.</typeparam>
    /// <typeparam name="TProp">The type of the property to be set.</typeparam>
    /// <param name="entity">The source entity from which to get the property value.</param>
    /// <param name="getEntityValueFactory">An expression to specify which property to get from the source entity.</param>
    /// <param name="target">The target object on which to set the property value.</param>
    /// <param name="canSet">A boolean value indicating whether the property should be set.</param>
    /// <exception cref="ArgumentException"></exception>
    public static void SetIfNotNullOrDefault<T, TProp>(this T entity, Expression<Func<T, TProp>> getEntityValueFactory, object target, bool canSet)
    {
        if (!canSet)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(getEntityValueFactory);
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(target);

        var value = getEntityValueFactory.Compile()(entity);

        if (value == null || EqualityComparer<TProp>.Default.Equals(value, default))
        {
            return;
        }

        var accessor = TypeAccessor.Create(target.GetType());
        FieldVisitor fieldVisitor = new();
        fieldVisitor.Visit(getEntityValueFactory);

        if (fieldVisitor.Member?.Name is not string memberName)
        {
            throw new ArgumentException("Expression must target a member access.", nameof(getEntityValueFactory));
        }

        accessor[target, memberName] = value;
    }

    // A thread-safe cache to store the specific members we care about for each type
    private static readonly Lazy<ConcurrentDictionary<Type, string[]>> _mappableMemberCache = new([]);
    /// <summary>
    /// Defines a mapping operation that copies values from a source object to a target object of the same type. This method is intended for use with types that implement the <see cref="IMapper{TSource}"/> interface, allowing for a standardized way to perform mapping operations without needing to manually map each property. The method uses the FastMember library to efficiently access and copy properties between the source and target objects.
    /// <para>
    /// It is ideal for scenarios where the source and target objects are of the same type and a straightforward copy of properties is desired. For more complex mapping scenarios, such as when the source and target types differ or when custom mapping logic is required, it is recommended to add your custom mappings below this method inside the <see cref="IMapper{TSource}.Map(TSource)"/>. implementation.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the source object.</typeparam>
    /// <typeparam name="TDestination">The type of the destination object.</typeparam>
    /// <param name="target">The target object to which values will be copied.</param>
    /// <param name="source">The source object from which values will be copied.</param>
    internal static void SingularMap<TSource, TDestination>(this TDestination target, TSource source)
    {
        if (source == null || target == null)
        {
            return;
        }

        var type = source.GetType();
        var sourceAccessor = TypeAccessor.Create(type);
        var sourceTypeNames = _mappableMemberCache.Value.GetOrAdd(type, (t) => [.. sourceAccessor.GetMembers().Where(m => m.CanWrite && m.CanRead).Select(m => m.Name)]);

        var destinationType = target.GetType();

        var destinationAccessor = TypeAccessor.Create(destinationType);
        var destinationTypeNames = _mappableMemberCache.Value.GetOrAdd(destinationType, (t) => [.. destinationAccessor.GetMembers().Where(m => m.CanWrite && m.CanRead).Select(m => m.Name)]);

        var destinationObjectAccessor = ObjectAccessor.Create(target);
        var sourceObjectAccessor = ObjectAccessor.Create(source);

        foreach (var name in destinationTypeNames.Where(x => sourceTypeNames.Contains(x)))
        {
            destinationAccessor[target, name] = sourceAccessor[source, name];
        }
    }
}
