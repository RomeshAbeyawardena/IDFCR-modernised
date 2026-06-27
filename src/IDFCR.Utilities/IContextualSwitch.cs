namespace IDFCR.Utilities;

/// <summary>
/// Represents a context-aware switch-like structure that maps keys to values based on defined cases and an optional default case, with additional context passed during evaluation. The <see cref="IContextualSwitch{TKey, TContext, TValue}"/> interface provides methods for evaluating keys with context and obtaining corresponding values based on the defined cases and default case. If a key matches a defined case, the associated value factory is invoked with both the key and context; if no cases match and a default case is defined, the default value factory is invoked; otherwise, null is returned. This interface allows for flexible and extensible switch-like behavior with contextual parameters in applications and systems, enabling developers to implement conditional logic that requires additional runtime information beyond the key.
/// </summary>
/// <typeparam name="TKey">The type of the keys used to identify cases in the switch structure.</typeparam>
/// <typeparam name="TContext">The type of the contextual parameter passed during evaluation. Can be a tuple, record, class, or any type containing additional parameters needed by value factories.</typeparam>
/// <typeparam name="TValue">The type of the values associated with the keys in the switch structure.</typeparam>
public interface IContextualSwitch<TKey, TContext, TValue>
    where TKey : notnull
{
    /// <summary>
    /// Evaluates the specified key and returns a value factory function that generates the corresponding value based on the defined cases and optional default case. The factory accepts both the key and a context parameter. If the key matches a defined case, the associated value factory is returned; if no cases match and a default case is defined, the default value factory is returned; otherwise, null is returned.
    /// </summary>
    /// <param name="key">The key to evaluate.</param>
    /// <returns>A value factory function that accepts a key and context and generates the corresponding value, or null if no matching case is found and no default case is defined.</returns>
    Func<TKey, TContext, TValue>? Then(TKey key);

    /// <summary>
    /// Evaluates the specified key with the provided context and returns the corresponding value based on the defined cases and optional default case. If the key matches a defined case, the associated value factory is invoked with the key and context; if no cases match and a default case is defined, the default value factory is invoked; otherwise, null is returned.
    /// </summary>
    /// <param name="key">The key to evaluate.</param>
    /// <param name="context">The contextual parameter to pass to the value factory.</param>
    /// <returns>The corresponding value, or null if no matching case is found and no default case is defined.</returns>
    TValue? ThenValue(TKey key, TContext context);
}
