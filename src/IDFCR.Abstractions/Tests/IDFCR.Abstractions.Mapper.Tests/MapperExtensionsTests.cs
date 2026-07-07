using IDFCR.Abstractions.Mapper.Extensions;

using NUnit.Framework;

using System.Linq.Expressions;

namespace IDFCR.Abstractions.Mapper.Tests;

[TestFixture]
internal class MapperExtensionsTests
{
    private sealed class Source
    {
        public int Number { get; set; }
        public string? Text { get; set; }
        public Guid Id { get; set; }
    }

    private sealed class Target
    {
        public int Number { get; set; }
        public string? Text { get; set; }
        public Guid Id { get; set; }
    }

    [Test]
    public void SetIfNotNullOrDefault_WhenCanSetIsFalse_DoesNotModifyTarget()
    {
        Source source = new()
        {
            Number = 502
        };

        Target target = new()
        {
            Number = 501
        };

        source.SetIfNotNullOrDefault(n => n.Number, target, false);

        Assert.That(target.Number, Is.EqualTo(501));
    }

    [Test]
    public void SetIfNotNullOrDefault_WhenCanSetIsFalse_ShortCircuitsBeforeNullValidation()
    {
        Source source = null!;
        Target target = null!;
        Expression<Func<Source, int>> selector = null!;

        Assert.DoesNotThrow(() => source.SetIfNotNullOrDefault(selector, target, false));
    }

    [Test]
    public void SetIfNotNullOrDefault_WhenValueTypeIsDefault_DoesNotModifyTarget()
    {
        Source source = new()
        {
            Number = default
        };

        Target target = new()
        {
            Number = 501
        };

        source.SetIfNotNullOrDefault(n => n.Number, target, true);

        Assert.That(target.Number, Is.EqualTo(501));
    }

    [Test]
    public void SetIfNotNullOrDefault_WhenReferenceTypeIsNull_DoesNotModifyTarget()
    {
        Source source = new()
        {
            Text = null
        };

        Target target = new()
        {
            Text = "Jack"
        };

        source.SetIfNotNullOrDefault(n => n.Text, target, true);

        Assert.That(target.Text, Is.EqualTo("Jack"));
    }

    [Test]
    public void SetIfNotNullOrDefault_WhenValueTypeIsNotDefault_SetsTargetProperty()
    {
        Source source = new()
        {
            Number = 502
        };

        Target target = new()
        {
            Number = 501
        };

        source.SetIfNotNullOrDefault(n => n.Number, target, true);

        Assert.That(target.Number, Is.EqualTo(502));
    }

    [Test]
    public void SetIfNotNullOrDefault_WhenReferenceTypeIsNotNull_SetsTargetProperty()
    {
        Source source = new()
        {
            Text = "Updated"
        };

        Target target = new()
        {
            Text = "Initial"
        };

        source.SetIfNotNullOrDefault(n => n.Text, target, true);

        Assert.That(target.Text, Is.EqualTo("Updated"));
    }

    [Test]
    public void SetIfNotNullOrDefault_WhenGuidIsNotDefault_SetsTargetProperty()
    {
        Guid expected = Guid.NewGuid();
        Source source = new()
        {
            Id = expected
        };

        Target target = new()
        {
            Id = Guid.Empty
        };

        source.SetIfNotNullOrDefault(n => n.Id, target, true);

        Assert.That(target.Id, Is.EqualTo(expected));
    }

    [Test]
    public void SetIfNotNullOrDefault_WhenEntityIsNull_ThrowsArgumentNullException()
    {
        Source source = null!;
        Target target = new();

        Assert.Throws<ArgumentNullException>(() => source.SetIfNotNullOrDefault(n => n.Number, target, true));
    }

    [Test]
    public void SetIfNotNullOrDefault_WhenTargetIsNull_ThrowsArgumentNullException()
    {
        Source source = new()
        {
            Number = 10
        };

        Target target = null!;

        Assert.Throws<ArgumentNullException>(() => source.SetIfNotNullOrDefault(n => n.Number, target, true));
    }

    [Test]
    public void SetIfNotNullOrDefault_WhenExpressionIsNull_ThrowsArgumentNullException()
    {
        Source source = new()
        {
            Number = 10
        };

        Target target = new();
        Expression<Func<Source, int>> selector = null!;

        Assert.Throws<ArgumentNullException>(() => source.SetIfNotNullOrDefault(selector, target, true));
    }

    [Test]
    public void SetIfNotNullOrDefault_WhenExpressionIsNotMemberAccess_ThrowsArgumentException()
    {
        Source source = new()
        {
            Number = 10
        };

        Target target = new();

        Assert.Throws<ArgumentException>(() => source.SetIfNotNullOrDefault(_ => 1, target, true));
    }
}
