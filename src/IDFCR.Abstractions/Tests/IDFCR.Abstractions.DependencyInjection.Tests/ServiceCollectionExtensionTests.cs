using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using IDFCR.Abstractions.DependencyInjection.Extensions;
using IDFCR.TestUtilities.Fakes;
using IDFCR.Abstractions.DependencyInjection.Tests.Assets;

namespace IDFCR.Abstractions.DependencyInjection.Tests;

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
    public void AddGenericServices_FindsAndRegistersTwoServiceImplementations()
    {
        services.AddGenericServices<IService>(ServiceLifetime.Singleton, typeof(ServiceCollectionExtensionTests).Assembly);

        Assert.That(services, Has.Count.EqualTo(2));

        var firstService = services.FirstOrDefault();
        Assert.That(firstService, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(firstService.ServiceType, Is.EqualTo(typeof(IService)));
            Assert.That(firstService.ImplementationType, Is.EqualTo(typeof(MyService)));
            Assert.That(firstService.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
        }

        var secondService = services.ElementAtOrDefault(1);
        Assert.That(secondService, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(secondService.ServiceType, Is.EqualTo(typeof(IService)));
            Assert.That(secondService.ImplementationType, Is.EqualTo(typeof(MyService2)));
            Assert.That(secondService.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
        }
    }

}
