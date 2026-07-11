using System.Reflection;

namespace IDFCR.Utilities;

/// <summary>
/// Represents a builder for creating an assembly descriptor based on a specific enum type.
/// </summary>
/// <typeparam name="TEnum"></typeparam>
public interface IAssemblyDescriptorBuilder<TEnum>
    where TEnum : Enum
{
    /// <summary>
    /// Appends the specified assemblies to the descriptor for the given enum type.
    /// </summary>
    /// <param name="type">The enum value representing the type of assemblies to append.</param>
    /// <param name="assemblies">The assemblies to append to the descriptor.</param>
    /// <returns>The current instance of the assembly descriptor builder.</returns>
    IAssemblyDescriptorBuilder<TEnum> Append(TEnum type, params Assembly[] assemblies);
    /// <summary>
    /// Appends the assembly containing the specified type to the descriptor for the given enum type.
    /// </summary>
    /// <typeparam name="T">The type whose containing assembly is to be appended.</typeparam>
    /// <param name="type">The enum value representing the type of assemblies to append.</param>
    /// <returns>The current instance of the assembly descriptor builder.</returns>
    IAssemblyDescriptorBuilder<TEnum> Append<T>(TEnum type);
    /// <summary>
    /// Builds the assembly descriptor.
    /// </summary>
    /// <returns>The constructed assembly descriptor.</returns>
    IAssemblyDescriptor<TEnum> Build();
}
