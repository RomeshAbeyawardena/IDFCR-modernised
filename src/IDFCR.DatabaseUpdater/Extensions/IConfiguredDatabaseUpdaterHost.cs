namespace IDFCR.DatabaseUpdater.Extensions;

/// <summary>
///Represents a host that has been configured for database updating operations. This interface extends both IDisposable and IAsyncDisposable, allowing for proper resource management when the host is no longer needed. The CommandResult property provides access to the result of the last executed command, which can be useful for determining the outcome of database update operations. By implementing this interface, developers can ensure that their database updater host is properly disposed of and can access command results in a consistent manner.
/// </summary>
public interface IConfiguredDatabaseUpdaterHost : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the result of the last executed command. This property returns an integer value that represents the outcome of the command execution, which can be used to determine whether the operation was successful or if any errors occurred. The value is nullable, allowing for cases where no command has been executed or when the result is not applicable. By accessing this property, developers can programmatically check the status of database update operations and take appropriate actions based on the command results.
    /// </summary>
    int? CommandResult { get; }
}
