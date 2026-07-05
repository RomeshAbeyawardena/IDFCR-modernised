using IDFCR.Caching.Http;
using IDFCR.Caching.Http.Extensions;
using IDFCR.Caching.Serialisation.Extensions;
using Moq;
using NUnit.Framework;

namespace IDFCR.Caching.Tests;

[TestFixture]
internal class DistributedGroupCacheExtensionsTests
{
    private Mock<IDistributedGroupCache> cacheMock = null!;

    [SetUp]
    public void SetUp()
    {
        cacheMock = new(MockBehavior.Strict);
    }

    [Test]
    public async Task GetOrSetAsync_WithProvidedItem_WhenCacheHit_ReturnsCachedValueAndSkipsWrite()
    {
        var options = MessagePack.MessagePackSerializerOptions.Standard;
        const string cachedValue = "cached-value";
        const string fallbackValue = "fallback-value";

        cacheMock.Setup(x => x.GetAsync("orders", "orders:1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(await cachedValue.SerialiseAsync(options, CancellationToken.None));

        var result = await cacheMock.Object.GetOrSetAsync(
            "orders",
            "orders:1",
            options,
            fallbackValue,
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(cachedValue));
            cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    [Test]
    public async Task GetOrSetAsync_WithProvidedItem_WhenCacheMiss_WritesSerializedItemAndReturnsItem()
    {
        var options = MessagePack.MessagePackSerializerOptions.Standard;
        const string item = "fresh-value";
        byte[]? writtenBytes = null;

        cacheMock.Setup(x => x.GetAsync("orders", "orders:1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        cacheMock.Setup(x => x.SetAsync("orders", "orders:1", It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, byte[], CancellationToken>((_, _, bytes, _) => writtenBytes = bytes)
            .Returns(Task.CompletedTask);

        var result = await cacheMock.Object.GetOrSetAsync(
            "orders",
            "orders:1",
            options,
            item,
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(item));
            Assert.That(writtenBytes, Is.Not.Null);
            Assert.That(await writtenBytes!.DeserialiseAsync<string>(options, CancellationToken.None), Is.EqualTo(item));
        }
    }

    [Test]
    public async Task GetOrSetAsync_WithNullItem_WhenCacheMiss_ReturnsNullAndSkipsWrite()
    {
        var options = MessagePack.MessagePackSerializerOptions.Standard;
        string? item = null;

        cacheMock.Setup(x => x.GetAsync("orders", "orders:1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        var result = await cacheMock.Object.GetOrSetAsync(
            "orders",
            "orders:1",
            options,
            item,
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Null);
            cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    [Test]
    public async Task GetOrSetAsync_WithFactory_WhenCacheHit_ReturnsCachedValueAndSkipsFactory()
    {
        var options = MessagePack.MessagePackSerializerOptions.Standard;
        const string cachedValue = "cached-value";
        var factory = new Mock<Func<CancellationToken, Task<string>>>(MockBehavior.Strict);

        cacheMock.Setup(x => x.GetAsync("orders", "orders:1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(await cachedValue.SerialiseAsync(options, CancellationToken.None));

        var result = await cacheMock.Object.GetOrSetAsync(
            "orders",
            "orders:1",
            options,
            factory.Object,
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(cachedValue));
            factory.Verify(x => x(It.IsAny<CancellationToken>()), Times.Never);
            cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    [Test]
    public async Task GetOrSetAsync_WithFactory_WhenCacheMiss_WritesFactoryValueAndReturnsIt()
    {
        var options = MessagePack.MessagePackSerializerOptions.Standard;
        const string factoryValue = "factory-value";
        byte[]? writtenBytes = null;
        var factory = new Mock<Func<CancellationToken, Task<string>>>(MockBehavior.Strict);

        cacheMock.Setup(x => x.GetAsync("orders", "orders:1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        cacheMock.Setup(x => x.SetAsync("orders", "orders:1", It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, byte[], CancellationToken>((_, _, bytes, _) => writtenBytes = bytes)
            .Returns(Task.CompletedTask);
        factory.Setup(x => x(It.IsAny<CancellationToken>())).ReturnsAsync(factoryValue);

        var result = await cacheMock.Object.GetOrSetAsync(
            "orders",
            "orders:1",
            options,
            factory.Object,
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(factoryValue));
            Assert.That(writtenBytes, Is.Not.Null);
            Assert.That(await writtenBytes!.DeserialiseAsync<string>(options, CancellationToken.None), Is.EqualTo(factoryValue));
            factory.Verify(x => x(It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Test]
    public async Task GetOrSetAsync_WithFactory_WhenCachePayloadIsEmpty_TreatsAsMiss()
    {
        var options = MessagePack.MessagePackSerializerOptions.Standard;
        const string factoryValue = "factory-value";

        var factory = new Mock<Func<CancellationToken, Task<string>>>(MockBehavior.Strict);

        cacheMock.Setup(x => x.GetAsync("orders", "orders:1", It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        cacheMock.Setup(x => x.SetAsync("orders", "orders:1", It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        factory.Setup(x => x(It.IsAny<CancellationToken>()))
            .ReturnsAsync(factoryValue);

        var result = await cacheMock.Object.GetOrSetAsync(
            "orders",
            "orders:1",
            options,
            factory.Object,
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(factoryValue));
            factory.Verify(x => x(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
