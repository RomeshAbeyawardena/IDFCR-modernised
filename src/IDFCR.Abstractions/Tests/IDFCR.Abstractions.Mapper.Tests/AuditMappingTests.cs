using IDFCR.Abstractions.Metadata;
using NUnit.Framework;

namespace IDFCR.Abstractions.Mapper.Tests;

public interface IAuditableCustomer : IMapper<IAuditableCustomer>, IAuditCreatedTimestamp, IAuditModifiedTimestamp
{
}

public class AuditableCustomer : MapperBase<IAuditableCustomer>, IAuditableCustomer
{
    protected override void MapMembers(IAuditableCustomer source)
    {
    }

    public DateTimeOffset CreatedTimestampUtc { get; set; }
    public DateTimeOffset? ModifiedTimestampUtc { get; set; }
}

[TestFixture]
internal class AuditMappingTests
{
    [Test]
    public void Map_CopiesCreatedAndModifiedTimestamps_ToExistingTarget()
    {
        var created = new DateTimeOffset(2025, 1, 10, 8, 30, 0, TimeSpan.Zero);
        var modified = created.AddHours(2);

        var sourceCustomer = new AuditableCustomer
        {
            CreatedTimestampUtc = created,
            ModifiedTimestampUtc = modified
        };

        var targetCustomer = new AuditableCustomer();

        targetCustomer.Map(sourceCustomer);

        Assert.Multiple(() =>
        {
            Assert.That(targetCustomer.CreatedTimestampUtc, Is.EqualTo(created));
            Assert.That(targetCustomer.ModifiedTimestampUtc, Is.EqualTo(modified));
        });
    }

    [Test]
    public void Map_CopiesCreatedTimestamp_AndPreservesNullModifiedTimestamp()
    {
        var created = new DateTimeOffset(2025, 1, 10, 8, 30, 0, TimeSpan.Zero);

        var sourceCustomer = new AuditableCustomer
        {
            CreatedTimestampUtc = created,
            ModifiedTimestampUtc = null
        };

        var targetCustomer = new AuditableCustomer
        {
            CreatedTimestampUtc = created.AddDays(-1),
            ModifiedTimestampUtc = created.AddHours(4)
        };

        targetCustomer.Map(sourceCustomer);

        Assert.Multiple(() =>
        {
            Assert.That(targetCustomer.CreatedTimestampUtc, Is.EqualTo(created));
            Assert.That(targetCustomer.ModifiedTimestampUtc, Is.Null);
        });
    }

    [Test]
    public void Map_WithNullSource_DoesNotChangeTargetTimestamps()
    {
        var originalCreated = new DateTimeOffset(2025, 2, 1, 10, 0, 0, TimeSpan.Zero);
        var originalModified = originalCreated.AddMinutes(15);

        var targetCustomer = new AuditableCustomer
        {
            CreatedTimestampUtc = originalCreated,
            ModifiedTimestampUtc = originalModified
        };

        targetCustomer.Map(null!);

        Assert.Multiple(() =>
        {
            Assert.That(targetCustomer.CreatedTimestampUtc, Is.EqualTo(originalCreated));
            Assert.That(targetCustomer.ModifiedTimestampUtc, Is.EqualTo(originalModified));
        });
    }

    [Test]
    public void Map_ToNewInstance_CopiesAuditTimestamps()
    {
        var created = new DateTimeOffset(2025, 3, 3, 6, 0, 0, TimeSpan.Zero);
        var modified = created.AddMinutes(45);

        var sourceCustomer = new AuditableCustomer
        {
            CreatedTimestampUtc = created,
            ModifiedTimestampUtc = modified
        };

        var mappedCustomer = sourceCustomer.Map<AuditableCustomer>();

        Assert.Multiple(() =>
        {
            Assert.That(mappedCustomer.CreatedTimestampUtc, Is.EqualTo(created));
            Assert.That(mappedCustomer.ModifiedTimestampUtc, Is.EqualTo(modified));
        });
    }
}
