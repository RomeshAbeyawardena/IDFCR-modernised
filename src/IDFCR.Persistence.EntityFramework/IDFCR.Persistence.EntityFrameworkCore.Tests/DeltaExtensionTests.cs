using IDFCR.Abstractions.Metadata;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace IDFCR.Persistence.EntityFrameworkCore.Tests;

public class PackageDbContext : DbContext
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<GroupTag> GroupsTag { get; set; }

}

public class Group : IIdentifiable<Guid>
{
    public ICollection<GroupTag> Tags { get; set; }
    public Guid Id { get; set; }
}

public class Tag : INamed, IIdentifiable<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class GroupTag : IIdentifiable<Guid>
{
    public Guid Id { get; set; }
    public Guid TagId { get; set; }
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
    public Tag Tag { get; set; }

}


[TestFixture]
internal class DeltaExtensionTests
{

    [Test]
    public void Test()
    {
        var tagOptions = new NamedDeltaOptions<Tag, GroupTag, Guid, Guid>
        {
            FilterEntitiesByNames = (query, names) => query.Where(x => names.Contains(x.Name)),
            GetEntityName = tag => tag.Name,
            GetEntityId = tag => tag.Id,
            CreateNewEntity = name => new Tag { Name = name },

            FilterExistingJoins = (query, pId, cIds) => query.Where(x => x.GroupId == pId && cIds.Contains(x.TagId)),
            CreateNewJoin = (groupId, tag) => new GroupTag { GroupId = groupId, Tag = tag },

            GetJoinChildId = groupTag => groupTag.TagId,
            IsLocalJoin = (join, groupId, tag) => join.GroupId == groupId && ReferenceEquals(join.Tag, tag)
        };
    }
}
