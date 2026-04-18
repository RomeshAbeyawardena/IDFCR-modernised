using IDFCR.Abstractions.Mediator;
using IDFCR.Abstractions.Persistence;

namespace BuildTools.Shared.Contracts.Feature.Tags;

public record GetTagQuery : IUnitResultRequest<TagDto>
{
    public IEnumerable<string> Names { get; set; } = [];
    public string? Name { get; init; }
    public string? NameContains { get; init; }
}

public record UpsertTagCommand : IUnitResultRequest<object>, IUnitOfWorkRequest
{
    public TagDto? Tag { get; init; }
    public bool CommitChanges { get; init; }
}