using Scrutor;

namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Defines a static class that provides extension methods for the Scrutor library, which is a popular assembly scanning and registration tool for dependency injection in .NET applications. The HasInterface extension method allows developers to filter types based on whether they implement a specific interface, while also providing the option to exclude certain types from the filter. By using this extension method, developers can easily configure their dependency injection registrations to include only types that implement a desired interface, while excluding any types that may not be relevant or appropriate for the registration process. This can help improve the organization and maintainability of dependency injection configurations within applications and systems that utilize Scrutor for assembly scanning and registration purposes.
/// </summary>
public static class ScrutorExtensions
{
    /// <summary>
    /// Prepares a filter for types that implement a specific interface, while allowing for the exclusion of certain types from the filter. This method takes a generic type parameter TInterface, which represents the interface that the types must implement to be included in the filter. The method also accepts an array of Type objects representing the types to be excluded from the filter. By using this extension method, developers can easily create filters that target specific interfaces while excluding any types that may not be relevant or appropriate for the registration process, improving the organization and maintainability of dependency injection configurations within applications and systems that utilize Scrutor for assembly scanning and registration purposes.
    /// </summary>
    /// <typeparam name="TInterface">The interface type that the filtered types must implement.</typeparam>
    /// <param name="typeFilter">The type filter to apply the interface filter to.</param>
    /// <param name="excludedTypes">An array of types to exclude from the filter.</param>
    /// <returns>The updated type filter with the interface filter applied.</returns>
    public static IImplementationTypeFilter HasInterface<TInterface>(this IImplementationTypeFilter typeFilter, params Type[] excludedTypes)
    {
        return typeFilter.Where(x => x.HasInterface<TInterface>(excludedTypes));
    }

}
