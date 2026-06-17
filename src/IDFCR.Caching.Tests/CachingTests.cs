using NUnit.Framework;

namespace IDFCR.Caching.Tests;

#pragma warning disable CS0618

[TestFixture]
internal class DefaultCacheGroupsTests
{
    [Test]
    public void TryAssignToGroup_WhenCacheKeyIsNew_ReturnsTrueAndAddsKey()
    {
        DefaultCacheGroups sut = new();

        var result = sut.TryAssignToGroup("orders", "orders:1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(sut.HasCacheKey("orders", "orders:1"), Is.True);
        }
    }

    [Test]
    public void TryAssignToGroup_WhenCacheKeysAlreadyExist_ReturnsFalse()
    {
        DefaultCacheGroups sut = new();
        sut.TryAssignToGroup("orders", "orders:1");

        var result = sut.TryAssignToGroup("orders", "orders:1");

        Assert.That(result, Is.False);
    }

    [Test]
    public void TryRemoveFromGroup_WhenCacheKeyExists_ReturnsTrueAndRemovesKey()
    {
        DefaultCacheGroups sut = new();
        sut.TryAssignToGroup("orders", "orders:1", "orders:2");

        var result = sut.TryRemoveFromGroup("orders", "orders:2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(sut.HasCacheKey("orders", "orders:2"), Is.False);
            Assert.That(sut.HasCacheKey("orders", "orders:1"), Is.True);
        }
    }

    [Test]
    public void TryRemoveFromGroup_WhenGroupDoesNotExist_ReturnsFalse()
    {
        DefaultCacheGroups sut = new();

        var result = sut.TryRemoveFromGroup("missing", "orders:1");

        Assert.That(result, Is.False);
    }

    [Test]
    public void GetCacheKeys_WhenGroupExists_ReturnsAssignedKeys()
    {
        DefaultCacheGroups sut = new();
        sut.TryAssignToGroup("orders", "orders:1", "orders:2");

        var result = sut.GetCacheKeys("orders").ToArray();

        Assert.That(result, Is.EquivalentTo(new[] { "orders:1", "orders:2" }));
    }

    [Test]
    public void GetCacheKeys_WhenGroupDoesNotExist_ReturnsEmpty()
    {
        DefaultCacheGroups sut = new();

        var result = sut.GetCacheKeys("missing");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetCacheKeys_WhenGroupChangesAfterRead_ReturnsSnapshot()
    {
        DefaultCacheGroups sut = new();
        sut.TryAssignToGroup("orders", "orders:1", "orders:2");

        var snapshot = sut.GetCacheKeys("orders").ToArray();

        sut.TryRemoveFromGroup("orders", "orders:2");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(snapshot, Is.EquivalentTo(new[] { "orders:1", "orders:2" }));
            Assert.That(sut.GetCacheKeys("orders"), Is.EquivalentTo(new[] { "orders:1" }));
        }
    }

    [Test]
    public async Task TryAssignToGroup_WhenCalledConcurrently_DeduplicatesCacheKeys()
    {
        DefaultCacheGroups sut = new();
        var cacheKeys = Enumerable.Range(0, 250)
            .Select(index => $"orders:{index % 25}")
            .ToArray();

        await Task.WhenAll(cacheKeys.Select(cacheKey => Task.Run(() => sut.TryAssignToGroup("orders", cacheKey))));

        Assert.That(sut.CacheGroups["orders"].CacheKeys, Is.EquivalentTo(cacheKeys.Distinct()));
    }

    [Test]
    public async Task TryRemoveFromGroup_WhenCalledConcurrently_RemovesAllKeys()
    {
        DefaultCacheGroups sut = new();
        var cacheKeys = Enumerable.Range(0, 100)
            .Select(index => $"orders:{index}")
            .ToArray();

        sut.TryAssignToGroup("orders", cacheKeys);

        await Task.WhenAll(cacheKeys.Select(cacheKey => Task.Run(() => sut.TryRemoveFromGroup("orders", cacheKey))));

        Assert.That(sut.CacheGroups["orders"].CacheKeys, Is.Empty);
    }
}

#pragma warning restore
