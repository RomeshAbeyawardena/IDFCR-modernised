using System.Reflection;

namespace IDFCR.Utilities;

internal class DefaultAssemblyDescriptor<TEnum>(IReadOnlyDictionary<TEnum, Assembly[]> groupedAssemblies) : IAssemblyDescriptor<TEnum>
    where TEnum : Enum
{
    public IEnumerable<Assembly> GetAssemblies(TEnum type)
    {
        var flagAttribute = typeof(TEnum).GetCustomAttribute<FlagsAttribute>();

        if (flagAttribute is not null)
        {
            return [.. groupedAssemblies
                .Where(x => x.Key.HasFlag(type))
                .SelectMany(x => x.Value)];
        }

        if (groupedAssemblies.TryGetValue(type, out var assemblies))
        {
            return assemblies;
        }

        return [];
    }
}