using System.Collections.Frozen;
using System.Reflection;

namespace IDFCR.Utilities;

internal class DefaultAssemblyDescriptorBuilder<TEnum> : IAssemblyDescriptorBuilder<TEnum>
    where TEnum : Enum
{
    private readonly Dictionary<TEnum, List<Assembly>> groupedAssemblies = [];

    public IAssemblyDescriptorBuilder<TEnum> Append(TEnum type, params Assembly[] assemblies)
    {
        // If the type doesn't exist, create the list and add it to the dictionary immediately
        if (!groupedAssemblies.TryGetValue(type, out var assemblyList))
        {
            assemblyList = [];
            groupedAssemblies[type] = assemblyList;
        }

        // Now safely append the new assemblies
        assemblyList.AddRange(assemblies);

        return this;
    }

    public IAssemblyDescriptorBuilder<TEnum> Append<T>(TEnum type)
    {
        return Append(type, typeof(T).Assembly);
    }

    public IAssemblyDescriptor<TEnum> Build()
    {
        IReadOnlyDictionary<TEnum, Assembly[]> dict = groupedAssemblies
            .ToFrozenDictionary(x => x.Key, x => x.Value.Distinct().ToArray());

        return new DefaultAssemblyDescriptor<TEnum>(dict);
    }
}
