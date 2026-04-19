using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace IDFCR.Abstractions.GRPC.Tests;

[RegisteredGRPCServiceImplementation(true)]
public class MyGRPCService { }

[RegisteredGRPCServiceImplementation(true)]
public class MyOtherGRPCService { }

[RegisteredGRPCServiceImplementation(true)]
public class MyFinalOtherGRPCService { }

[RegisteredGRPCServiceImplementation(false)]
public class MyGRPCServiceNotInUse { }

[RegisteredGRPCServiceImplementation(true, "GRPC:MyGRPCServiceDependentOnConfiguration:Enabled")]
public class MyGRPCServiceDependentOnConfiguration { }

[TestFixture]
internal class DiscovererTests
{
    private const string ConfigurationKey = "GRPC:MyGRPCServiceDependentOnConfiguration:Enabled";

    private static readonly Type[] BaseExpectedTypes =
    [
        typeof(MyGRPCService),
        typeof(MyOtherGRPCService),
        typeof(MyFinalOtherGRPCService),
        typeof(MyGRPCServiceDependentOnConfiguration)
    ];

    private Mock<IConfiguration> configuration;

    [SetUp]
    public void SetUp()
    {
        configuration = new();
    }

    [TearDown]
    public void TearDown()
    {
        RegisteredGRPCServices.FlushCache();
    }

    private void SetupConfigurationKey(string value)
    {
        var sectionMock = new Mock<IConfigurationSection>();
        sectionMock.SetupGet(x => x.Value).Returns(value);
        configuration.Setup(x => x.GetSection(ConfigurationKey)).Returns(sectionMock.Object);
    }

    [Test]
    public void DiscoverTypes_BeforeDiscovery_DiscoveredTypesIsEmpty()
    {
        Assert.That(RegisteredGRPCServices.DiscoveredTypes, Is.Empty);
    }

    [Test]
    public void DiscoverTypes_ReturnsOnlyEnabledServices()
    {
        var discoveredTypes = RegisteredGRPCServices
            .DiscoverTypes(configuration.Object, typeof(DiscovererTests).Assembly);

        Assert.That(discoveredTypes, Is.EquivalentTo(BaseExpectedTypes));
    }

    [Test]
    public void DiscoverTypes_ExcludesDisabledServices()
    {
        var discoveredTypes = RegisteredGRPCServices
            .DiscoverTypes(configuration.Object, typeof(DiscovererTests).Assembly);

        Assert.That(discoveredTypes, Does.Not.Contain(typeof(MyGRPCServiceNotInUse)));
    }

    [Test]
    public void DiscoverTypes_PopulatesDiscoveredTypesCache()
    {
        var discoveredTypes = RegisteredGRPCServices
            .DiscoverTypes(configuration.Object, typeof(DiscovererTests).Assembly);

        Assert.That(RegisteredGRPCServices.DiscoveredTypes, Is.EquivalentTo(discoveredTypes));
    }

    [Test]
    public void DiscoverTypes_WhenConfigurationKeyIsTrue_IncludesConfigurationDependentService()
    {
        SetupConfigurationKey("true");

        var discoveredTypes = RegisteredGRPCServices
            .DiscoverTypes(configuration.Object, typeof(DiscovererTests).Assembly);

        Assert.That(discoveredTypes, Contains.Item(typeof(MyGRPCServiceDependentOnConfiguration)));
    }

    [Test]
    public void DiscoverTypes_WhenConfigurationKeyIsFalse_ExcludesConfigurationDependentService()
    {
        SetupConfigurationKey("false");

        var discoveredTypes = RegisteredGRPCServices
            .DiscoverTypes(configuration.Object, typeof(DiscovererTests).Assembly);

        Assert.That(discoveredTypes, Does.Not.Contain(typeof(MyGRPCServiceDependentOnConfiguration)));
    }
}
