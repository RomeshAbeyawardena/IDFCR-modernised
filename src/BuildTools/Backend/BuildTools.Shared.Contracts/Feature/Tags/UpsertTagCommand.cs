using IDFCR.Abstractions.Mediator;
using IDFCR.Abstractions.Persistence;

namespace BuildTools.Shared.Contracts.Feature.Tags;

public record UpsertTagCommand : IUnitResultRequest<object>, IUnitOfWorkRequest
{
    public TagDto? Tag { get; init; }
    public bool CommitChanges { get; init; }
}