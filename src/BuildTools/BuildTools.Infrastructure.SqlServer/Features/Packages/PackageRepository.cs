using BuildTools.Infrastructure.Features.Packages;
using BuildTools.Shared.Features.Packages;
using IDFCR.Abstractions.Filters;
using IDFCR.Abstractions.Interceptors;
using IDFCR.Abstractions.Results;
using IDFCR.Persistence.EntityFrameworkCore;
using IDFCR.Persistence.EntityFrameworkCore.Attributes;
using Microsoft.EntityFrameworkCore;

namespace BuildTools.Infrastructure.SqlServer.Features.Packages;

[RegisteredRepository]
public class PackageRepository(PackageManagerDbContext db, IFilterFactory filterFactory, IEntityInterceptorFactory entityInterceptorFactory)
    : EntityFrameworkRepositoryBase<PackageManagerDbContext, IPackage, PackageEntity, Package, Guid>(
        db, filterFactory, entityInterceptorFactory), IPackageRepository
{
    public async Task<IUnitResult<Package>> GetPackageAsync(string? name, string? @namespace, CancellationToken cancellationToken)
    {
        var query = FilterFactory.Apply(DbSet.AsNoTracking(), new GetPagedPackagesQuery
        {
            Name = name,
            Namespace = @namespace
        });

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return UnitResult.NotFound<Package>(name ?? @namespace ?? "Unknown");
        }

        return UnitResult.FromResult(base.Map(result));
    }

    public async Task<IUnitResultCollection<TagAssignmentResult>> TryAssignTags(
        TagAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        request.Deconstruct(out var packageId, out var packageNamespace, out var tags, out var _tagIds);

        var tagIds = _tagIds?.OfType<Guid>().ToList() ?? [];

        List<TagAssignmentResult> tagAssignmentResults = [];
        var query = DbSet.Include(x => x.PackageTags);

        var foundPackage = packageId.HasValue
            ? await query
                .FirstOrDefaultAsync(x => x.Id == packageId, cancellationToken)
            : !string.IsNullOrWhiteSpace(packageNamespace)
                ? await query.FirstOrDefaultAsync(x => x.Namespace == packageNamespace, cancellationToken)
                : null;

        if (foundPackage is null)
        {
            return UnitResultCollection.Failed<TagAssignmentResult>(new InvalidOperationException("Unable to find package"));
        }

        var foundTags = tagIds.Any()
            ? Db.Tags.Where(x => tagIds.Contains(x.Id))
            : tags is not null
                ? Db.Tags.Where(x => tags.Contains(x.Name))
                : null;

        if (foundTags is null)
        {
            return UnitResultCollection.Failed<TagAssignmentResult>(new InvalidOperationException("Tags aren't supplied"));
        }

        // Single round trip
        var allFoundTags = await foundTags.ToArrayAsync(cancellationToken);

        foreach (var tag in allFoundTags)
        {
            var alreadyAssigned = foundPackage.PackageTags.Any(pt => pt.TagId == tag.Id);

            if (alreadyAssigned)
            {
                tagAssignmentResults.Add(new TagAssignmentResult(tag.Name, TagAssignmentStatus.Duplicated));
                continue;
            }

            foundPackage.PackageTags.Add(new PackageTagEntity { Tag = tag });
            tagAssignmentResults.Add(new TagAssignmentResult(tag.Name, TagAssignmentStatus.Assigned));
        }

        if (tags is not null)
        {
            var newTags = tags.Where(x => !allFoundTags.Any(f => f.Name.Equals(x, StringComparison.OrdinalIgnoreCase)));

            foreach (var tag in newTags)
            {
                foundPackage.PackageTags.Add(new PackageTagEntity { Tag = new Tags.TagEntity { Name = tag, DisplayName = tag } });
                tagAssignmentResults.Add(new TagAssignmentResult(tag, TagAssignmentStatus.Added));
            }
        }

        return UnitResultCollection.FromResult(tagAssignmentResults);
    }

    public async Task<IUnitResultCollection<TagUnassignmentResult>> TryUnassignTags(
        TagAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        request.Deconstruct(out var packageId, out var packageNamespace, out var tags, out var _tagIds);

        var tagIds = _tagIds?.OfType<Guid>().ToList() ?? [];

        List<TagUnassignmentResult> tagUnassignmentResults = [];
        var query = DbSet.Include(x => x.PackageTags);

        var foundPackage = packageId.HasValue
            ? await query.FirstOrDefaultAsync(x => x.Id == packageId, cancellationToken)
            : !string.IsNullOrWhiteSpace(packageNamespace)
                ? await query.FirstOrDefaultAsync(x => x.Namespace == packageNamespace, cancellationToken)
                : null;

        if (foundPackage is null)
        {
            return UnitResultCollection.Failed<TagUnassignmentResult>(new InvalidOperationException("Unable to find package"));
        }

        var foundTags = tagIds.Any()
            ? Db.Tags.Where(x => tagIds.Contains(x.Id))
            : tags is not null
                ? Db.Tags.Where(x => tags.Contains(x.Name))
                : null;

        if (foundTags is null)
        {
            return UnitResultCollection.Failed<TagUnassignmentResult>(new InvalidOperationException("Tags aren't supplied"));
        }

        // Single round trip
        var allFoundTags = await foundTags.ToArrayAsync(cancellationToken);

        foreach (var tag in allFoundTags)
        {
            var assignedLink = foundPackage.PackageTags.FirstOrDefault(pt => pt.TagId == tag.Id);

            if (assignedLink is null)
            {
                tagUnassignmentResults.Add(new TagUnassignmentResult(tag.Name, TagUnassignmentStatus.NotFound));
                continue;
            }

            foundPackage.PackageTags.Remove(assignedLink);
            tagUnassignmentResults.Add(new TagUnassignmentResult(tag.Name, TagUnassignmentStatus.Unassigned));
        }

        if (tags is not null)
        {
            var notFoundTags = tags.Where(x => !allFoundTags.Any(f => f.Name.Equals(x, StringComparison.OrdinalIgnoreCase)));

            foreach (var tag in notFoundTags)
            {
                tagUnassignmentResults.Add(new TagUnassignmentResult(tag, TagUnassignmentStatus.NotFound));
            }
        }

        return UnitResultCollection.FromResult(tagUnassignmentResults);
    }

    protected override bool IsHandled(Exception exception)
    {
        return true;
    }
}
