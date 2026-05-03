namespace IDFCR.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents the behavior to be applied when performing an upsert operation in the repository. This enumeration defines the different strategies for handling upsert operations, such as whether to only allow forward-only operations (inserts) or to allow both inserts and updates. The RepositoryUpsertBehaviour enum can be used to configure the behavior of the repository when performing upsert operations, providing flexibility in how data is managed and updated within the database context while adhering to the repository pattern and leveraging Entity Framework Core's capabilities for data management. By specifying the desired upsert behavior, developers can control how the repository handles scenarios where an entity may already exist in the database, ensuring that data integrity and consistency are maintained according to the application's requirements.
/// </summary>
public enum RepositoryUpsertBehaviour
{
    /// <summary>
    /// Inserts or updates an entity in the database based on whether it already exists. If the entity does not exist, it will be inserted as a new record. If the entity already exists, it will be updated with the new values provided. This behavior is useful when you want to ensure that the database reflects the most current state of the data, allowing for both the addition of new records and the modification of existing records as necessary. By using the InsertOrUpdate upsert behavior, you can maintain data integrity while ensuring that your database remains up-to-date with the latest information.
    /// </summary>
    InsertOrUpdate = 0,
    /// <summary>
    /// Inserts a new entity into the database if it does not already exist, but does not allow updates to existing entities. This behavior is typically used when you want to ensure that only new records are added to the database, and any attempts to update existing records will be ignored or result in an error. By using the ForwardOnly upsert behavior, you can maintain data integrity by preventing unintended modifications to existing records while still allowing for the addition of new data as needed.
    /// </summary>
    ForwardOnly = 1
}
