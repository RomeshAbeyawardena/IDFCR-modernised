using BuildTools.Shared.Features.Packages;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;

namespace BuildTools.Infrastructure.Features.Packages;

public enum TagAssignmentStatus { Added, Assigned, Duplicated }
public enum TagUnassignmentStatus { Unassigned, NotFound }
public record TagAssignmentResult(string TagName, TagAssignmentStatus AssignmentStatus);
public record TagUnassignmentResult(string TagName, TagUnassignmentStatus AssignmentStatus);

public record TagAssignmentRequest(Guid? PackageId = null,
        string? PackageNamespace = null,
        IEnumerable<string>? Tags = null,
        IEnumerable<object>? TagIds = null);

public interface IPackageRepository : IRepository<Package, Guid>
{
    Task<IUnitResult<Package>> GetPackageAsync(string? name, string? @namespace, CancellationToken cancellationToken);
    Task<IUnitResultCollection<TagAssignmentResult>> TryAssignTags(TagAssignmentRequest request, CancellationToken cancellationToken);
    Task<IUnitResultCollection<TagUnassignmentResult>> TryUnassignTags(TagAssignmentRequest request, CancellationToken cancellationToken);
}
