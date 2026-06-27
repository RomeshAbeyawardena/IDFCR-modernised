namespace IDFCR.Utilities;

/// <summary>
/// Represents a switch-like structure that maps keys to values based on defined cases and an optional default case. The <see cref="ISwitch{TKey, TValue}"/> interface provides methods for evaluating keys and obtaining corresponding values based on the defined cases and default case. If a key matches a defined case, the associated value is returned; if no cases match and a default case is defined, the default value is returned; otherwise, null is returned. This interface allows for flexible and extensible switch-like behavior in applications and systems, enabling developers to implement conditional logic based on key-value mappings.
/// </summary>
/// <typeparam name="TKey">The type of the keys used to identify cases in the switch structure.</typeparam>
/// <typeparam name="TValue">The type of the values associated with the keys in the switch structure.</typeparam>
public interface ISwitch<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// Evaluates the specified key and returns a value factory function that generates the corresponding value based on the defined cases and optional default case. If the key matches a defined case, the associated value factory is returned; if no cases match and a default case is defined, the default value factory is returned; otherwise, null is returned.
    /// </summary>
    /// <param name="key">The key to evaluate.</param>
    /// <returns>A value factory function that generates the corresponding value, or null if no matching case is found and no default case is defined.</returns>
    Func<TKey, TValue>? Then(TKey key);
    /// <summary>
    /// Evaluates the specified key and returns the corresponding value based on the defined cases and optional default case. If the key matches a defined case, the associated value is returned; if no cases match and a default case is defined, the default value is returned; otherwise, null is returned.
    /// </summary>
    /// <param name="key">The key to evaluate.</param>
    /// <returns>The corresponding value, or null if no matching case is found and no default case is defined.</returns>
    TValue? ThenValue(TKey key);
}
