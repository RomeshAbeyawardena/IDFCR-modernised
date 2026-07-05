using IDFCR.Abstractions.DependencyInjection;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors.Factories;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Persistence.Extensions;
using IDFCR.Abstractions.Results;
using IDFCR.Abstractions.Results.Exceptions;
using Moq;

namespace IDFCR.TestUtilities;

/// <summary>
/// Provides an in-memory implementation of a repository for entities, supporting basic CRUD operations and paging
/// for testing or mock scenarios.
/// </summary>
/// <remarks>This repository is intended for use in testing or development environments where a persistent
/// data store is not required. All data is stored in memory and is lost when the repository instance is disposed or
/// goes out of scope. Thread safety is not guaranteed.</remarks>
/// <typeparam name="TCommon">The common interface or base type shared by the entity types managed by the repository.</typeparam>
/// <typeparam name="TDb">The database entity type, which must implement IMapper, TCommon, and </typeparam>
/// <typeparam name="T">The domain entity type, which must implement IMapper and TCommon.</typeparam>
/// <param name="entityInterceptorFactory">The factory used to create entity interceptors for handling entity lifecycle events.</param>
/// <param name="filterFactory">The factory used to apply filtering and paging to in-memory entity collections.</param>
/// <param name="scopedResources"></param>
public class InternalMemoryMockRepository<TCommon, TDb, T>(IEntityInterceptorFactory entityInterceptorFactory, IFilterFactory filterFactory, IScopedResources scopedResources)
    : RepositoryBase<TCommon, TDb, T, Guid>(entityInterceptorFactory)
    where TDb : class, IMapper<TCommon>, TCommon, IIdentifiable<Guid>
    where T : class, IMapper<TCommon>, TCommon
{
    private readonly List<TDb> entries = [];

    /// <summary>
    /// Gets the collection of entries managed by the current instance.
    /// </summary>
    protected List<TDb> Entries => entries;

    /// <summary>
    /// Sets a value indicating whether the current operation has been marked as handled.
    /// </summary>
    public bool IsHandledFlag { private get; set; } = false;
    /// <summary>
    /// Determines whether the specified exception is considered handled by this handler.
    /// </summary>
    /// <param name="exception">The exception to evaluate for handling.</param>
    /// <returns>true if the exception is handled by this handler; otherwise, false.</returns>
    protected override bool IsHandled(Exception exception)
    {
        return IsHandledFlag;
    }

    /// <summary>
    /// Asynchronously adds a new entry to the collection and assigns a unique identifier.
    /// </summary>
    /// <param name="entry">The entry to add to the collection. The entry's identifier will be set during the operation.</param>
    /// <param name="rawEntry">The raw data used to create or populate the entry. Used to provide additional context or values for the
    /// entry.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation is canceled if the token is triggered.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unique identifier assigned
    /// to the added entry.</returns>
    protected override Task<Guid> OnAddAsync(TDb entry, T rawEntry, CancellationToken cancellationToken)
    {
        //simulate db 
        entry.Id = Guid.NewGuid();
        entries.Add(entry);
        return Task.FromResult(entry.Id);
    }

    /// <summary>
    /// Attempts to delete the entry with the specified unique identifier asynchronously.
    /// </summary>
    /// <param name="key">The unique identifier of the entry to delete.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the delete operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the entry
    /// was found and deleted; otherwise, <see langword="false"/>.</returns>
    protected override Task<bool> OnDeleteAsync(Guid key, CancellationToken cancellationToken)
    {
        var entry = entries.Find(x => x.Id == key);

        if (entry is null)
        {
            return Task.FromResult(false);
        }

        entries.Remove(entry);

        return Task.FromResult(true);
    }

    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="key">The unique identifier of the entity to find.</param>
    /// <param name="trackChanges">A value indicating whether the returned entity should be tracked for changes.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous find operation. The task result contains the entity if found;
    /// otherwise, null.</returns>
    protected override Task<TDb?> OnFindAsync(Guid key, bool trackChanges, CancellationToken cancellationToken)
    {
        return Task.FromResult(entries.Find(x => x.Id == key));
    }

    /// <summary>
    /// Asynchronously finds an entity by its primary key values.
    /// </summary>
    /// <remarks>If the keys array is empty or the first element is not a valid GUID, the method
    /// returns null. Only the first key is used for lookup.</remarks>
    /// <param name="keys">An array of key values used to identify the entity. The first element must be a valid GUID or a string
    /// representation of a GUID.</param>
    /// <param name="trackChanges">true to track the returned entity in the context; otherwise, false.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous find operation. The task result contains the found entity, or null
    /// if no entity with the specified key is found.</returns>
    protected override Task<TDb?> OnFindAsync(object[] keys, bool trackChanges, CancellationToken cancellationToken)
    {
        if (keys.Length < 1)
        {
            return Task.FromResult<TDb?>(null);
        }
        var key = keys.FirstOrDefault();
        if (key is null || !Guid.TryParse(key?.ToString(), out var id))
        {
            return Task.FromResult<TDb?>(null);
        }

        return OnFindAsync(id, trackChanges, cancellationToken);
    }

    /// <summary>
    /// Retrieves a paged subset of data entries based on the specified request parameters.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object containing paging and filtering criteria.</typeparam>
    /// <param name="request">An object that specifies the paging and filtering options to apply when retrieving data.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the paged data
    /// entries and the total number of rows matching the criteria.</returns>
    protected override Task<(IEnumerable<TDb> data, int totalRows)> OnGetPagedAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        var (data, totalRows) = filterFactory.ApplyPaged(entries.AsQueryable(), request);
        return Task.FromResult<(IEnumerable<TDb> data, int totalRows)>((data.ToArray(), totalRows));
    }

    /// <summary>
    /// Updates an existing entry in the data store asynchronously and returns the unique identifier of the updated
    /// entry.
    /// </summary>
    /// <param name="entry">The updated entry data to apply to the existing entity. Must have a valid identifier corresponding to an
    /// existing entity.</param>
    /// <param name="rawEntry">The original, unprocessed entry data used as input for the update operation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unique identifier of the
    /// updated entry.</returns>
    /// <exception cref="EntityNotFoundException">Thrown if no existing entity with the same identifier as <paramref name="entry"/> is found.</exception>
    protected override Task<Guid> OnUpdateAsync(TDb entry, T rawEntry, CancellationToken cancellationToken)
    {
        var foundEntry = entries.Find(x => x.Id == entry.Id) ?? throw new EntityNotFoundException(typeof(TDb), entry.Id);
        foundEntry.Apply(entry);
        return Task.FromResult(foundEntry.Id);
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the underlying data store.
    /// </summary>
    /// <remarks>In this implementation, no changes are persisted because the context uses an
    /// in-memory list. The method always returns 0.</remarks>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous save operation.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries
    /// written to the underlying data store.</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        //No-op in an in-memory list
        return Task.FromResult(0);
    }

    /// <summary>
    /// On update is not implemented in this in-memory repository as the update logic is handled directly in the OnUpdateAsync method.
    /// </summary>
    /// <param name="db">The database entity being updated.</param>
    /// <param name="dto">The DTO containing the updated values.</param>
    /// <exception cref="NotImplementedException"></exception>
    protected override void OnUpdate(TDb db, T dto)
    {

    }

    /// <inheritdoc />
    public override Task<IUnitResult<Guid>> UpsertAsync(T entry, CancellationToken cancellationToken)
    {
        EntityInterceptorFactory.ScopedResources = scopedResources;

        if (!scopedResources.Contains<DbContextMarker<TDb>>())
        {
            EntityInterceptorFactory.ScopedResources.AddOrUpdate(new DbContextMarker<TDb>
            {
                Entries = entries
            });
        }

        return base.UpsertAsync(entry, cancellationToken);
    }

    /// <inheritdoc />
    protected override Task OnReloadEntityAsync(TDb entity)
    {
        return Task.CompletedTask;
    }
}

