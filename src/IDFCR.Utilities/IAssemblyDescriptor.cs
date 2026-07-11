using System.Reflection;

namespace IDFCR.Utilities;

/// <summary>
/// Represents a descriptor for assemblies based on a specific enum type.
/// </summary>
/// <typeparam name="TEnum"></typeparam>
public interface IAssemblyDescriptor<TEnum>
    where TEnum : Enum
{
    /// <summary>
    /// Gets the assemblies associated with the specified enum type.
    /// </summary>
    /// <param name="type">The enum value representing the type of assemblies to retrieve.</param>
    /// <returns>A collection of assemblies associated with the specified enum type.</returns>
    IEnumerable<Assembly> GetAssemblies(TEnum type);
}
