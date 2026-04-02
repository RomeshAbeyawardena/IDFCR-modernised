namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Defines a static class that provides extension methods for the Type class in .NET. The HasInterface extension method allows developers to check if a given type implements a specific interface, while also providing the option to exclude certain types from the check. By using this extension method, developers can easily determine if a type implements a desired interface, while excluding any types that may not be relevant or appropriate for the check. This can help improve the organization and maintainability of code that involves type checking and interface implementation within applications and systems that utilize the Type class in .NET.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Determines whether the specified type implements a given interface, excluding specified types.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to check for.</typeparam>
    /// <param name="type">The type to check.</param>
    /// <param name="excludedTypes">An array of types to exclude from the check.</param>
    /// <returns>True if the type implements the interface and is not excluded; otherwise, false.</returns>
   public static bool HasInterface<TInterface>(this Type type, params Type[] excludedTypes)
    {
        return type.GetInterface(typeof(TInterface).Name) is not null
            && excludedTypes.All(x => x != type);
    }
}
