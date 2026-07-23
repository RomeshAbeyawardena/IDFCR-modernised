using IDFCR.Abstractions.Metadata;
using IDFCR.Persistence.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace IDFCR.Persistence.EntityFrameworkCore.Tests;

public class PackageDbContext(DbContextOptions<PackageDbContext> options) : DbContext(options)
{
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<GroupTag> GroupsTag => Set<GroupTag>();
}

public class Group : IIdentifiable<Guid>
{
    public ICollection<GroupTag> Tags { get; set; } = [];
    public Guid Id { get; set; }
}

public class Tag : INamed, IIdentifiable<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class GroupTag : IIdentifiable<Guid>
{
    public Guid Id { get; set; }
    public Guid TagId { get; set; }
    public Guid GroupId { get; set; }
    public Group? Group { get; set; }
    public Tag? Tag { get; set; }
}

[TestFixture]
internal class DeltaExtensionTests
{
    private static NamedDeltaOptions<Tag, GroupTag, Guid, Guid> CreateTagOptions()
    {
        return new NamedDeltaOptions<Tag, GroupTag, Guid, Guid>
        {
            NormalizeName = name => name.Trim(),
            NameComparer = StringComparer.OrdinalIgnoreCase,
            FilterEntitiesByNames = (query, names) => query.Where(x => names.Contains(x.Name)),
            GetEntityName = tag => tag.Name,
            GetEntityId = tag => tag.Id,
            CreateNewEntity = name => new Tag { Id = Guid.NewGuid(), Name = name },
            FilterExistingJoins = (query, parentId, childIds) => query.Where(x => x.GroupId == parentId && childIds.Contains(x.TagId)),
            CreateNewJoin = (groupId, tag) => new GroupTag { Id = Guid.NewGuid(), GroupId = groupId, TagId = tag.Id, Tag = tag },
            GetJoinChildId = groupTag => groupTag.TagId,
            IsLocalJoin = (join, groupId, tag) => join.GroupId == groupId && ReferenceEquals(join.Tag, tag)
        };
    }

    private static PackageDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PackageDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PackageDbContext(options);
    }

    [Test]
    public async Task PerformDeltaAsync_WhenDeltaIsEffectivelyEmpty_ReturnsZeroes()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());

        var delta = new StringListDelta
        {
            Add = ["  ", "\t", ""],
            Remove = ["  ", ""]
        };

        var result = await delta.PerformDeltaAsync(context, Guid.NewGuid(), CreateTagOptions());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EntitiesCreated, Is.EqualTo(0));
            Assert.That(result.RelationshipsAdded, Is.EqualTo(0));
            Assert.That(result.RelationshipsRemoved, Is.EqualTo(0));
            Assert.That(context.Tags, Is.Empty);
            Assert.That(context.GroupsTag, Is.Empty);
        }
    }

    [Test]
    public async Task PerformDeltaAsync_WhenAddAndRemoveContainSameName_AddWins()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var groupId = Guid.NewGuid();

        var delta = new StringListDelta
        {
            Add = [" TagA ", "taga"],
            Remove = ["TAGA"]
        };

        var result = await delta.PerformDeltaAsync(context, groupId, CreateTagOptions());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EntitiesCreated, Is.EqualTo(1));
            Assert.That(result.RelationshipsAdded, Is.EqualTo(1));
            Assert.That(result.RelationshipsRemoved, Is.EqualTo(0));
            Assert.That(context.Tags.Local.Count, Is.EqualTo(1));
            Assert.That(context.GroupsTag.Local.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task PerformDeltaAsync_WhenEntityExistsInDatabase_AddsRelationshipWithoutCreatingEntity()
    {
        var dbName = Guid.NewGuid().ToString();
        var groupId = Guid.NewGuid();
        var existingTagId = Guid.NewGuid();

        await using (var seedContext = CreateContext(dbName))
        {
            seedContext.Tags.Add(new Tag { Id = existingTagId, Name = "TagA" });
            await seedContext.SaveChangesAsync();
        }

        await using var context = CreateContext(dbName);

        var delta = new StringListDelta
        {
            Add = ["TagA"]
        };

        var result = await delta.PerformDeltaAsync(context, groupId, CreateTagOptions());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EntitiesCreated, Is.EqualTo(0));
            Assert.That(result.RelationshipsAdded, Is.EqualTo(1));
            Assert.That(result.RelationshipsRemoved, Is.EqualTo(0));
            Assert.That(context.Tags.Count(), Is.EqualTo(1));
            Assert.That(context.GroupsTag.Local.Count, Is.EqualTo(1));
            Assert.That(context.GroupsTag.Local.Single().TagId, Is.EqualTo(existingTagId));
        }
    }

    [Test]
    public async Task PerformDeltaAsync_WhenEntityExistsLocally_UsesLocalEntityAndAddsJoin()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var groupId = Guid.NewGuid();

        var localTag = new Tag { Id = Guid.NewGuid(), Name = "TagA" };
        context.Tags.Add(localTag);

        var delta = new StringListDelta
        {
            Add = [" taga "]
        };

        var result = await delta.PerformDeltaAsync(context, groupId, CreateTagOptions());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EntitiesCreated, Is.EqualTo(0));
            Assert.That(result.RelationshipsAdded, Is.EqualTo(1));
            Assert.That(result.RelationshipsRemoved, Is.EqualTo(0));
            Assert.That(context.Tags.Local.Count, Is.EqualTo(1));
            Assert.That(context.GroupsTag.Local.Count, Is.EqualTo(1));
            Assert.That(context.GroupsTag.Local.Single().Tag, Is.SameAs(localTag));
        }
    }

    [Test]
    public async Task PerformDeltaAsync_WhenRelationshipExistsInDatabase_DoesNotAddDuplicateJoin()
    {
        var dbName = Guid.NewGuid().ToString();
        var groupId = Guid.NewGuid();
        var existingTagId = Guid.NewGuid();

        await using (var seedContext = CreateContext(dbName))
        {
            seedContext.Tags.Add(new Tag { Id = existingTagId, Name = "TagA" });
            seedContext.GroupsTag.Add(new GroupTag { Id = Guid.NewGuid(), GroupId = groupId, TagId = existingTagId });
            await seedContext.SaveChangesAsync();
        }

        await using var context = CreateContext(dbName);
        var delta = new StringListDelta
        {
            Add = ["TagA"]
        };

        var result = await delta.PerformDeltaAsync(context, groupId, CreateTagOptions());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EntitiesCreated, Is.EqualTo(0));
            Assert.That(result.RelationshipsAdded, Is.EqualTo(0));
            Assert.That(result.RelationshipsRemoved, Is.EqualTo(0));
            Assert.That(context.GroupsTag.Count(), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task PerformDeltaAsync_WhenRelationshipExistsLocally_DoesNotAddDuplicateJoin()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var groupId = Guid.NewGuid();

        var localTag = new Tag { Id = Guid.NewGuid(), Name = "TagA" };
        context.Tags.Add(localTag);

        context.GroupsTag.Add(new GroupTag
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            TagId = localTag.Id,
            Tag = localTag
        });

        var delta = new StringListDelta
        {
            Add = ["TagA"]
        };

        var result = await delta.PerformDeltaAsync(context, groupId, CreateTagOptions());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EntitiesCreated, Is.EqualTo(0));
            Assert.That(result.RelationshipsAdded, Is.EqualTo(0));
            Assert.That(result.RelationshipsRemoved, Is.EqualTo(0));
            Assert.That(context.GroupsTag.Local.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task PerformDeltaAsync_WhenPersistedRelationshipRequestedForRemoval_RemovesJoin()
    {
        var dbName = Guid.NewGuid().ToString();
        var groupId = Guid.NewGuid();
        var existingTagId = Guid.NewGuid();

        await using (var seedContext = CreateContext(dbName))
        {
            seedContext.Tags.Add(new Tag { Id = existingTagId, Name = "TagA" });
            seedContext.GroupsTag.Add(new GroupTag { Id = Guid.NewGuid(), GroupId = groupId, TagId = existingTagId });
            await seedContext.SaveChangesAsync();
        }

        await using var context = CreateContext(dbName);
        var delta = new StringListDelta
        {
            Remove = ["TagA"]
        };

        var result = await delta.PerformDeltaAsync(context, groupId, CreateTagOptions());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EntitiesCreated, Is.EqualTo(0));
            Assert.That(result.RelationshipsAdded, Is.EqualTo(0));
            Assert.That(result.RelationshipsRemoved, Is.EqualTo(1));
        }

        await context.SaveChangesAsync();

        await using var verifyContext = CreateContext(dbName);
        Assert.That(verifyContext.GroupsTag.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task PerformDeltaAsync_WhenRemovalNameDoesNotResolve_DoesNothing()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var delta = new StringListDelta
        {
            Remove = ["Ghost"]
        };

        var result = await delta.PerformDeltaAsync(context, Guid.NewGuid(), CreateTagOptions());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EntitiesCreated, Is.EqualTo(0));
            Assert.That(result.RelationshipsAdded, Is.EqualTo(0));
            Assert.That(result.RelationshipsRemoved, Is.EqualTo(0));
            Assert.That(context.Tags, Is.Empty);
            Assert.That(context.GroupsTag, Is.Empty);
        }
    }

    [Test]
    public async Task PerformDeltaAsync_WhenMixedDeltaProvided_AppliesCreateAddAndRemove()
    {
        var dbName = Guid.NewGuid().ToString();
        var groupId = Guid.NewGuid();
        var removableTagId = Guid.NewGuid();
        var existingTagWithoutJoinId = Guid.NewGuid();

        await using (var seedContext = CreateContext(dbName))
        {
            seedContext.Tags.Add(new Tag { Id = removableTagId, Name = "OldTag" });
            seedContext.Tags.Add(new Tag { Id = existingTagWithoutJoinId, Name = "KeepTag" });
            seedContext.GroupsTag.Add(new GroupTag { Id = Guid.NewGuid(), GroupId = groupId, TagId = removableTagId });
            await seedContext.SaveChangesAsync();
        }

        await using var context = CreateContext(dbName);

        var delta = new StringListDelta
        {
            Add = ["KeepTag", "NewTag"],
            Remove = ["OldTag"]
        };

        var result = await delta.PerformDeltaAsync(context, groupId, CreateTagOptions());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EntitiesCreated, Is.EqualTo(1));
            Assert.That(result.RelationshipsAdded, Is.EqualTo(2));
            Assert.That(result.RelationshipsRemoved, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task PerformDeltaAsync_WhenRemovingLocalJoinForAddedEntity_RemovesByLocalReference()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var groupId = Guid.NewGuid();

        var localTag = new Tag { Id = Guid.NewGuid(), Name = "TagA" };
        context.Tags.Add(localTag);

        var localJoin = new GroupTag
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            TagId = localTag.Id,
            Tag = localTag
        };
        context.GroupsTag.Add(localJoin);

        var delta = new StringListDelta
        {
            Remove = ["TagA"]
        };

        var result = await delta.PerformDeltaAsync(context, groupId, CreateTagOptions());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EntitiesCreated, Is.EqualTo(0));
            Assert.That(result.RelationshipsAdded, Is.EqualTo(0));
            Assert.That(result.RelationshipsRemoved, Is.EqualTo(1));
            Assert.That(context.Entry(localJoin).State, Is.EqualTo(EntityState.Deleted));
        }
    }

    [Test]
    public async Task PerformDeltaAsync_WhenLocalEntityIsMarkedDeleted_IgnoresItAndCreatesNewEntity()
    {
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var groupId = Guid.NewGuid();

        var deletedTag = new Tag { Id = Guid.NewGuid(), Name = "TagA" };
        context.Tags.Add(deletedTag);
        context.Tags.Remove(deletedTag);

        var delta = new StringListDelta
        {
            Add = ["TagA"]
        };

        var result = await delta.PerformDeltaAsync(context, groupId, CreateTagOptions());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EntitiesCreated, Is.EqualTo(1));
            Assert.That(result.RelationshipsAdded, Is.EqualTo(1));
            Assert.That(context.ChangeTracker.Entries<Tag>().Count(entry => entry.State == EntityState.Added), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task PerformDeltaAsync_WhenJoinAppearsInDbAndLocal_RemovalIsDeduplicated()
    {
        var dbName = Guid.NewGuid().ToString();
        var groupId = Guid.NewGuid();
        var existingTagId = Guid.NewGuid();

        await using (var seedContext = CreateContext(dbName))
        {
            seedContext.Tags.Add(new Tag { Id = existingTagId, Name = "TagA" });
            seedContext.GroupsTag.Add(new GroupTag { Id = Guid.NewGuid(), GroupId = groupId, TagId = existingTagId });
            await seedContext.SaveChangesAsync();
        }

        await using var context = CreateContext(dbName);
        var options = CreateTagOptions();
        options.IsLocalJoin = (join, parentId, tag) => join.GroupId == parentId && join.TagId == tag.Id;

        var delta = new StringListDelta
        {
            Remove = ["TagA"]
        };

        var result = await delta.PerformDeltaAsync(context, groupId, options);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.EntitiesCreated, Is.EqualTo(0));
            Assert.That(result.RelationshipsAdded, Is.EqualTo(0));
            Assert.That(result.RelationshipsRemoved, Is.EqualTo(1));
        }
    }
}
