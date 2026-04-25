namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Represents an attribute that can be applied to a class to indicate that it should be ignored during auditing processes. When a class is decorated with the <see cref="IgnoreAuditingAttribute"/>, any properties or changes associated with that class will be excluded from audit logs and change tracking. This attribute is useful for scenarios where certain classes or entities should not be audited, such as those that contain sensitive information or are not relevant to auditing requirements. By applying this attribute, developers can ensure that specific classes are omitted from auditing operations, providing greater control over what data is tracked and logged in audit trails.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class IgnoreAuditingAttribute : Attribute
{
	
}
