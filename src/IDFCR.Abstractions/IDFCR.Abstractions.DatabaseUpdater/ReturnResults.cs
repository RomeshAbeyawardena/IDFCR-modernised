namespace IDFCR.Abstractions.DatabaseUpdater;

/// <summary>
/// Represents a static class that defines constant integer values for return results in the context of database updating operations. These constants can be used to indicate the outcome of various operations, such as whether an action is required, if work is pending, if an action is pending, if work has been completed, or if an error has occurred. By using these predefined constants, developers can standardize the way return results are represented and handled throughout the database updating process, making it easier to interpret and manage the outcomes of different operations within the application.
/// </summary>
public static class ReturnResults
{
    /// <summary>
    /// Represents a return result indicating that no action is required. This constant can be used to signify that an operation has completed successfully and that there are no further actions needed. It is represented by the integer value 0, which can be used in various contexts to indicate a successful outcome without any pending work or actions.
    /// </summary>
    public const int Success_NoActionRequired = 0;
    /// <summary>
    /// Represents a return result indicating that work is pending. This constant can be used to signify that an operation has started but is not yet complete, and that further actions are required to complete the work. It is represented by the integer value 1.
    /// </summary>
    public const int Success_WorkPending = 1;
    /// <summary>
    /// Represents a return result indicating that an action is pending. This constant can be used to signify that an operation has completed successfully, but there are still actions that need to be taken before the work is fully complete. It is represented by the integer value 2, which can be used to indicate that while the main operation was successful, there are additional steps or follow-up actions that need to be addressed. This allows for a more nuanced representation of the outcome of an operation, distinguishing between work that is still in progress and work that has been completed but requires further attention.
    /// </summary>
    public const int Success_ActionPending = 2;
    /// <summary>
    /// Represents a return result indicating that work has been completed successfully. This constant can be used to signify that an operation has finished all required tasks without any pending actions. It is represented by the integer value 3.
    /// </summary>
    public const int Success_WorkCompleted = 3;
    /// <summary>
    /// Represents a return result indicating that an error has occurred. This constant can be used to signify that an operation has encountered an issue or failure during its execution. It is represented by the integer value -1, which can be used to indicate that the operation did not complete successfully and that there may be a need for error handling or troubleshooting to address the underlying issue. By using this constant, developers can standardize the way errors are represented and handled throughout the application, making it easier to identify and manage error conditions in a consistent manner.
    /// </summary>
    public const int Error = -1;
}
