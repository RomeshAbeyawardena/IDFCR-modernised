using IDFCR.Abstractions.Mediator;
using IDFCR.Abstractions.Persistence;

namespace BuildTools.Shared.Contracts.Features.Tags;

public record UpsertTagCommand : IUnitResultRequest<object>, IUnitOfWorkRequest
{
    public TagDto? Tag { get; init; }
    public bool CommitChanges { get; init; }
}