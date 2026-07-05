using NUnit.Framework;

namespace IDFCR.Abstractions.Interceptors.Tests.Assets;

public sealed class MyExpensiveTestResource : IDisposable
{
    public void Dispose() => GC.SuppressFinalize(this);
}

public sealed class AnotherTestResource;

[TestFixture]
internal class ScopedResourceTests
{
    private MyExpensiveTestResource expensiveTestResource = null!;
    private MyExpensiveTestResource expensiveTestResource2 = null!;

    [SetUp]
    public void SetUp()
    {
        expensiveTestResource = new();
        expensiveTestResource2 = new();
    }

    [TearDown]
    public void TearDown()
    {
        expensiveTestResource.Dispose();
        expensiveTestResource2.Dispose();
    }

    [Test]
    public void AddOrUpdate_NewValue_TryGetScopedResource_ReturnsStoredInstance()
    {
        var scopedResources = new DefaultScopedResources();

        scopedResources.AddOrUpdate(expensiveTestResource);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(scopedResources.TryGetScopedResource<MyExpensiveTestResource>(out var value), Is.True);
            Assert.That(value, Is.SameAs(expensiveTestResource));
        }
    }

    [Test]
    public void AddOrUpdate_SameTypeTwice_ReplacesStoredInstance()
    {
        var scopedResources = new DefaultScopedResources();

        scopedResources.AddOrUpdate(expensiveTestResource);
        scopedResources.AddOrUpdate(expensiveTestResource2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(scopedResources.TryGetScopedResource<MyExpensiveTestResource>(out var value), Is.True);
            Assert.That(value, Is.SameAs(expensiveTestResource2));
            Assert.That(value, Is.Not.SameAs(expensiveTestResource));
        }
    }

    [Test]
    public void Contains_WhenTypeExists_ReturnsTrue()
    {
        var scopedResources = new DefaultScopedResources();
        scopedResources.AddOrUpdate(expensiveTestResource);

        Assert.That(scopedResources.Contains<MyExpensiveTestResource>(), Is.True);
    }

    [Test]
    public void Contains_WhenTypeMissing_ReturnsFalse()
    {
        var scopedResources = new DefaultScopedResources();

        Assert.That(scopedResources.Contains<MyExpensiveTestResource>(), Is.False);
    }

    [Test]
    public void GetScopedResource_WhenTypeMissing_ReturnsDefault()
    {
        var scopedResources = new DefaultScopedResources();

        var value = scopedResources.GetScopedResource<MyExpensiveTestResource>();

        Assert.That(value, Is.Null);
    }

    [Test]
    public void TryGetScopedResource_WhenTypeMissing_ReturnsFalseAndNull()
    {
        var scopedResources = new DefaultScopedResources();

        var found = scopedResources.TryGetScopedResource<MyExpensiveTestResource>(out var value);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(found, Is.False);
            Assert.That(value, Is.Null);
        }
    }

    [Test]
    public void AddOrUpdate_DifferentTypes_StoreIndependently()
    {
        var scopedResources = new DefaultScopedResources();
        var other = new AnotherTestResource();

        scopedResources.AddOrUpdate(expensiveTestResource);
        scopedResources.AddOrUpdate(other);

        using (Assert.EnterMultipleScope())
        {       
            Assert.That(scopedResources.TryGetScopedResource<MyExpensiveTestResource>(out var expensive), Is.True);
            Assert.That(scopedResources.TryGetScopedResource<AnotherTestResource>(out var second), Is.True);

            Assert.That(expensive, Is.SameAs(expensiveTestResource));
            Assert.That(second, Is.SameAs(other));
        }
    }
}
