namespace IDFCR.Abstractions.Cli.Operations;

/// <summary>
/// Represents an injectable command operation that extends the ICommandOperation interface. This interface is designed to support dependency injection and provide additional properties for managing cached operations, accessing services, and identifying the member type and qualified name of the operation. By implementing this interface, command operations can leverage dependency injection to access necessary services and manage their state more effectively in a CLI application. The CachedOperations property allows for storing and retrieving previously executed operations, while the Services property provides access to the service provider for resolving dependencies. The MemberOfType and QualifiedName properties can be used to identify the context and unique identifier of the operation within the application. This design promotes modularity, testability, and maintainability in a CLI application by enabling flexible command operations that can be easily extended and integrated with other components through dependency injection.
/// </summary>
public interface IInjectableCommandOperation : ICommandOperation
{
    /// <summary>
    /// Gets or sets a collection of cached operations that can be stored and retrieved for later use. The CachedOperations property allows for managing previously executed operations, enabling features such as command history, undo/redo functionality, or caching results for performance optimization in a CLI application. By setting this property, you can provide a collection of IInjectableCommandOperation instances that represent the cached operations, which can be accessed and utilized as needed during the execution of commands. This promotes a more dynamic and responsive user experience by allowing users to interact with their command history or access frequently used operations without having to re-enter commands manually.
    /// </summary>
    IEnumerable<IInjectableCommandOperation> CachedOperations { set; }
    /// <summary>
    /// Gets the service provider for resolving dependencies within the command operation. The Services property provides access to the IServiceProvider instance that can be used to resolve services and dependencies required by the command operation. This allows for leveraging dependency injection to manage the lifecycle and dependencies of the operation, promoting modularity and testability in a CLI application. By accessing the service provider, you can retrieve necessary services, such as logging, configuration, or other application-specific services, to enhance the functionality and behavior of the command operation. This design enables a more flexible and maintainable architecture by decoupling the command operation from its dependencies and allowing for easier integration with other components in the application.
    /// </summary>
    IServiceProvider Services { get; }
    /// <summary>
    /// Gets the member type that the command operation is associated with. The MemberOfType property provides information about the type that the command operation belongs to, which can be useful for identifying the context and scope of the operation within the application. This property can be used to determine the class or component that the command operation is a part of, allowing for better organization and management of operations in a CLI application. By accessing the member type, you can also leverage reflection or other techniques to dynamically discover and invoke operations based on their associated types, enhancing the flexibility and extensibility of the command handling system in the application.
    /// </summary>
    Type? MemberOfType { get; }
    /// <summary>
    /// Gets the qualified name of the command operation, which serves as a unique identifier for the operation within the application. The QualifiedName property provides a string representation of the operation's identity, which can be used for various purposes such as logging, debugging, or routing commands to the appropriate operations based on their qualified names. This property can help distinguish between different operations that may have similar functionality but belong to different contexts or components in a CLI application. By utilizing the qualified name, you can enhance the clarity and maintainability of your command handling system by providing a clear and consistent way to identify and reference command operations throughout the application.
    /// </summary>
    string QualifiedName { get; }
}
