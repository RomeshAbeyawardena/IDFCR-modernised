using System.Collections.Frozen;

namespace IDFCR.Utilities;

/// <summary>
/// Represents a builder for creating a switch-like structure that maps keys to values based on specified cases and an optional default case. The <see cref="ISwitchBuilder{TKey, TValue}"/> interface provides methods for defining cases and an optional default case, allowing developers to build a switch-like structure that can be used to retrieve values based on keys. The resulting <see cref="ISwitch{TKey, TValue}"/> instance can be used to evaluate keys and obtain corresponding values based on the defined cases and default case, providing a flexible and extensible way to implement switch-like behavior in applications and systems.
/// </summary>
/// <typeparam name="TKey">The type of the keys used to identify cases.</typeparam>
/// <typeparam name="TValue">The type of the values associated with each case.</typeparam>
public interface ISwitchBuilder<TKey, TValue>
    where TKey: notnull
{
    /// <summary>
    /// Defines a case for the switch-like structure, associating a specific key with a value factory function that generates the corresponding value when the key is evaluated. If the specified key already exists in the switch structure, the existing value factory will be replaced with the new one.
    /// </summary>
    /// <param name="key">The key that identifies the case.</param>
    /// <param name="valueFactory">A function that generates the value associated with the specified key.</param>
    /// <returns>The current instance of the switch builder, allowing for method chaining.</returns>
    ISwitchBuilder<TKey, TValue> CaseWhen(TKey key, Func<TKey, TValue> valueFactory);
    /// <summary>
    /// Defines a case for the switch-like structure, associating a specific key with a fixed value. If the specified key already exists in the switch structure, the existing value will be replaced with the new one.
    /// </summary>
    /// <param name="key">The key that identifies the case.</param>
    /// <param name="value">The value associated with the specified key.</param>
    /// <returns>The current instance of the switch builder, allowing for method chaining.</returns>
    ISwitchBuilder<TKey, TValue> CaseWhen(TKey key, TValue value);

    /// <summary>
    /// Defines a default case for the switch-like structure, specifying a value factory function that generates the corresponding value when no defined cases match the evaluated key. If a default case has already been defined, it will be replaced with the new one.
    /// </summary>
    /// <param name="valueFactory">A function that generates the value for the default case.</param>
    /// <returns>The current instance of the switch builder, allowing for method chaining.</returns>
    ISwitchBuilder<TKey, TValue> Else(Func<TKey, TValue> valueFactory);
    /// <summary>
    /// Defines a default case for the switch-like structure, specifying a fixed value to be used when no defined cases match the evaluated key. If a default case has already been defined, it will be replaced with the new one.
    /// </summary>
    /// <param name="value">The value for the default case.</param>
    /// <returns>The current instance of the switch builder, allowing for method chaining.</returns>
    ISwitchBuilder<TKey, TValue> Else(TValue value);

    /// <summary>
    /// Builds the switch-like structure based on the defined cases and optional default case, returning an instance of <see cref="ISwitch{TKey, TValue}"/> that can be used to evaluate keys and obtain corresponding values. Once built, the switch structure is immutable and cannot be modified.
    /// </summary>
    /// <returns>An instance of <see cref="ISwitch{TKey, TValue}"/> representing the built switch structure.</returns>
    ISwitch<TKey, TValue> Build();
}

/// <summary>
/// Defines a static class that provides a method for building a switch-like structure using a builder pattern. The <see cref="SwitchBuilder"/> class contains a single static method, <see cref="Build{TKey, TValue}(Action{ISwitchBuilder{TKey, TValue}})"/>, which accepts a delegate that configures the switch builder and returns an instance of <see cref="ISwitch{TKey, TValue}"/> representing the built switch structure. This class serves as a convenient entry point for creating switch-like structures in applications and systems.
/// </summary>
public static class SwitchBuilder
{
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
        var builder = new DefaultSwitchBuilder<TKey, TValue>();

        builderFactory(builder);

        return builder.Build();
    }
}
