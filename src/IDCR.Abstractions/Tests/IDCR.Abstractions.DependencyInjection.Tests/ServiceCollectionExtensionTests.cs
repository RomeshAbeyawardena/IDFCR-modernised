using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using IDCR.Abstractions.DependencyInjection.Extensions;
using IDCR.Abstractions.DependencyInjection.Tests.Fakes;
using IDCR.Abstractions.DependencyInjection.Tests.Assets;

namespace IDCR.Abstractions.DependencyInjection.Tests;

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
    public void AddGenericServices()
    {
        services.AddGenericServices<IService>(ServiceLifetime.Singleton, typeof(ServiceCollectionExtensionTests).Assembly);

        Assert.That(services, Has.Count.EqualTo(2));
        var firstService = services.FirstOrDefault();
        Assert.That(firstService, Is.Not.Null);
        Assert.That(firstService.ImplementationType, Is.EqualTo(typeof(MyService)));

        var secondService = services.ElementAtOrDefault(1);
        Assert.That(secondService, Is.Not.Null);
        Assert.That(secondService.ImplementationType, Is.EqualTo(typeof(MyService2)));
    }

}
