using FastMember;
using System.Collections.Concurrent;
using System.Text;

namespace IDFCR.Abstractions.Interceptors.Extensions;

public static class AuditProcessorExtensions
{
    private static readonly ConcurrentDictionary<Type, string[]> _mappableMemberCache = [];
#pragma warning disable IDE0060
    public static string ChangeDescriptor<TEntity>(
        this IAuditProcessor auditProcessor, TEntity oldEntity, TEntity newEntity)
    {
        var changeDescriptions = new StringBuilder();

        if (oldEntity is null || newEntity is null)
        {
            return changeDescriptions.ToString();
        }

        var type = oldEntity.GetType();
        var sourceAccessor = TypeAccessor.Create(type);

        var sourceTypeNames = _mappableMemberCache.GetOrAdd(type, (t) => [.. sourceAccessor.GetMembers().Where(m => m.CanRead).Select(m => m.Name)]);

        foreach(var name in sourceTypeNames)
        {
            var oldValue = sourceAccessor[oldEntity, name];
            var newValue = sourceAccessor[newEntity, name];

            if(object.Equals(oldValue, newValue))
            {
                continue;
            }

            changeDescriptions.AppendLine($"{name} changed from '{oldValue}' to {newValue}.");
        }

        return changeDescriptions.ToString();
    }
#pragma warning restore
}
