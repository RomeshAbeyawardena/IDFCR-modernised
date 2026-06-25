
using IDFCR.Abstractions.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IDFCR.Persistence.EntityFrameworkCore.Extensions;

/// <summary>
/// Defines extension methods for configuring entity types in Entity Framework Core, specifically for entities that implement the IJsonAudit interface. These extension methods provide a convenient way to configure audit-related properties, such as ChangeDescription, OldValueJson, and NewValueJson, with appropriate maximum lengths. By using these extension methods, developers can ensure consistent configuration of audit fields across different entity types that require auditing capabilities within their applications.
/// </summary>
public static class EntityTypeConfigurationExtensions
{
    /// <summary>
    /// Configures the properties of an entity type that implements the IJsonAudit interface, setting maximum lengths for the ChangeDescription, OldValueJson, and NewValueJson properties. This method is intended to be used within the OnModelCreating method of a DbContext to ensure consistent configuration of audit-related fields across different entity types that require auditing capabilities. By calling this extension method, developers can easily apply the necessary configurations for JSON-based audit entities, facilitating the tracking and analysis of changes to entities in a structured JSON format.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being configured.</typeparam>
    /// <param name="builder">The EntityTypeBuilder used to configure the entity.</param>
    public static void ConfigureJsonAuditSqlFields<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class, IJsonAudit
    {
        builder.Property(x => x.ChangeDescription)
            .HasMaxLength(4000);
        builder.Property(x => x.NewValueJson)
            .HasMaxLength(4000);
        builder.Property(x => x.OldValueJson)
            .HasMaxLength(4000);
    }
}
