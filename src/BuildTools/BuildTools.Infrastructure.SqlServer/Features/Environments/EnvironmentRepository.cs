using BuildTools.Infrastructure.Features.Environments;
using BuildTools.Shared.Features.Environments;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Results;
using IDFCR.Persistence.EntityFrameworkCore;
using IDFCR.Persistence.EntityFrameworkCore.Attributes;
using Microsoft.EntityFrameworkCore;
using EnvironmentDto = BuildTools.Shared.Features.Environments.Environment;

namespace BuildTools.Infrastructure.SqlServer.Features.Environments;

[RegisteredRepository]
public class EnvironmentRepository(PackageManagerDbContext db, IFilterFactory filterFactory, IEntityInterceptorFactory entityInterceptorFactory)
    : EntityFrameworkRepositoryBase<PackageManagerDbContext, IEnvironment, EnvironmentEntity, EnvironmentDto, Guid>(db, filterFactory, entityInterceptorFactory),
    IEnvironmentRepository
{
    public async Task<IUnitResult<EnvironmentDto>> GetEnvironmentAsync(IEnvironmentQuery request, CancellationToken cancellationToken)
    {
        var query = FilterFactory.Apply(DbSet.AsNoTracking(), request);

        var foundResult = await query.FirstOrDefaultAsync(cancellationToken);

        if (foundResult is null)
        {
            return UnitResult.NotFound<EnvironmentDto>(query);
        }

        return UnitResult.FromResult(Map(foundResult));
    }

    public async Task<bool> IsEnvironmentInUseAsync(Guid environmentId, CancellationToken cancellationToken)
    {
        return await DbSet.AnyAsync(x => x.Id == environmentId && x.Settings.Any(), cancellationToken);
    }

    protected override bool IsHandled(Exception exception)
    {
        return true;
    }
}
