using System.Diagnostics.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IDFCR.Abstractions.Interceptors;

/// <summary>
/// Represents a collection of scoped resources that can be shared and accessed within the context of entity interceptors. This interface defines the contract for managing and retrieving resources that are specific to the scope of an entity interception operation, allowing for the storage and retrieval of data or services that may be needed during the interception process. The IScopedResources interface provides methods for adding or updating resources, checking for the existence of resources, and retrieving resources based on their type, enabling developers to manage and utilize scoped resources effectively within applications and systems that utilize interception mechanisms for managing entity operations.
/// <para>
/// ⚠️ This is not a service locator. Required dependencies must be injected.
///This collection is intended only for sharing already-resolved, execution-scoped resources(e.g.DbContext instances, execution metadata, or intermediate state) within a single interceptor pipeline.
/// It must not be used to resolve services or introduce hidden dependencies.
/// </para>
/// </summary>
public interface IScopedResources
{
    /// <summary>
    /// Gets a read-only dictionary of scoped resources, where the key is the type of the resource and the value is the resource itself. This property allows for accessing the collection of scoped resources that have been added or updated within the context of entity interceptors, providing a way to retrieve resources based on their type for use during the interception process. By exposing this property, developers can easily access and utilize the scoped resources that are relevant to their interception logic within applications and systems that utilize interception mechanisms for managing entity operations.
    /// </summary>
    IReadOnlyDictionary<Type, object?> Items { get; }
    /// <summary>
    /// Determines whether a scoped resource of the specified type exists within the collection of scoped resources. This method allows for checking if a resource of a particular type has been added or updated within the context of entity interceptors, providing a way to verify the presence of resources before attempting to retrieve them. By implementing this method, developers can ensure that they are accessing valid resources during the interception process and handle cases where resources may not be available, enhancing the robustness and reliability of their interception logic within applications and systems that utilize interception mechanisms for managing entity operations.
    /// </summary>
    /// <typeparam name="T">The type of the scoped resource to check for.</typeparam>
    /// <returns>True if a scoped resource of the specified type exists; otherwise, false.</returns>
    bool Contains<T>();
    /// <summary>
    /// Gets a scoped resource of the specified type from the collection of scoped resources. This method allows for retrieving a resource of a particular type that has been added or updated within the context of entity interceptors, providing a way to access resources that may be needed during the interception process. By implementing this method, developers can easily obtain the necessary resources for their interception logic, ensuring that they have access to the relevant data or services required for managing entity operations effectively within applications and systems that utilize interception mechanisms.
    /// </summary>
    /// <typeparam name="T">The type of the scoped resource to retrieve.</typeparam>
    /// <returns>The scoped resource of the specified type, or null if it does not exist.</returns>
    T? GetScopedResource<T>();
    /// <summary>
    /// Tries to get a scoped resource of the specified type from the collection of scoped resources. This method allows for attempting to retrieve a resource of a particular type that has been added or updated within the context of entity interceptors, providing a way to access resources that may be needed during the interception process while also handling cases where the resource may not exist. By implementing this method, developers can safely attempt to obtain the necessary resources for their interception logic, ensuring that they can handle scenarios where resources may not be available and maintain the robustness and reliability of their interception logic within applications and systems that utilize interception mechanisms for managing entity operations.
    /// </summary>
    /// <typeparam name="T">The type of the scoped resource to retrieve.</typeparam>
    /// <param name="value">When this method returns, contains the scoped resource of the specified type if it exists; otherwise, the default value for the type.</param>
    /// <returns>True if a scoped resource of the specified type exists; otherwise, false.</returns>
    bool TryGetScopedResource<T>([NotNullWhen(true)]out T? value);
    /// <summary>
    /// Adds or updates a scoped resource of the specified type in the collection of scoped resources. This method allows for adding a new resource or updating an existing resource within the context of entity interceptors, providing a way to manage resources that may be needed during the interception process. By implementing this method, developers can ensure that the necessary resources are available and up-to-date for their interception logic, enhancing the flexibility and reliability of their interception mechanisms within applications and systems that utilize interception mechanisms for managing entity operations.
    /// </summary>
    /// <typeparam name="T">The type of the scoped resource to add or update.</typeparam>
    /// <param name="value">The scoped resource to add or update.</param>
    void AddOrUpdate<T>(T value);
}
