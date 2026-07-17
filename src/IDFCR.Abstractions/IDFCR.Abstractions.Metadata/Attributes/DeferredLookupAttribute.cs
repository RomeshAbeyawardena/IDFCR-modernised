namespace IDFCR.Abstractions.Metadata.Attributes;

/// <summary>
/// Represents a custom attribute that can be applied to properties or fields to indicate that they should be treated as deferred lookups during auditing processes. This attribute allows developers to specify a lookup key that can be used to perform asynchronous lookups for the property or field when generating audit descriptions. By applying this attribute to a property or field, developers can enable the inclusion of additional information in audit logs by performing deferred lookups based on the specified key, enhancing the auditing capabilities and providing more comprehensive audit records for applications and systems that utilize auditing mechanisms for tracking entity modifications.
/// </summary>
/// <param name="lookupKey">The key used to perform the deferred lookup.</param>
/// <param name="exposeLookupIdsInAudits">Indicates whether the lookup IDs should be exposed in audit logs.</param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class DeferredLookupAttribute(string lookupKey, bool exposeLookupIdsInAudits = false) : Attribute
{
    /// <summary>
    /// Gets the key used to perform the deferred lookup. This property is set through the constructor of the attribute and is intended to be used when generating audit descriptions to perform asynchronous lookups for the associated property or field. By providing a lookup key, developers can enable the retrieval of additional information related to the property or field during the auditing process, allowing for more detailed and informative audit logs within applications and systems that utilize auditing mechanisms for tracking entity modifications.
    /// </summary>
    public string LookupKey { get; } = lookupKey;
    /// <summary>
    /// Gets a value indicating whether the lookup IDs should be exposed in audit logs. This property is set through the constructor of the attribute and can be used to control whether the IDs associated with the deferred lookup should be included in the generated audit descriptions. By setting this property to true, developers can choose to include the lookup IDs in audit logs, providing additional context and information about the deferred lookup during the auditing process within applications and systems that utilize auditing mechanisms for tracking entity modifications.
    /// </summary>
    public bool ExposeLookupIdsInAudits { get; } = exposeLookupIdsInAudits;
}
