using BuildTools.Cli.Features.Tags;
using BuildTools.Cli.ManagedStreams;
using Moq;

namespace BuildTools.Cli.Tests;

public class TagWriteOperationTests
{
    private TagWriteOperation sut;

    private Mock<IServiceProvider> serviceProviderMock = new();
    private Mock<IManagedStream> managedStreamMock = new();

    [SetUp]
    public void Setup()
    {
        sut = new(serviceProviderMock.Object,);
    }

    [Test]
    public void Test1()
    {

        Assert.Pass();
    }
}
