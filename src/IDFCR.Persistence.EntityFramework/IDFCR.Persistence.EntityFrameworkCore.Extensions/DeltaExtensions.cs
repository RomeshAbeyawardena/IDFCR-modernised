using IDFCR.Abstractions.Metadata;
using Microsoft.EntityFrameworkCore;
using MSEntityState = Microsoft.EntityFrameworkCore.EntityState;
namespace IDFCR.Persistence.EntityFrameworkCore.Extensions;

/// <summary>
/// Defines extension methods for performing delta operations on entity relationships in an Entity Framework Core context.
/// </summary>
public static class DeltaExtensions
{
    /// <summary>
    /// Performs a delta operation on a many-to-many relationship between a parent entity and child entities, based on the provided additions and removals.
    /// </summary>
    /// <typeparam name="TEntity">The type of the child entity.</typeparam>
    /// <typeparam name="TJoinEntity">The type of the join entity.</typeparam>
    /// <typeparam name="TParentKey">The type of the parent entity's key.</typeparam>
    /// <typeparam name="TChildKey">The type of the child entity's key.</typeparam>
    /// <param name="delta">The delta containing additions and removals.</param>
    /// <param name="context">The Entity Framework Core context.</param>
    /// <param name="parentId">The key of the parent entity.</param>
    /// <param name="options">The options for configuring the delta operation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the delta operation.</returns>
    public static async Task<RelationshipDeltaResult> PerformDeltaAsync<TEntity, TJoinEntity, TParentKey, TChildKey>(
     this StringListDelta delta,
     DbContext context,
     TParentKey parentId,
     NamedDeltaOptions<TEntity, TJoinEntity, TParentKey, TChildKey> options,
     CancellationToken cancellationToken = default)
     where TEntity : class
     where TJoinEntity : class
     where TParentKey : notnull
     where TChildKey : notnull
    {
        int entitiesCreated = 0;
        int relationshipsAdded = 0;
        int relationshipsRemoved = 0;

        // Local helper to ensure consistent normalisation everywhere
        string GetNormalisedEntityName(TEntity entity) => options.NormalizeName(options.GetEntityName(entity));

        // 1. Establish deterministic Add-Wins semantics with safe post-normalisation filtering
        var additions = delta.Add
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(options.NormalizeName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(options.NameComparer);

        var removals = delta.Remove
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(options.NormalizeName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(options.NameComparer);

        removals.ExceptWith(additions);

        if (additions.Count == 0 && removals.Count == 0)
        {
            return new RelationshipDeltaResult(0, 0, 0);
        }

        var entitySet = context.Set<TEntity>();
        var entitiesByName = new Dictionary<string, TEntity>(options.NameComparer);

        // 2. Merge locally tracked entities (normalising their names)
        var localEntities = entitySet.Local.Where(e => context.Entry(e).State != MSEntityState.Deleted);
        foreach (var local in localEntities)
        {
            var name = GetNormalisedEntityName(local);
            if (additions.Contains(name) || removals.Contains(name))
            {
                entitiesByName.TryAdd(name, local);
            }
        }

        // 3. Query the database for remaining requested entities (normalising their names)
        var missingNames = additions.Concat(removals)
            .Except(entitiesByName.Keys, options.NameComparer)
            .ToArray();

        if (missingNames.Length > 0)
        {
            var persistedEntities = await options.FilterEntitiesByNames(entitySet, missingNames)
                .ToListAsync(cancellationToken);

            foreach (var entity in persistedEntities)
            {
                entitiesByName.TryAdd(GetNormalisedEntityName(entity), entity);
            }
        }

        // 4. Create genuinely missing entities
        foreach (var name in additions)
        {
            if (!entitiesByName.ContainsKey(name))
            {
                var newEntity = options.CreateNewEntity(name);
                entitySet.Add(newEntity);
                entitiesByName.Add(name, newEntity);
                entitiesCreated++;
            }
        }

        // 5. Gather existing joins securely
        var requestedEntities = additions.Concat(removals)
            .Where(entitiesByName.ContainsKey) // Safeguard against non-existent removals
            .Select(name => entitiesByName[name])
            .Distinct()
            .ToArray();

        var persistedChildIds = requestedEntities
            .Where(entity => context.Entry(entity).State != MSEntityState.Added)
            .Select(options.GetEntityId)
            .Distinct()
            .ToArray();

        var joinSet = context.Set<TJoinEntity>();

        var dbJoins = persistedChildIds.Length > 0
            ? await options.FilterExistingJoins(joinSet, parentId, persistedChildIds).ToListAsync(cancellationToken)
            : [];

        var localJoins = joinSet.Local
            .Where(join => context.Entry(join).State != MSEntityState.Deleted)
            .ToList();

        // 6. Add missing relationships
        foreach (var name in additions)
        {
            var entity = entitiesByName[name];
            var isNewEntity = context.Entry(entity).State == MSEntityState.Added;

            var existsInDatabase = !isNewEntity && dbJoins.Any(join =>
                EqualityComparer<TChildKey>.Default.Equals(options.GetJoinChildId(join), options.GetEntityId(entity)));

            var existsLocally = localJoins.Any(join => options.IsLocalJoin(join, parentId, entity));

            if (existsInDatabase || existsLocally)
            {
                continue;
            }

            var newJoin = options.CreateNewJoin(parentId, entity);
            joinSet.Add(newJoin);
            localJoins.Add(newJoin);
            relationshipsAdded++;
        }

        // 7. Remove obsolete relationships
        var entitiesToRemove = removals
            .Where(entitiesByName.ContainsKey)
            .Select(name => entitiesByName[name])
            .Distinct()
            .ToArray();

        var joinsToRemove = new List<TJoinEntity>();

        foreach (var entity in entitiesToRemove)
        {
            var isNewEntity = context.Entry(entity).State == MSEntityState.Added;

            // DB Joins: Match by stable keys
            if (!isNewEntity)
            {
                var childId = options.GetEntityId(entity);
                joinsToRemove.AddRange(dbJoins.Where(join =>
                    EqualityComparer<TChildKey>.Default.Equals(options.GetJoinChildId(join), childId)));
            }

            // Local Joins: Match by reference
            joinsToRemove.AddRange(localJoins.Where(join => options.IsLocalJoin(join, parentId, entity)));
        }

        var distinctJoinsToRemove = joinsToRemove.Distinct().ToList();

        if (distinctJoinsToRemove.Count > 0)
        {
            joinSet.RemoveRange(distinctJoinsToRemove);
            relationshipsRemoved += distinctJoinsToRemove.Count;
        }

        return new RelationshipDeltaResult(entitiesCreated, relationshipsAdded, relationshipsRemoved);
    }
}
