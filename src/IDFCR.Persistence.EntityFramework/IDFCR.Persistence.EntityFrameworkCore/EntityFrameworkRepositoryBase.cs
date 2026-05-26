using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Interceptors.Factories;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Metadata.Extensions;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;
using IDFCR.Persistence.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace IDFCR.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a base repository implementation for Entity Framework Core, providing common data access functionalities for entities that implement the specified interfaces. This class is designed
/// </summary>
/// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
/// <typeparam name="TCommon">The type of the common entity.</typeparam>
/// <typeparam name="TDb">The type of the database entity.</typeparam>
/// <typeparam name="T">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
/// <param name="db">The DbContext instance.</param>
/// <param name="filterFactory">The filter factory instance.</param>
/// <param name="entityInterceptorFactory">The entity interceptor factory instance.</param>
/// <param name="scopedResources">The scoped resources instance.</param>
/// <param name="upsertBehaviour">The upsert behavior for the repository.</param>
public abstract class EntityFrameworkRepositoryBase<TDbContext, TCommon, TDb, T, TKey>(TDbContext db, 
        IFilterFactory filterFactory, 
        IEntityInterceptorFactory entityInterceptorFactory, 
        IScopedResources scopedResources, RepositoryUpsertBehaviour upsertBehaviour = default)
    : RepositoryBase<TCommon, TDb, T, TKey>(entityInterceptorFactory), ITransactionalUnitOfWork
    where TDbContext : DbContext
    where TKey : struct
    where TDb : class, IMapper<TCommon>, TCommon, IIdentifiable<TKey>
    where T : class, IMapper<TCommon>, TCommon
{
    /// <summary>
    /// Gets a boolean value indicating whether the repository should default to using no tracking for queries. This property is set to true by default, meaning that when retrieving entities from the database, the repository will not track changes to those entities in the DbContext. This can improve performance for read-only scenarios where tracking is not necessary. However, derived classes can override this property to change the default behavior if they require tracking for certain operations. By providing this property, the repository base class allows for flexibility in how entities are managed and tracked within the context of Entity Framework Core while adhering to the repository pattern.
    /// <para>⚠️ Unless you know what you're doing it is advisable to leave this field alone as it can impact the behavior of the repository and the performance of database operations.</para>
    /// </summary>
    public bool DefaultsToNoTracking { get; protected set; } = true;
    /// <summary>
    /// Gets the last tracked entity entry for the database entity type. This property provides access to the EntityEntry object that represents the last tracked entity of the specified database entity type. The EntityEntry contains information about the state of the entity, such as whether it is added, modified, or deleted, and allows for tracking changes to the entity during the lifecycle of the DbContext. By exposing this property, the repository base class enables derived classes to access and utilize the tracking information for entities when performing operations such as updates or deletes, while adhering to the repository pattern and leveraging Entity Framework Core's capabilities for data management.
    /// </summary>
    protected EntityEntry<TDb>? LastTrackedEntry { get; private set; }
    /// <summary>
    /// Gets the DbContext instance used for database operations. This property provides access to the underlying DbContext, allowing derived classes to perform various database operations such as querying, adding, updating, and deleting entities. By exposing this property, the repository base class enables derived classes to leverage the full capabilities of Entity Framework Core for data management while adhering to the repository pattern. This allows for efficient and flexible data access while maintaining a clean separation of concerns between the repository and the database context.
    /// </summary>
    protected TDbContext Db => db;

    /// <summary>
    /// Gets the filter factory instance used for applying filters to queries. This property provides access to the filter factory, allowing derived classes to utilize it for filtering data when retrieving entities from the database. The filter factory is typically used in conjunction with the repository's querying methods to apply specific filtering criteria based on the requirements of the application. By exposing this property, the repository base class enables derived classes to easily access and utilize the filter factory for implementing custom filtering logic as needed.
    /// </summary>
    protected IFilterFactory FilterFactory { get; } = filterFactory;
    /// <summary>
    /// Gets the DbSet for the specified database entity type with no tracking. This property provides access to the DbSet of the specified database entity type, allowing derived classes to perform read-only operations without tracking changes to the entities. By using AsNoTracking, this property ensures that the entities retrieved from the database are not tracked by the DbContext, which can improve performance for read-only scenarios. Derived classes can use this property when they do not need to modify the entities and want to optimize query performance while adhering to the repository pattern and leveraging Entity Framework Core's capabilities for data management.
    /// </summary>
    protected IQueryable<TDb> DbSet => DefaultsToNoTracking ? RawDbSet.AsNoTracking() : RawDbSet;

    /// <summary>
    /// Gets the raw DbSet for the specified database entity type. This property provides direct access to the DbSet without applying AsNoTracking, allowing derived classes to perform operations that require tracking changes to entities, such as updates or deletes. By exposing this property, the repository base class enables derived classes to have more control over the tracking behavior of entities when necessary, while still providing the option to use the DbSet property for read-only operations that do not require tracking. This design allows for flexibility in how entities are managed and manipulated within the repository while adhering to the repository pattern and leveraging Entity Framework Core's capabilities for data management.
    /// </summary>
    protected DbSet<TDb> RawDbSet { get; } = db.Set<TDb>();
    /// <summary>
    /// Defines an asynchronous method for adding a new entity to the database. This method takes an instance of the database entity, the raw entity, and a cancellation token as parameters. It adds the entity to the DbSet and returns the key of the newly added entity. The method is designed to be overridden by derived classes to provide specific implementation details for adding entities to the database. By utilizing this method, developers can ensure that new entities are properly added to the database while adhering to the repository pattern and leveraging Entity Framework Core's capabilities for data management.
    /// </summary>
    /// <param name="entry">The database entity to be added.</param>
    /// <param name="rawEntry">The raw entity to be added.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The key of the newly added entity.</returns>
    protected override async Task<TKey> OnAddAsync(TDb entry, T rawEntry, CancellationToken cancellationToken)
    {
        if (entry is IIdentifiable<Guid> guidEntry && guidEntry.Id == Guid.Empty)
        {
            // Generate the sequential GUID before the tracker gets involved
            guidEntry.Id = new SequentialGuidValueGenerator().Next(null!);
        }

        LastTrackedEntry = await RawDbSet.AddAsync(entry, cancellationToken);

        // Now CurrentValue is guaranteed to be the sequential GUID you just set
        return LastTrackedEntry.Property(x => x.Id).CurrentValue;
    }

    /// <summary>
    /// Defines an asynchronous method for deleting an entity from the database based on its key. This method takes the key of the entity to be deleted and a cancellation token as parameters. It attempts to find the entity in the DbSet using the provided key, and if found, it removes the entity from the DbSet. The method returns a boolean value indicating whether the deletion was successful (i.e., whether the entity was found and removed). This method is designed to be overridden by derived classes to provide specific implementation details for deleting entities from the database while adhering to the repository pattern and leveraging Entity Framework Core's capabilities for data management.
    /// </summary>
    /// <param name="key">The key of the entity to be deleted.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A boolean value indicating whether the deletion was successful.</returns>
    protected override async Task<bool> OnDeleteAsync(TKey key, CancellationToken cancellationToken)
    {
        var item = await RawDbSet.FindAsync([key], cancellationToken);

        if (item is null)
        {
            return false;
        }

        RawDbSet.Remove(item);
        return true;
    }

    /// <summary>
    /// Defines an asynchronous method for finding an entity in the database based on its key. This method takes the key of the entity to be found, a boolean indicating whether to track changes, and a cancellation token as parameters. If tracking changes is enabled, it uses the DbSet's FindAsync method to retrieve the entity. If tracking changes is disabled, it performs a query using AsNoTracking to find the entity based on its key. The method returns the found entity or null if no matching entity is found. This method is designed to be overridden by derived classes to provide specific implementation details for finding entities in the database while adhering to the repository pattern and leveraging Entity Framework Core's capabilities for data management.
    /// </summary>
    /// <param name="key">The key of the entity to be found.</param>
    /// <param name="trackChanges">A boolean indicating whether to track changes.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The found entity or null if no matching entity is found.</returns>
    protected override async Task<TDb?> OnFindAsync(TKey key, bool trackChanges, CancellationToken cancellationToken)
    {
        if (trackChanges)
        {
            return await RawDbSet.FindAsync([key], cancellationToken);
        }

        return await DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(key), cancellationToken);
    }

    /// <summary>
    /// Defines an asynchronous method for finding an entity in the database based on an array of keys. This method takes an array of keys, a boolean indicating whether to track changes, and a cancellation token as parameters. If tracking changes is enabled, it uses the DbSet's FindAsync method to retrieve the entity based on the provided keys. If tracking changes is disabled, it performs a query using AsNoTracking to find the entity based on the first key in the array. The method returns the found entity or null if no matching entity is found. This method is designed to be overridden by derived classes to provide specific implementation details for finding entities in the database while adhering to the repository pattern and leveraging Entity Framework Core's capabilities for data management.
    /// </summary>
    /// <param name="keys">An array of keys of the entity to be found.</param>
    /// <param name="trackChanges">A boolean indicating whether to track changes.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The found entity or null if no matching entity is found.</returns>
    protected override async Task<TDb?> OnFindAsync(object[] keys, bool trackChanges, CancellationToken cancellationToken)
    {
        if (trackChanges)
        {
            return await RawDbSet.FindAsync(keys, cancellationToken);
        }

        if (keys.Length > 0)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.Id.Equals(keys.First()), cancellationToken);
        }

        return null;
    }

    /// <summary>
    /// Defines an asynchronous method for retrieving a paged list of entities from the database based on a request object. This method takes a request object and a cancellation token as parameters. It applies filtering to the DbSet using the filter factory based on the provided request, resulting in a filtered query and the total count of matching entities. The method then executes the query to retrieve the paged list of entities and returns both the data and the total count as a tuple. This method is designed to be overridden by derived classes to provide specific implementation details for retrieving paged data from the database while adhering to the repository pattern and leveraging Entity Framework Core's capabilities for data management.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object used for filtering and paging.</typeparam>
    /// <param name="request">The request object containing filtering and paging information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A tuple containing the paged list of entities and the total count of matching entities.</returns>
    protected override async Task<(IEnumerable<TDb> data, int totalRows)> OnGetPagedAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        var filteredResult = FilterFactory.ApplyPaged(DbSet, request);

        var (query, totalCount) = filteredResult;

        return (await OrderBy(request, query).ToArrayAsync(cancellationToken), totalCount);
    }

    /// <summary>
    /// Defines an asynchronous method for updating an existing entity in the database. This method takes an instance of the database entity, the raw entity, and a cancellation token as parameters. It updates the entity in the DbSet and returns the key of the updated entity. The method is designed to be overridden by derived classes to provide specific implementation details for updating entities in the database. By utilizing this method, developers can ensure that existing entities are properly updated in the database while adhering to the repository pattern and leveraging Entity Framework Core's capabilities for data management.
    /// </summary>
    /// <param name="entry">The database entity to be updated.</param>
    /// <param name="rawEntry">The raw entity containing the updated values.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The key of the updated entity.</returns>
    protected override async Task<TKey> OnUpdateAsync(TDb entry, T rawEntry, CancellationToken cancellationToken)
    {
        if (upsertBehaviour == RepositoryUpsertBehaviour.ForwardOnly)
        {
            throw new InvalidOperationException("Update operations are not allowed with ForwardOnly upsert type repositories.");
        }

        await Task.CompletedTask;
        LastTrackedEntry = RawDbSet.Update(entry);
        return entry.Id;
    }

    /// <summary>
    /// On update method that is called when an entity is being updated. This method checks if the database entity and the DTO both implement the IHasRowVersion interface. If they do, it sets the original value of the RowVersion property in the database entity to the value of the RowVersion property in the DTO. This is typically done to handle concurrency control when updating entities in a database, ensuring that the update operation is based on the correct version of the entity. By overriding this method, derived classes can implement specific logic for handling row versioning during updates while adhering to the repository pattern and leveraging Entity Framework Core's capabilities for data management.
    /// </summary>
    /// <param name="dbValue">The database entity being updated.</param>
    /// <param name="dto">The DTO containing the updated values.</param>
    protected override void OnUpdate(TDb dbValue, T dto)
    {
        if (dbValue is IHasRowVersion dbRowVersion && dto is IHasRowVersion rowVersion)
        {
            if (dbRowVersion.RowVersion is not null)
            {
                RawDbSet.Entry(dbValue)
                    .Property("RowVersion")
                    .OriginalValue = rowVersion.RowVersion;
            }
        }
    }

    /// <summary>
    /// Saves the changes made to the entities in the database context asynchronously. This method overrides the base implementation to call the SaveChangesAsync method of the DbContext, passing the provided cancellation token. It returns the number of state entries written to the database as a result of the save operation. By overriding this method, derived classes can ensure that changes made to entities are properly persisted to the database while adhering to the repository pattern and leveraging Entity Framework Core's capabilities for data management.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Defines a method for ordering a query based on the provided request. This method checks if the request implements the IOrderedRequest interface and if it contains a valid OrderBy property. If both conditions are met, it applies the specified ordering to the query using the ApplyOrdering extension method. If the request does not implement IOrderedRequest or does not contain a valid OrderBy property, it defaults to ordering the query by the Id property of the entities. This method is designed to be overridden by derived classes to provide specific implementation details for ordering queries based on different types of requests while adhering to the repository pattern and leveraging Entity Framework Core's capabilities for data management.
    /// </summary>
    /// <param name="request">The request containing any parameters for ordering.</param>
    /// <param name="query">The query to order.</param>
    /// <returns>The ordered query.</returns>
    protected virtual IQueryable<TDb> OrderBy<TRequest>(TRequest request, IQueryable<TDb> query)
    {
        if (request is IOrderedRequest orderedRequest && !string.IsNullOrEmpty(orderedRequest.OrderBy))
        {
            return query.ApplyOrdering(orderedRequest.OrderBy, 
                orderedRequest.DefaultOrderDirection?.ToDirectionString());
        }

        return query.OrderBy(x => x.Id);
    }

    /// <inheritdoc />
    public override Task<IUnitResult<TKey>> UpsertAsync(T entry, CancellationToken cancellationToken)
    {
        EntityInterceptorFactory.ScopedResources = scopedResources;

        if (!scopedResources.Contains<TDbContext>())
        {
            scopedResources.AddOrUpdate(Db);
        }

        return base.UpsertAsync(entry, cancellationToken);
    }

    /// <inheritdoc />
    public IDbTransaction BeginTransaction()
    {
        if (Db.Database.CurrentTransaction is not null)
        {
            throw new InvalidOperationException("Transaction already in progress.");
        }

        return new DbTransactionWrapper(Db.Database.BeginTransaction());
    }

    /// <inheritdoc />
    public IDbTransaction GetCurrentTransaction()
    {
        return Db.Database.CurrentTransaction is null
            ? throw new InvalidOperationException("No active transaction.")
            : new DbTransactionWrapper(Db.Database.CurrentTransaction);
    }

    /// <inheritdoc />
    public void Commit(IDbTransaction transaction)
    {
        transaction.Commit();
    }

    /// <inheritdoc />
    public void Rollback(IDbTransaction transaction)
    {
        transaction.Rollback();
    }

    /// <inheritdoc />
    public Task CommitAsync(IDbTransaction transaction, CancellationToken cancellationToken)
    {
        return transaction.CommitAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task RollbackAsync(IDbTransaction transaction, CancellationToken cancellationToken)
    {
        return transaction.RollbackAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override async Task OnReloadEntityAsync(TDb entity)
    {
        await RawDbSet.Entry(entity).ReloadAsync();
    }
}
