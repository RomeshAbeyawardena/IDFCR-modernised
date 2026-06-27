namespace IDFCR.Utilities;

/// <summary>
/// Defines a static class that provides a method for building a switch-like structure using a builder pattern. The <see cref="SwitchBuilder"/> class contains a single static method, <see cref="Build{TKey, TValue}(Action{ISwitchBuilder{TKey, TValue}})"/>, which accepts a delegate that configures the switch builder and returns an instance of <see cref="ISwitch{TKey, TValue}"/> representing the built switch structure. This class serves as a convenient entry point for creating switch-like structures in applications and systems.
/// </summary>
public static class SwitchBuilder
{
    /// <summary>
    /// Builds a switch-like structure based on the specified configuration provided by the <paramref name="builderFactory"/> delegate, returning an instance of <see cref="ISwitch{TKey, TValue}"/> that can be used to evaluate keys and obtain corresponding values. The <paramref name="builderFactory"/> delegate is responsible for defining cases and an optional default case using the methods provided by the <see cref="ISwitchBuilder{TKey, TValue}"/> interface. Once built, the switch structure is immutable and cannot be modified.
    /// <para>The <paramref name="source"/> parameter provides an existing switch structure that can be used as a base for the new switch structure, allowing for modifications or extensions to the existing cases and default case.</para>
    /// </summary>
    /// <typeparam name="TKey">The type of the keys used to identify cases in the switch structure.</typeparam>
    /// <typeparam name="TValue">The type of the values associated with the keys in the switch structure.</typeparam>
    /// <param name="source">The source switch structure to be used as a base for the new switch structure.</param>
    /// <param name="builderFactory">A delegate that configures the switch builder.</param>
    /// <returns>An instance of <see cref="ISwitch{TKey, TValue}"/> representing the built switch structure.</returns>
    public static ISwitch<TKey, TValue> Build<TKey, TValue>(ISwitch<TKey, TValue> source, Action<ISwitchBuilder<TKey, TValue>> builderFactory)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(builderFactory);

        var builder = new DefaultSwitchBuilder<TKey, TValue>(source);

        builderFactory(builder);

        return builder.Build();
    }

    /// <summary>
    /// Builds a switch-like structure based on the specified configuration provided by the <paramref name="builderFactory"/> delegate, returning an instance of <see cref="ISwitch{TKey, TValue}"/> that can be used to evaluate keys and obtain corresponding values. The <paramref name="builderFactory"/> delegate is responsible for defining cases and an optional default case using the methods provided by the <see cref="ISwitchBuilder{TKey, TValue}"/> interface. Once built, the switch structure is immutable and cannot be modified.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys used to identify cases in the switch structure.</typeparam>
    /// <typeparam name="TValue">The type of the values associated with the keys in the switch structure.</typeparam>
    /// <param name="builderFactory">A delegate that configures the switch builder.</param>
    /// <returns>An instance of <see cref="ISwitch{TKey, TValue}"/> representing the built switch structure.</returns>
    public static ISwitch<TKey, TValue> Build<TKey, TValue>(Action<ISwitchBuilder<TKey, TValue>> builderFactory)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(builderFactory);

        var builder = new DefaultSwitchBuilder<TKey, TValue>();

        builderFactory(builder);

        return builder.Build();
    }
}
