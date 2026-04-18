using IDFCR.Abstractions.Mediator;
using IDFCR.Abstractions.Persistence;

namespace BuildTools.Shared.Contracts.Features.Tags;

public record DeleteTagCommand : IUnitResultRequest, IUnitOfWorkRequest
{
    public required string Name { get; init; }
    public bool CommitChanges { get; init; }
}
