using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;

namespace BuildTools.Infrastructure.Features.Settings;

public interface ISettingRepository : IRepository<Setting, Guid>
{
    Task<IUnitResult<Setting>> GetSettingAsync(string key, string? type, CancellationToken cancellationToken);
    Task<IUnitResult<string>> GetValueAsync(string key, string? type, CancellationToken cancellationToken);
}
