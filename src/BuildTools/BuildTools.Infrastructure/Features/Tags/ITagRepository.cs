using BuildTools.Shared.Features.Tags;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;

namespace BuildTools.Infrastructure.Features.Tags;

public interface ITagRepository : IRepository<Tag, Guid>
{
    Task<IUnitResult<Tag>> GetTagAsync(string name, CancellationToken cancellationToken);
    Task<IUnitResultCollection<Tag>> GetExistingTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken);

    Task<IUnitResult> AddTagsAsync(IEnumerable<Tag> tags, CancellationToken cancellationToken);
}
