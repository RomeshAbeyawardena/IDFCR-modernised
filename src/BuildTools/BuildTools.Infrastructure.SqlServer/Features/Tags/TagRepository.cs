using BuildTools.Infrastructure.Features.Tags;
using BuildTools.Shared.Features.Tags;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Results;
using Microsoft.EntityFrameworkCore;

namespace BuildTools.Infrastructure.SqlServer.Features.Tags;

[RegisteredRepository]
public class TagRepository(PackageManagerDbContext db, IFilterFactory filterFactory, IEntityInterceptorFactory entityInterceptorFactory) 
    : EntityFrameworkRepositoryBase<PackageManagerDbContext, ITag, TagEntity, Tag, Guid>(db, filterFactory, entityInterceptorFactory), ITagRepository
{
    public async Task<IUnitResult<Tag>> GetTagAsync(string name, CancellationToken cancellationToken)
    {
        var query = FilterFactory.Apply(DbSet.AsNoTracking(), new GetPagedTagsQuery
        {
            Name = name
        });

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return UnitResult.NotFound<Tag>(name);
        }

        return UnitResult.FromResult(base.Map(result));
    }

    protected override bool IsHandled(Exception exception)
    {
        return true;
    }
}