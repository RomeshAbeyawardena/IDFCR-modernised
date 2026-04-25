using IDFCR.Abstractions.Mediator;
using IDFCR.Abstractions.Persistence;

namespace BuildTools.Shared.Contracts.Features.SystemSettings
{
    public record UpsertSystemSettingCommand : IUnitResultRequest<object>, IUnitOfWorkRequest
    {
        public SystemSettingDto? Setting { get; init; }
        public bool CommitChanges { get; init; }
    }
}
