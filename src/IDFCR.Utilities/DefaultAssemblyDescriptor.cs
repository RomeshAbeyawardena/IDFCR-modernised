using System.Reflection;

namespace IDFCR.Utilities;

internal class DefaultAssemblyDescriptor<TEnum>(IReadOnlyDictionary<TEnum, Assembly[]> groupedAssemblies) : IAssemblyDescriptor<TEnum>
    where TEnum : Enum
{
    public IEnumerable<Assembly> GetAssemblies(TEnum type)
    {
        if (groupedAssemblies.TryGetValue(type, out var assemblies))
        {
            return assemblies;
        }

        return [];
    }
}