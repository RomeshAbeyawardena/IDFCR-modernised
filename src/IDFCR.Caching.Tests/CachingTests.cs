using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Caching.Distributed;
namespace IDFCR.Caching.Tests;

[TestFixture]
internal class CachingTests
{
#pragma warning disable CS0618
    private Mock<IDistributedCache> cacheMock;
    private DistributedCacheGroups distributedCacheGroups;
    [SetUp]
    public void SetUp()
    {
        cacheMock = new();
        distributedCacheGroups = new(cacheMock.Object, MessagePack.MessagePackSerializerOptions.Standard);
    }

    [Test]
    public async Task Test()
    {
        DefaultCacheGroups cacheGroups = new();

        var result = cacheGroups.TryAssignToGroup("nord.cache", "nord.cache:test");

        Assert.That(result, Is.True);

        cacheMock.Setup(x => x.GetAsync(nameof(DefaultCacheGroups)))
            .ReturnsAsync(await distributedCacheGroups.SerializeAsync(cacheGroups, CancellationToken.None));

        await distributedCacheGroups.LoadAsync(CancellationToken.None);

        Assert.That(distributedCacheGroups.Groups, Is.SameAs(cacheGroups));
    }
}
#pragma warning restore