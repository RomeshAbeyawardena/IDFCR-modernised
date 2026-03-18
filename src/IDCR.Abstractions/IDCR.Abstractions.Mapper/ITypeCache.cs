using System.Reflection;

namespace IDCR.Abstractions.Mapper;

public interface ITypeCache
{
    Type Type { get; }
    IEnumerable<PropertyInfo> Properties { get; }
}

public interface ITypeCache<T> : ITypeCache
{
    
}

public class TypeCache(Type type) : ITypeCache
{
    private readonly Lazy<IEnumerable<PropertyInfo>> properties = new(() => type.GetProperties());
    public Type Type => type;

    public virtual IEnumerable<PropertyInfo> Properties => properties.Value;
}

public class TypeCache<T>(IEnumerable<PropertyInfo>? copiedProperties) 
    : TypeCache(typeof(T)), ITypeCache<T>
{
    public override IEnumerable<PropertyInfo> Properties => copiedProperties ?? base.Properties;
}