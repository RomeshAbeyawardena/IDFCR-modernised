namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents an interface for JSON-based audit entities, allowing for the tracking of changes to entities in a JSON format. This interface extends the IAuditCreatedTimestamp interface, which provides a property for tracking the timestamp of when the audit entry was created. The IJsonAudit interface includes properties for describing the change (ChangeDescription), as well as properties for storing the old and new values of the entity in JSON format (OldValueJson and NewValueJson). By implementing this interface, developers can create audit entities that capture changes to entities in a structured JSON format, making it easier to track and analyze changes over time within applications that require auditing capabilities.
/// </summary>
public interface IJsonAudit : IAuditCreatedTimestamp
{
    /// <summary>
    /// Gets or sets a description of the change that occurred to the entity being audited. This property provides a human-readable description of the changes made to the entity, allowing for easier understanding and analysis of the changes when reviewing audit logs. The ChangeDescription property can be used to summarize the modifications made to the entity, providing context and details about the changes that were captured in the audit entry. By including a change description, developers can enhance the audit logs with meaningful information about the changes that occurred, making it easier to track and analyze changes over time within applications that require auditing capabilities.
    /// </summary>
    string? ChangeDescription { get; set; }
    /// <summary>
    /// Gets or sets the old value of the entity being audited in JSON format. This property allows for the storage of the previous state of the entity before the changes were made, providing a snapshot of the entity's state prior to modification. The OldValueJson property can be used to capture the original values of the entity's properties in a structured JSON format, making it easier to track and analyze changes over time when reviewing audit logs. By storing the old value in JSON format, developers can facilitate the comparison of changes and provide a clear representation of the entity's state before modifications were applied.
    /// </summary>
    string? OldValueJson { get; set; }
    /// <summary>
    /// Gets or sets the new value of the entity being audited in JSON format. This property allows for the storage of the updated state of the entity after the changes were made, providing a snapshot of the entity's state following modification. The NewValueJson property can be used to capture the new values of the entity's properties in a structured JSON format, making it easier to track and analyze changes over time when reviewing audit logs. By storing the new value in JSON format, developers can facilitate the comparison of changes and provide a clear representation of the entity's state after modifications were applied.
    /// </summary>
    string? NewValueJson { get; set; }
}
