using IDFCR.Abstractions.Caching;
using IDFCR.Caching.Http;
using IDFCR.Caching.Http.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace IDFCR.Caching.Tests;

#pragma warning disable CS0618
[TestFixture]
internal class DistributedCacheGroupsTests
{
    private Mock<IDistributedCache> cacheMock = null!;
    private DistributedCacheGroups sut = null!;

    [SetUp]
    public void SetUp()
    {
        cacheMock = new();
        sut = new(cacheMock.Object, MessagePack.MessagePackSerializerOptions.Standard);
    }

    [Test]
    public async Task LoadAsync_WhenGroupsPayloadExists_PopulatesGroups()
    {
        DefaultCacheGroups cacheGroups = new();
        cacheGroups.TryAssignToGroup("orders", "orders:1", "orders:2");

        cacheMock.Setup(x => x.GetAsync(nameof(DefaultCacheGroups), It.IsAny<CancellationToken>()))
            .ReturnsAsync(await sut.SerializeAsync(cacheGroups, CancellationToken.None));

        await sut.LoadAsync(CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.Groups.Keys, Is.EquivalentTo(cacheGroups.CacheGroups.Keys));
            Assert.That(sut.Groups["orders"].CacheKeys, Is.EquivalentTo(cacheGroups.CacheGroups["orders"].CacheKeys));
        }
    }

    [Test]
    public async Task LoadAsync_WhenPayloadIsMissing_LeavesGroupsUnchanged()
    {
        cacheMock.Setup(x => x.GetAsync(nameof(DefaultCacheGroups), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        await sut.LoadAsync(CancellationToken.None);

        Assert.That(sut.Groups.Keys, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenCompositeKeyIsNotAssigned_ReturnsNullWithoutCallingCache()
    {
        var result = await sut.GetAsync("orders", "orders:1", CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Null);
            cacheMock.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    [Test]
    public async Task GetAsync_WhenCompositeKeyIsAssigned_UsesFormattedKey()
    {
        var expected = new byte[] { 1, 2, 3 };
        sut.Groups.TryAssignToGroup("orders", "orders:1");

        cacheMock.Setup(x => x.GetAsync("orders::orders:1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await sut.GetAsync("orders", "orders:1", static (group, key) => $"{group}::{key}", CancellationToken.None);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task SetAsync_WhenCacheWriteSucceeds_AssignsGroupAndWritesData()
    {
        var payload = new byte[] { 7, 8, 9 };

        cacheMock.Setup(x => x.SetAsync(
                "orders::orders:1",
                payload,
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await sut.SetAsync("orders", "orders:1", static (group, key) => $"{group}::{key}", payload, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.Groups.HasCacheKey("orders", "orders:1"), Is.True);
            cacheMock.Verify(x => x.SetAsync(
                "orders::orders:1",
                payload,
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Test]
    public async Task SetAsync_WhenCacheWriteThrows_RollsBackGroupAssignment()
    {
        var payload = new byte[] { 7, 8, 9 };

        cacheMock.Setup(x => x.SetAsync(
                "orders::orders:1",
                payload,
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        await sut.SetAsync("orders", "orders:1", static (group, key) => $"{group}::{key}", payload, CancellationToken.None);

        Assert.That(sut.Groups.HasCacheKey("orders", "orders:1"), Is.False);
    }

    [Test]
    public async Task SaveAsync_PersistsSerializedGroupsSnapshot()
    {
        sut.Groups.TryAssignToGroup("orders", "orders:1", "orders:2");
        byte[]? persistedPayload = null;

        cacheMock.Setup(x => x.SetAsync(
                nameof(DefaultCacheGroups),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((_, bytes, _, _) => persistedPayload = bytes)
            .Returns(Task.CompletedTask);

        await sut.SaveAsync(CancellationToken.None);

        Assert.That(persistedPayload, Is.Not.Null);
        Assert.That(persistedPayload!.Length, Is.GreaterThan(0));

        var rehydrated = await sut.DeserializeAsync(persistedPayload, CancellationToken.None);

        Assert.That(rehydrated.HasCacheKey("orders", "orders:1"), Is.True);
    }

    [Test]
    public async Task GetCacheKeysAsync_WhenGroupExists_ReturnsAssignedKeys()
    {
        sut.Groups.TryAssignToGroup("orders", "orders:1", "orders:2");

        var result = await sut.GetCacheKeysAsync("orders", CancellationToken.None);

        Assert.That(result, Is.EquivalentTo(new[] { "orders:1", "orders:2" }));
    }

    [Test]
    public async Task GetCacheKeysAsync_WhenGroupDoesNotExist_ReturnsEmpty()
    {
        var result = await sut.GetCacheKeysAsync("missing", CancellationToken.None);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetCacheKeysAsync_AfterSetAsync_ReturnsTrackedKeys()
    {
        var payload = new byte[] { 1, 2, 3 };

        cacheMock.Setup(x => x.SetAsync(
                It.IsAny<string>(),
                payload,
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await sut.SetAsync("orders", "orders:1", payload, CancellationToken.None);
        await sut.SetAsync("orders", "orders:2", payload, CancellationToken.None);

        var result = await sut.GetCacheKeysAsync("orders", CancellationToken.None);

        Assert.That(result, Is.EquivalentTo(new[] { "orders:1", "orders:2" }));
    }

    [Test]
    public async Task RemoveAsync_WhenGroupExists_RemovesAllCacheEntriesAndGroup()
    {
        var payload = new byte[] { 5, 6, 7 };
        cacheMock.Setup(x => x.SetAsync(
                It.IsAny<string>(),
                payload,
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await sut.SetAsync("orders", "orders:1", payload, CancellationToken.None);
        await sut.SetAsync("orders", "orders:2", payload, CancellationToken.None);

        var result = await sut.RemoveAsync("orders", CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(sut.Groups.HasCacheKey("orders", "orders:1"), Is.False);
            Assert.That(sut.Groups.HasCacheKey("orders", "orders:2"), Is.False);
            Assert.That(sut.Groups["orders"].CacheKeys, Is.Empty);
            cacheMock.Verify(x => x.Remove("orders:1"), Times.Once);
            cacheMock.Verify(x => x.Remove("orders:2"), Times.Once);
        }
    }

    [Test]
    public async Task RemoveAsync_WhenGroupDoesNotExist_ReturnsFalseWithoutRemovingEntries()
    {
        var result = await sut.RemoveAsync("missing", CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.False);
            cacheMock.Verify(x => x.Remove(It.IsAny<string>()), Times.Never);
        }
    }

    [Test]
    public async Task SetAsync_WhenCalledConcurrentlyForSameKey_WritesToDistributedCacheOnce()
    {
        var payload = new byte[] { 7, 8, 9 };

        cacheMock.Setup(x => x.SetAsync(
                "orders::orders:1",
                payload,
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var writers = Enumerable.Range(0, 50)
            .Select(_ => sut.SetAsync("orders", "orders:1", static (group, key) => $"{group}::{key}", payload, CancellationToken.None));

        await Task.WhenAll(writers);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.Groups.HasCacheKey("orders", "orders:1"), Is.True);
            cacheMock.Verify(x => x.SetAsync(
                "orders::orders:1",
                payload,
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Test]
    public async Task SetAsync_WhenCalledConcurrentlyForDifferentKeys_WritesAllAndTracksAll()
    {
        var payload = new byte[] { 1, 2, 3 };
        var compositeKeys = Enumerable.Range(0, 40)
            .Select(index => $"orders:{index}")
            .ToArray();

        cacheMock.Setup(x => x.SetAsync(
                It.IsAny<string>(),
                payload,
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await Task.WhenAll(compositeKeys.Select(compositeKey =>
            sut.SetAsync("orders", compositeKey, static (group, key) => $"{group}::{key}", payload, CancellationToken.None)));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.Groups["orders"].CacheKeys, Is.EquivalentTo(compositeKeys));
            cacheMock.Verify(x => x.SetAsync(
                It.IsAny<string>(),
                payload,
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Exactly(compositeKeys.Length));
        }
    }
}

[TestFixture]
internal class DefaultDistributedGroupCacheTests
{
    [Test]
    public async Task RemoveAsync_LoadsThenRemovesAndReturnsTrue()
    {
        var distributedCacheGroupsMock = new Mock<IDistributedCacheGroups>();
        using var serviceProvider = new ServiceCollection()
            .AddGroupedDistributedCache()
            .AddSingleton(distributedCacheGroupsMock.Object)
            .BuildServiceProvider();

        var sut = serviceProvider.GetRequiredService<IDistributedGroupCache>();

        var sequence = new MockSequence();
        distributedCacheGroupsMock.InSequence(sequence)
            .Setup(x => x.LoadAsync(CancellationToken.None))
            .Returns(Task.CompletedTask);
        distributedCacheGroupsMock.InSequence(sequence)
            .Setup(x => x.RemoveAsync("orders", CancellationToken.None))
            .ReturnsAsync(true);

        var result = await sut.RemoveAsync("orders", CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            distributedCacheGroupsMock.Verify(x => x.LoadAsync(CancellationToken.None), Times.Once);
            distributedCacheGroupsMock.Verify(x => x.RemoveAsync("orders", CancellationToken.None), Times.Once);
        }
    }

    [Test]
    public async Task RemoveAsync_WhenInnerRemoveReturnsFalse_ReturnsFalse()
    {
        var distributedCacheGroupsMock = new Mock<IDistributedCacheGroups>();
        using var serviceProvider = new ServiceCollection()
            .AddGroupedDistributedCache()
            .AddSingleton(distributedCacheGroupsMock.Object)
            .BuildServiceProvider();

        var sut = serviceProvider.GetRequiredService<IDistributedGroupCache>();

        distributedCacheGroupsMock.Setup(x => x.LoadAsync(CancellationToken.None))
            .Returns(Task.CompletedTask);
        distributedCacheGroupsMock.Setup(x => x.RemoveAsync("missing", CancellationToken.None))
            .ReturnsAsync(false);

        var result = await sut.RemoveAsync("missing", CancellationToken.None);

        Assert.That(result, Is.False);
    }
}

#pragma warning restore
