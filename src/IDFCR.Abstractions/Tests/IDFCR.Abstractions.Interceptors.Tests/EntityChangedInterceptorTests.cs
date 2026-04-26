using IDFCR.Abstractions.Interceptors.Factories;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Interceptors.Providers;
using IDFCR.Abstractions.Interceptors.Tests.Assets;
using NUnit.Framework;

namespace IDFCR.Abstractions.Interceptors.Tests;

internal class TestEntityAuditProcessor(ICollection<TestEntityAudit> testEntityAuditEntries) : TestEntityAuditProcessorBase<TestEntity>(nameof(TestEntity), testEntityAuditEntries);
internal class TestEntityAuditProcessor2(ICollection<TestEntityAudit> testEntityAuditEntries) : TestEntityAuditProcessorBase<TestEntity2>(nameof(TestEntity2), testEntityAuditEntries);

[TestFixture]
internal class EntityChangedInterceptorTests
{
    private DefaultEntityInterceptorFactory _entityInterceptorFactory;
    private DefaultAuditProcessorProvider _auditProcessorProvider;
    private List<IEntityInterceptor> _entityInterceptorList;
    private List<TestEntityAudit> _testEntityAuditEntries;

    [SetUp]
    public void SetUp()
    {
        _testEntityAuditEntries = [];
        _auditProcessorProvider = new([new TestEntityAuditProcessor(_testEntityAuditEntries), new TestEntityAuditProcessor2(_testEntityAuditEntries)]);
        _entityInterceptorList = [new AuditEntityChangesInterceptor(_auditProcessorProvider)];
        _entityInterceptorFactory = new(_entityInterceptorList);
    }

    [Test]
    public async Task AuditEntityChangesInterceptor_CapturesChangesAsync()
    {
        TestEntity oldSubject = new()
        {
            Name = "Test",
            DepartmentName = "Test department",
            DisplayName = "Test entity",
            UnitName = "Test Unit",
            DoesNotChange = "Does not change"
        };

        TestEntity subject = new()
        {
            Name = "Test",
            DepartmentName = "Another test department",
            DisplayName = "Testy",
            UnitName = "Another test unit",
            DoesNotChange = "Does not change"
        };
        
        var context = new TestEntityInterceptContext(EntityContextBehaviorStage.Post, EntityContextBehavior.Update, subject);
        context.Dictionary.Add(AuditEntityChangesInterceptor.OldDataKey, oldSubject);
        
        var interceptors = await _entityInterceptorFactory
            .GetEntityInterceptorsAsync(context, CancellationToken.None);

        await _entityInterceptorFactory.InvokeAsync(interceptors, context, CancellationToken.None);

        // Verify audit entry was created
        Assert.That(_testEntityAuditEntries, Has.Count.EqualTo(1));
        
        var auditEntry = _testEntityAuditEntries[0];
        
        // Verify old and new values are captured
        Assert.That(auditEntry.OldValue, Is.Not.Null.And.Not.Empty);
        Assert.That(auditEntry.NewValue, Is.Not.Null.And.Not.Empty);
        
        // Verify change descriptions include the changed fields
        Assert.That(auditEntry.ChangeDescription, Does.Contain("UnitName changed"));
        Assert.That(auditEntry.ChangeDescription, Does.Contain("DepartmentName changed"));
        Assert.That(auditEntry.ChangeDescription, Does.Contain("DisplayName changed"));
        Assert.That(auditEntry.ChangeDescription, Does.Not.Contain("DoesNotChange changed"));
    }

    [Test]
    public async Task AuditEntityChangesInterceptor_CapturesChangesAsync2()
    {
        TestEntity2 oldSubject = new()
        {
            Name = "Test",
            DepartmentName = "Test department",
            DisplayName = "Test entity",
            UnitName = "Test Unit",
            DoesNotChange = "Does not change",
            Ignored = true
        };

        TestEntity2 subject = new()
        {
            Name = "Test",
            DepartmentName = "Another test department",
            DisplayName = "Testy",
            UnitName = "Another test unit",
            DoesNotChange = "Does not change",
            Ignored = false
        };

        var context = new TestEntityInterceptContext(EntityContextBehaviorStage.Post, EntityContextBehavior.Update, subject);
        context.Dictionary.Add(AuditEntityChangesInterceptor.OldDataKey, oldSubject);

        var interceptors = await _entityInterceptorFactory
            .GetEntityInterceptorsAsync(context, CancellationToken.None);

        await _entityInterceptorFactory.InvokeAsync(interceptors, context, CancellationToken.None);

        // Verify audit entry was created
        Assert.That(_testEntityAuditEntries, Has.Count.EqualTo(1));

        var auditEntry = _testEntityAuditEntries[0];

        // Verify old and new values are captured
        Assert.That(auditEntry.OldValue, Is.Not.Null.And.Not.Empty);
        Assert.That(auditEntry.NewValue, Is.Not.Null.And.Not.Empty);

        // Verify change descriptions include the changed fields
        Assert.That(auditEntry.ChangeDescription, Does.Contain("Unit name changed"));
        Assert.That(auditEntry.ChangeDescription, Does.Contain("Department name changed"));
        Assert.That(auditEntry.ChangeDescription, Does.Contain("Display name changed"));
        Assert.That(auditEntry.ChangeDescription, Does.Not.Contain("DoesNotChange changed"));
        Assert.That(auditEntry.ChangeDescription, Does.Not.Contain("Ignored changed"));
    }
}
