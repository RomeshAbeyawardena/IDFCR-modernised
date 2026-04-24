namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents a contract for entities that can be audited. Implementing this interface indicates that the entity has an associated name that can be used for auditing purposes, such as logging or tracking changes to the entity. The AuditEntityName property provides a way to identify the entity in audit logs or other auditing mechanisms, allowing for better traceability and accountability in applications that require auditing of entity changes or actions performed on the entity.
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// Gets the name of the DB entity used for storing the changed fields of the entity. This name is typically used in auditing scenarios to identify the specific entity being audited, allowing for better traceability and organization of audit logs. The value returned by this property should correspond to the name of the database entity that is being tracked for changes, enabling consistent and meaningful audit records for operations performed on the entity.
    /// </summary>
    string AuditEntityName { get; }
}
