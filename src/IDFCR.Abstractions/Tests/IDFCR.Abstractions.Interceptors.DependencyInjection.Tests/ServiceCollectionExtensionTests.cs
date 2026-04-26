using IDFCR.Abstractions.Interceptors.DependencyInjection.Extensions;
using IDFCR.Abstractions.Interceptors.Factories;
using IDFCR.Abstractions.Interceptors.Interceptors;
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

        Assert.That(services, Has.Count.EqualTo(5));

        var factoryService = services.FirstOrDefault(x => x.ImplementationType == typeof(DefaultEntityInterceptorFactory));
        Assert.That(factoryService, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(factoryService.ServiceType, Is.EqualTo(typeof(IEntityInterceptorFactory)));
            Assert.That(factoryService.ImplementationType, Is.EqualTo(typeof(DefaultEntityInterceptorFactory)));
            Assert.That(factoryService.Lifetime, Is.EqualTo(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton));
        }

        var auditCreatedService = services.FirstOrDefault(x => x.ImplementationType == typeof(AuditCreatedTimestampEntityInterceptor));
        Assert.That(auditCreatedService, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(auditCreatedService.ServiceType, Is.EqualTo(typeof(IEntityInterceptor)));
            Assert.That(auditCreatedService.ImplementationType, Is.EqualTo(typeof(AuditCreatedTimestampEntityInterceptor)));
            Assert.That(auditCreatedService.Lifetime, Is.EqualTo(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient));
        }

        var auditModifiedService = services.FirstOrDefault(x => x.ImplementationType == typeof(AuditModifiedTimestampEntityInterceptor));
        Assert.That(auditModifiedService, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(auditModifiedService.ServiceType, Is.EqualTo(typeof(IEntityInterceptor)));
            Assert.That(auditModifiedService.ImplementationType, Is.EqualTo(typeof(AuditModifiedTimestampEntityInterceptor)));
            Assert.That(auditModifiedService.Lifetime, Is.EqualTo(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient));
        }

        var auditChangesService = services.FirstOrDefault(x => x.ImplementationType == typeof(AuditEntityChangesInterceptor));
        Assert.That(auditChangesService, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(auditChangesService.ServiceType, Is.EqualTo(typeof(IEntityInterceptor)));
            Assert.That(auditChangesService.ImplementationType, Is.EqualTo(typeof(AuditEntityChangesInterceptor)));
            Assert.That(auditChangesService.Lifetime, Is.EqualTo(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient));
        }
    }
}
