using FastMember;
using IDFCR.Abstractions.Metadata;
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
    /// Audits the changes between two entities by comparing their properties and generating a description of the changes. This method takes two entities of the same type, compares their properties, and constructs a string that describes the differences between them. It uses the FastMember library to access the properties of the entities efficiently and considers any display names defined for the properties when generating the change descriptions. The resulting string can be used for logging or auditing purposes to track changes in data over time.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities being compared.</typeparam>
    /// <param name="auditProcessor">The audit processor instance.</param>
    /// <param name="oldEntity">The original entity.</param>
    /// <param name="newEntity">The updated entity.</param>
    /// <returns>A string describing the changes between the two entities.</returns>
#pragma warning disable IDE0060
    public static string AuditChanges<TEntity>(
        this IAuditProcessor auditProcessor, TEntity oldEntity, TEntity newEntity)
    {
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

        var newTypeNames = _mappableMemberCache.Value.GetOrAdd(type, (t) => [.. targetAccessor.GetMembers().Where(IsApplicableMember).Select(m => m.Name)]);

        var members = sourceAccessor.GetMembers();

        foreach (var name in sourceTypeNames)
        {
            var oldValue = sourceAccessor[oldEntity, name];
            var newValue = targetAccessor[newEntity, name];
            
            if(Equals(oldValue, newValue))
            {
                continue;
            }

            var attribute = members.FirstOrDefault(x => x.Name == name)?.GetAttribute(typeof(DisplayNameAttribute), true);

            var fieldName = name;
            if (attribute is not null && attribute is DisplayNameAttribute displayNameAttribute)
            {
                fieldName = string.IsNullOrWhiteSpace(displayNameAttribute.DisplayName) ? fieldName : displayNameAttribute.DisplayName;
            }

            changeDescriptions.AppendLine($"{fieldName} changed from '{oldValue}' to {newValue}.");
        }

        return changeDescriptions.ToString();
    }
#pragma warning restore
}
