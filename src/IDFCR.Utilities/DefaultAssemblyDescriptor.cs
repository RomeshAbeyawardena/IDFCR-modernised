using System.Reflection;
using IDFCR.Utilities.Extensions;

namespace IDFCR.Utilities;

internal class DefaultAssemblyDescriptor<TEnum>(IReadOnlyDictionary<TEnum, Assembly[]> groupedAssemblies) : IAssemblyDescriptor<TEnum>
    where TEnum : Enum
{
    private readonly bool isFlagsEnum = typeof(TEnum).IsDefined(typeof(FlagsAttribute), inherit: false);

    public IEnumerable<Assembly> GetAssemblies(TEnum type)
    {
        bool overlaps = Convert.ToInt64(type) == 0;
        if (!overlaps && isFlagsEnum)
        {
            return [.. groupedAssemblies
                .Where(x => x.Key.Overlaps(type))
                .Distinct()
                .SelectMany(x => x.Value)];
        }

        if (groupedAssemblies.TryGetValue(type, out var assemblies))
        {
            return assemblies;
        }

        return [];
    }
}