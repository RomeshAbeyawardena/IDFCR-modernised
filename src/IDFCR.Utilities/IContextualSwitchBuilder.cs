namespace IDFCR.Utilities;

/// <summary>
/// Represents a builder for creating a context-aware switch-like structure that maps keys to values based on specified cases and an optional default case, with additional context passed during evaluation. The <see cref="IContextualSwitchBuilder{TKey, TContext, TValue}"/> interface provides methods for defining cases and an optional default case, allowing developers to build a switch-like structure that requires contextual parameters for value resolution. The resulting <see cref="IContextualSwitch{TKey, TContext, TValue}"/> instance can be used to evaluate keys with context and obtain corresponding values based on the defined cases and default case, providing a flexible and extensible way to implement context-aware switch-like behavior in applications and systems.
/// </summary>
/// <typeparam name="TKey">The type of the keys used to identify cases.</typeparam>
/// <typeparam name="TContext">The type of the contextual parameter passed during evaluation. Can be a tuple, record, class, or any type containing additional parameters needed by value factories.</typeparam>
/// <typeparam name="TValue">The type of the values associated with each case.</typeparam>
public interface IContextualSwitchBuilder<TKey, TContext, TValue>
    where TKey : notnull
{
    /// <summary>
    /// Defines a case for the switch-like structure, associating a specific key with a value factory function that generates the corresponding value when the key is evaluated with context. The factory receives both the key and context as parameters. If the specified key already exists in the switch structure, the existing value factory will be replaced with the new one.
    /// </summary>
    /// <param name="key">The key that identifies the case.</param>
    /// <param name="valueFactory">A function that generates the value associated with the specified key, accepting both the key and context as parameters.</param>
    /// <returns>The current instance of the switch builder, allowing for method chaining.</returns>
    IContextualSwitchBuilder<TKey, TContext, TValue> CaseWhen(TKey key, Func<TKey, TContext, TValue> valueFactory);

    /// <summary>
    /// Defines a case for the switch-like structure, associating a specific key with a fixed value that ignores the context parameter. If the specified key already exists in the switch structure, the existing value will be replaced with the new one.
    /// </summary>
    /// <param name="key">The key that identifies the case.</param>
    /// <param name="value">The value associated with the specified key.</param>
    /// <returns>The current instance of the switch builder, allowing for method chaining.</returns>
    IContextualSwitchBuilder<TKey, TContext, TValue> CaseWhen(TKey key, TValue value);

    /// <summary>
    /// Defines a default case for the switch-like structure, specifying a value factory function that generates the corresponding value when no defined cases match the evaluated key. The factory receives both the key and context as parameters. If a default case has already been defined, it will be replaced with the new one.
    /// </summary>
    /// <param name="valueFactory">A function that generates the value for the default case, accepting both the key and context as parameters.</param>
    /// <returns>The current instance of the switch builder, allowing for method chaining.</returns>
    IContextualSwitchBuilder<TKey, TContext, TValue> Else(Func<TKey, TContext, TValue> valueFactory);

    /// <summary>
    /// Defines a default case for the switch-like structure, specifying a fixed value to be used when no defined cases match the evaluated key. The value ignores the context parameter. If a default case has already been defined, it will be replaced with the new one.
    /// </summary>
    /// <param name="value">The value for the default case.</param>
    /// <returns>The current instance of the switch builder, allowing for method chaining.</returns>
    IContextualSwitchBuilder<TKey, TContext, TValue> Else(TValue value);

    /// <summary>
    /// Builds the context-aware switch-like structure based on the defined cases and optional default case, returning an instance of <see cref="IContextualSwitch{TKey, TContext, TValue}"/> that can be used to evaluate keys with context and obtain corresponding values. Once built, the switch structure is immutable and cannot be modified.
    /// </summary>
    /// <returns>An instance of <see cref="IContextualSwitch{TKey, TContext, TValue}"/> representing the built switch structure.</returns>
    IContextualSwitch<TKey, TContext, TValue> Build();
}
