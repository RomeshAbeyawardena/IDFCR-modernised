namespace IDFCR.Utilities;

/// <summary>
/// Defines a static class for building assembly descriptors based on a specific enum type.
/// </summary>
public static class AssemblyDescriptorBuilder
{
    /// <summary>
    /// Creates a new instance of an assembly descriptor builder for the specified enum type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type for which to create the assembly descriptor builder.</typeparam>
    /// <returns>A new instance of an assembly descriptor builder for the specified enum type.</returns>
    public static IAssemblyDescriptorBuilder<TEnum> Create<TEnum>()
        where TEnum : Enum
    {
        return new DefaultAssemblyDescriptorBuilder<TEnum>();
    }

    /// <summary>
    /// Builds an assembly descriptor for the specified enum type using the provided configuration action.
    /// </summary>
    /// <typeparam name="TEnum">The enum type for which to build the assembly descriptor.</typeparam>
    /// <param name="configure">The action to configure the assembly descriptor builder.</param>
    /// <returns>The constructed assembly descriptor.</returns>
    public static IAssemblyDescriptor<TEnum> Build<TEnum>(Action<IAssemblyDescriptorBuilder<TEnum>> configure)
        where TEnum : Enum
    {
        var builder = Create<TEnum>();
        configure(builder);
        return builder.Build();
    }
}
