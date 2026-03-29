using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildTools.Infrastructure.SqlServer.Features;

public abstract class EntityFrameworkRepositoryBase<TDbContext, TCommon, TDb, T, TKey>(TDbContext db, IFilterFactory filterFactory, IEntityInterceptorFactory entityInterceptorFactory)
    : RepositoryBase<TCommon, TDb, T, TKey>(entityInterceptorFactory)
    where TDbContext : DbContext
    where TKey : struct
    where TDb : class, IMapper<TCommon>, TCommon, IIdentifiable<TKey>
    where T : class, IMapper<TCommon>, TCommon
{
    protected IFilterFactory FilterFactory { get; } = filterFactory;
    protected DbSet<TDb> DbSet { get; } = db.Set<TDb>();
    protected override async Task<TKey> OnAddAsync(TDb entry, T rawEntry, CancellationToken cancellationToken)
    {
        await DbSet.AddAsync(entry, cancellationToken);
        return entry.Id;
    }

    protected override async Task<bool> OnDeleteAsync(TKey key, CancellationToken cancellationToken)
    {
        var item = await DbSet.FindAsync([key], cancellationToken);

        if (item is null)
        {
            return false;
        }

        DbSet.Remove(item);
        return true;
    }

    protected override async Task<TDb?> OnFindAsync(TKey key, bool trackChanges, CancellationToken cancellationToken)
    {
        if (trackChanges)
        {
            return await DbSet.FindAsync([key], cancellationToken);
        }

        return await DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(key), cancellationToken);
    }

    protected override async Task<TDb?> OnFindAsync(object[] keys, bool trackChanges, CancellationToken cancellationToken)
    {
        if (trackChanges)
        {
            return await DbSet.FindAsync(keys, cancellationToken);
        }

        if (keys.Length > 0)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(keys.First()), cancellationToken);
        }

        return null;
    }

    protected override async Task<(IEnumerable<TDb> data, int totalRows)> OnGetPagedAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        var filteredResult = FilterFactory.ApplyPaged(DbSet, request);

        var (query, totalCount) = filteredResult;

        return (await query.ToArrayAsync(cancellationToken), totalCount);
    }

    protected override async Task<TKey> OnUpdateAsync(TDb entry, T rawEntry, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        DbSet.Update(entry);
        return entry.Id;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return db.SaveChangesAsync(cancellationToken);
    }
}
