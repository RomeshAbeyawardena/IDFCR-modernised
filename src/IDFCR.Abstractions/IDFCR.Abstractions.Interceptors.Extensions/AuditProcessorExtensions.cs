using FastMember;
using IDFCR.Abstractions.Interceptors.Processors;
using IDFCR.Abstractions.Metadata.Attributes;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Text;

namespace IDFCR.Abstractions.Interceptors.Extensions;

/// <summary>
/// Defines a set of extension methods for the <see cref="IAuditProcessor"/> interface, providing additional functionality to enhance the auditing capabilities. These extension methods allow for the generation of change descriptions by comparing old and new entities, making it easier to track changes in data. The methods utilize the FastMember library to access properties of the entities efficiently, and they also consider display names for properties when generating change descriptions. This class is intended to be used in conjunction with implementations of the <see cref="IAuditProcessor"/> interface to provide a standardized way to generate audit logs based on entity changes.
/// </summary>
public static class AuditProcessorExtensions
{
    private static readonly Lazy<ConcurrentDictionary<Type, string[]>> _mappableMemberCache = new([]);

    private static bool IsApplicableMember(Member member)
    {
        return member.CanRead && member.GetAttribute(typeof(IgnoreAuditingAttribute), true) is null;
    }

    /// <summary>
    /// Audits the changes between two entities by comparing their properties and generating a descriptive string of the differences. This method takes in the original entity and the updated entity, and it uses reflection to compare their properties. If a property has changed, it generates a description of the change, optionally using a provided formatting function to customize the output. The method also considers display names for properties when generating change descriptions, making the output more user-friendly. This extension method is intended to be used with implementations of the <see cref="IAuditProcessor"/> interface to facilitate the creation of audit logs based on entity changes.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities being compared.</typeparam>
    /// <param name="auditProcessor">The audit processor instance.</param>
    /// <param name="oldEntity">The original entity.</param>
    /// <param name="newEntity">The updated entity.</param>
    /// <param name="cancellationToken"></param>
    /// <param name="formatLineAction">An optional function to format the change description for each property.</param>
    /// <param name="deferredLookupAsyncAction">An optional function to perform asynchronous lookups for each property change.</param>
    /// <returns>A string describing the changes between the two entities.</returns>
#pragma warning disable IDE0060
    public static async Task<string> AuditChangeDescriptionAsync<TEntity>(
        this IAuditProcessor auditProcessor, TEntity oldEntity, TEntity newEntity, 
        CancellationToken cancellationToken,
        Func<string, object, object, string>? formatLineAction = null,
        Func<string, object, CancellationToken, Task<object?>>? deferredLookupAsyncAction = null)
    {
        formatLineAction ??= (fieldName, oldValue, newValue) => $"{fieldName} changed from '{oldValue}' to '{newValue}'.";

        var changeDescriptions = new StringBuilder();

        if (oldEntity is null || newEntity is null)
        {
            return changeDescriptions.ToString();
        }

        var type = oldEntity.GetType();
        var sourceAccessor = TypeAccessor.Create(type);
        
        var newType = oldEntity.GetType();
        var targetAccessor = TypeAccessor.Create(newType);

        var sourceTypeNames = _mappableMemberCache.Value.GetOrAdd(type, (t) => [.. sourceAccessor.GetMembers().Where(IsApplicableMember).Select(m => m.Name)]);

        var newTypeNames = _mappableMemberCache.Value.GetOrAdd(newType, (t) => [.. targetAccessor.GetMembers().Where(IsApplicableMember).Select(m => m.Name)]);

        var members = sourceAccessor.GetMembers();
        var commonNames = sourceTypeNames.Intersect(newTypeNames);
        foreach (var name in commonNames)
        {
            var oldValue = sourceAccessor[oldEntity, name];
            var newValue = targetAccessor[newEntity, name];
            
            if(Equals(oldValue, newValue))
            {
                continue;
            }

            var member = members.FirstOrDefault(x => x.Name == name);

            var attribute = member?.GetAttribute(typeof(DeferredLookupAttribute), true);
            if (attribute is not null && attribute is DeferredLookupAttribute deferredLookupAttribute
                && deferredLookupAsyncAction is not null)
            {
                oldValue = await deferredLookupAsyncAction(deferredLookupAttribute.LookupKey, oldValue, cancellationToken) ?? (deferredLookupAttribute.ExposeLookupIdsInAudits ? oldValue : "[Not found]");
                newValue = await deferredLookupAsyncAction(deferredLookupAttribute.LookupKey, newValue, cancellationToken) ?? (deferredLookupAttribute.ExposeLookupIdsInAudits ? newValue : "[Not found]");
            }

            attribute = member?.GetAttribute(typeof(DisplayNameAttribute), true);

            var fieldName = name;
            if (attribute is not null && attribute is DisplayNameAttribute displayNameAttribute)
            {
                fieldName = string.IsNullOrWhiteSpace(displayNameAttribute.DisplayName) ? fieldName : displayNameAttribute.DisplayName;
            }

            changeDescriptions.AppendLine(formatLineAction(fieldName, oldValue, newValue));
        }

        return changeDescriptions.ToString();
    }
#pragma warning restore
}
