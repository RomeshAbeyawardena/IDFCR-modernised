using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Interceptors.Tests.Assets;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Results;
using NUnit.Framework;
using System.Text;
using System.Text.Json;

namespace IDFCR.Abstractions.Interceptors.Tests;

internal class TestEntity : IAuditable
{
    string IAuditable.AuditEntityName => nameof(TestEntity);
    public required string Name { get; set; }
    public string? DisplayName { get; set; }
    public string? UnitName { get; set; }
    public string? DepartmentName { get; set; }
}

internal class TestEntityAudit
{
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? ChangeDescription { get; set; }
}

internal class TestEntityAuditProcessor(ICollection<TestEntityAudit> testEntityAuditEntries) : AuditProcessorBase<TestEntity, TestEntityAudit>(nameof(TestEntity))
{
    public override async Task<IUnitResult> AuditChangesAsync(TestEntity oldValue, TestEntity newValue, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        TestEntityAudit testAuditEntity = new()
        {
            OldValue = JsonSerializer.Serialize(oldValue),
            NewValue = JsonSerializer.Serialize(newValue)
        };

        StringBuilder changeDescription = new();
        if (!StringComparer.Ordinal.Equals(oldValue.UnitName, newValue.UnitName))
        {
            changeDescription.AppendLine($"Unit name changed from '{oldValue.UnitName}' to {newValue.UnitName}");
        }

        if (!StringComparer.Ordinal.Equals(oldValue.DisplayName, newValue.DisplayName))
        {
            changeDescription.AppendLine($"Display name changed from '{oldValue.DisplayName}' to {newValue.DisplayName}");
        }

        if (!StringComparer.Ordinal.Equals(oldValue.DepartmentName, newValue.DepartmentName))
        {
            changeDescription.AppendLine($"Department name changed from '{oldValue.DepartmentName}' to {newValue.DepartmentName}");
        }

        testAuditEntity.ChangeDescription = changeDescription.ToString();

        testEntityAuditEntries.Add(testAuditEntity);
        return UnitResult.Success(UnitAction.Add);

    }
}

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
        _auditProcessorProvider = new([new TestEntityAuditProcessor(_testEntityAuditEntries)]);
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
            UnitName = "Test Unit"
        };

        TestEntity subject = new()
        {
            Name = "Test",
            DepartmentName = "Another test department",
            DisplayName = "Testy",
            UnitName = "Another test unit"
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
    }
}
