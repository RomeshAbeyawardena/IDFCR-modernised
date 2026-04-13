using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;
using EnvironmentDto = BuildTools.Shared.Features.Environments.Environment;
namespace BuildTools.Infrastructure.Features.Environments;

public interface IEnvironmentRepository : IRepository<EnvironmentDto, Guid>
{
    Task<IUnitResult<EnvironmentDto>> GetEnvironmentAsync(IEnvironmentQuery query, CancellationToken cancellationToken);
}
