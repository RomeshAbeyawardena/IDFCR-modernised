using BuildTools.Infrastructure.Features.Environments;
using BuildTools.Shared.Features.Environments;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Results;
using IDFCR.Persistence.EntityFrameworkCore;
using IDFCR.Persistence.EntityFrameworkCore.Attributes;
using System.Data.Entity;
using EnvironmentDto = BuildTools.Shared.Features.Environments.Environment;

namespace BuildTools.Infrastructure.SqlServer.Features.Environments;

[RegisteredRepository]
public class EnvironmentRepository(PackageManagerDbContext db, IFilterFactory filterFactory, IEntityInterceptorFactory entityInterceptorFactory)
    : EntityFrameworkRepositoryBase<PackageManagerDbContext, IEnvironment, EnvironmentEntity, EnvironmentDto, Guid>(db, filterFactory, entityInterceptorFactory),
    IEnvironmentRepository
{
    public async Task<IUnitResult<EnvironmentDto>> GetEnvironmentAsync(IEnvironmentQuery query, CancellationToken cancellationToken)
    {
        var foundResult = await DbSet.AsNoTracking().FirstOrDefaultAsync(new EnvironmentPagedFilter().BuildPredicate(query), cancellationToken);

        if (foundResult is null)
        {
            return UnitResult.NotFound<EnvironmentDto>(query);
        }

        return UnitResult.FromResult(Map(foundResult));
    }

    protected override bool IsHandled(Exception exception)
    {
        return true;
    }
}
