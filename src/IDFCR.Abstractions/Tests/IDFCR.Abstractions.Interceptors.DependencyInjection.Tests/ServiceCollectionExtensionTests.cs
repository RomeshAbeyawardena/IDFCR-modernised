using IDFCR.Abstractions.Interceptors.DependencyInjection.Extensions;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Metadata;
using IDFCR.TestUtilities.Fakes;
using NUnit.Framework;

namespace IDFCR.Abstractions.Interceptors.DependencyInjection.Tests;

[TestFixture]
public class ServiceCollectionExtensionTests
{
    private FakeServiceCollection services;

    [SetUp]
    public void SetUp()
    {
        services = [];
    }

    [Test]
    public void AddInterceptors_RegistersFactoryAndAuditInterceptors()
    {
        services.AddInterceptors();

        Assert.That(services, Has.Count.EqualTo(3));

        var implementationTypes = services.Select(x => x.ImplementationType).ToArray();

        Type[] expectedTypes = [typeof(DefaultEntityInterceptorFactory), 
            typeof(AuditCreatedTimestampEntityInterceptor), 
            typeof(AuditModifiedTimestampEntityInterceptor)];

        foreach (var expectedType in expectedTypes)
        {
            Assert.That(implementationTypes, Contains.Item(expectedType));
        }
    }
}
