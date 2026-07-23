namespace IDFCR.Abstractions.Metadata;

/// <summary>
/// Options for performing a delta operation on a named entity relationship, including normalization and entity/join operations.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TJoinEntity">The type of the join entity.</typeparam>
/// <typeparam name="TParentKey">The type of the parent key.</typeparam>
/// <typeparam name="TChildKey">The type of the child key.</typeparam>
public class NamedDeltaOptions<TEntity, TJoinEntity, TParentKey, TChildKey>
    where TEntity : class
    where TJoinEntity : class
    where TParentKey : notnull
    where TChildKey : notnull
{
    /// <summary>
    /// Gets or sets the function used to normalize entity names. By default, it trims whitespace from the names.
    /// </summary>
    public Func<string, string> NormalizeName { get; set; } = x => x.Trim();
    /// <summary>
    /// Gets or sets the string comparer used for comparing entity names. By default, it uses a case-insensitive ordinal comparison.
    /// </summary>
    public StringComparer NameComparer { get; set; } = StringComparer.OrdinalIgnoreCase;
    /// <summary>
    /// Gets or sets a value indicating whether to ignore empty or whitespace-only names during the delta operation. By default, it is set to true.
    /// </summary>
    public required Func<IQueryable<TEntity>, string[], IQueryable<TEntity>> FilterEntitiesByNames { get; set; }
    /// <summary>
    /// Gets or sets a function that retrieves the name of an entity. This function is required for identifying entities by their names.
    /// </summary>
    public required Func<TEntity, string> GetEntityName { get; set; }
    /// <summary>
    /// Gets or sets a function that retrieves the unique identifier of an entity. This function is required for identifying entities by their keys.
    /// </summary>
    public required Func<TEntity, TChildKey> GetEntityId { get; set; }
    /// <summary>
    /// Gets or sets a function that creates a new entity given its name. This function is required for creating new entities when they do not already exist.
    /// </summary>
    public required Func<string, TEntity> CreateNewEntity { get; set; }

    /// <summary>
    /// Gets or sets a function that filters existing join entities based on the parent key and an array of child keys. This function is required for identifying existing relationships between the parent entity and its child entities.
    /// </summary>
    public required Func<IQueryable<TJoinEntity>, TParentKey, TChildKey[], IQueryable<TJoinEntity>> FilterExistingJoins { get; set; }
    /// <summary>
    /// Gets or sets a function that creates a new join entity given a parent key and a child entity. This function is required for creating new relationships when they do not already exist.
    /// </summary>
    public required Func<TParentKey, TEntity, TJoinEntity> CreateNewJoin { get; set; }

    /// <summary>
    /// Gets or sets a function that retrieves the parent key from a join entity. This function is required for identifying the parent entity in a relationship.
    /// </summary>
    public required Func<TJoinEntity, TChildKey> GetJoinChildId { get; set; }

    /// <summary>
    /// Gets or sets a function that determines whether a given join entity is associated with a specific parent key and child entity. This function is required for identifying existing relationships between the parent entity and its child entities.
    /// </summary>
    public required Func<TJoinEntity, TParentKey, TEntity, bool> IsLocalJoin { get; set; }
}
