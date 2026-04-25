using IDFCR.Abstractions.Builders;
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Metadata;
using NUnit.Framework;
using System.ComponentModel;

namespace IDFCR.Abstractions.Interceptors.Tests;

internal class TestEntity3 : IAuditable
{
    string IAuditable.AuditEntityName => nameof(TestEntity3);

    [DeferredLookup("Lookup")]
    public Guid LookupId { get; set; }

    [DeferredLookup("Lookup")]
    public Guid AnotherLookupId { get; set; }

    public required string Name { get; set; }
    [DisplayName("Display name")]
    public string? DisplayName { get; set; }
    [DisplayName("Unit name")]
    public string? UnitName { get; set; }
    [DisplayName("Department name")]
    public string? DepartmentName { get; set; }
    public string? DoesNotChange { get; set; }

    [IgnoreAuditing]
    public bool Ignored { get; set; }
}

internal class TestEntityAuditProcessor3 : TestEntityAuditProcessorBase<TestEntity3>
{
    internal readonly Guid OldLookupId = Guid.NewGuid();
    internal readonly Guid NewLookupId = Guid.NewGuid();

    private readonly Lazy<Dictionary<Guid, string>> _fakeLookups;

    public TestEntityAuditProcessor3(ICollection<TestEntityAudit> testEntityAuditEntries) : base(nameof(TestEntity3), testEntityAuditEntries)
    {
        _fakeLookups = new(DictionaryBuilder.Create<Guid, string>(b => b
            .AddOrUpdate(OldLookupId, "Old look up result")
            .AddOrUpdate(NewLookupId, "New look up result")
            ).Build());
    }

    public override Task<object?> LookupAsync(string key, object value, CancellationToken cancellationToken)
    {
        return key switch
        {
            "Lookup" => value is Guid id && _fakeLookups.Value.TryGetValue(id, out var result) 
                ? Task.FromResult<object?>(result) 
                : Task.FromResult<object?>(null),
            _ => base.LookupAsync(key, value, cancellationToken),
        };
    }
}


[TestFixture]
internal class EntityChangedInterceptorTests2
{
    private DefaultEntityInterceptorFactory _entityInterceptorFactory;
    private DefaultAuditProcessorProvider _auditProcessorProvider;
    private List<IEntityInterceptor> _entityInterceptorList;
    private List<TestEntityAudit> _testEntityAuditEntries;
    private TestEntityAuditProcessor3 _testEntityAuditProcessor3;


    [SetUp]
    public void SetUp()
    {
        _testEntityAuditEntries = [];
        _testEntityAuditProcessor3 = new(_testEntityAuditEntries);

        _auditProcessorProvider = new([new TestEntityAuditProcessor(_testEntityAuditEntries), new TestEntityAuditProcessor2(_testEntityAuditEntries), _testEntityAuditProcessor3]);
        _entityInterceptorList = [new AuditEntityChangesInterceptor(_auditProcessorProvider)];
        _entityInterceptorFactory = new(_entityInterceptorList);
    }

    [Test]
    public async Task Test_that_deferred_lookups_resolve_correctly()
    {
        var oldLookupId = _testEntityAuditProcessor3.OldLookupId;
        var newLookupId = _testEntityAuditProcessor3.NewLookupId;

        TestEntity3 oldSubject = new()
        {
            Name = "Test",
            LookupId = oldLookupId,
            AnotherLookupId = newLookupId,
            DepartmentName = "Department",
            DisplayName = "Display",
            UnitName = "Unit",
            DoesNotChange = "Static value"
        };

        TestEntity3 subject = new()
        {
            Name = "Test",
            LookupId = newLookupId,
            AnotherLookupId = Guid.NewGuid(),
            DepartmentName = "Department",
            DisplayName = "Display",
            UnitName = "Unit",
            DoesNotChange = "Static value"
        };

        var context = new Assets.TestEntityInterceptContext(
            EntityContextBehaviorStage.Post,
            EntityContextBehavior.Update,
            subject);

        context.Dictionary.Add(AuditEntityChangesInterceptor.OldDataKey, oldSubject);

        var interceptors = await _entityInterceptorFactory
            .GetEntityInterceptorsAsync(context, CancellationToken.None);

        await _entityInterceptorFactory.InvokeAsync(interceptors, context, CancellationToken.None);

        Assert.That(_testEntityAuditEntries, Has.Count.EqualTo(1));

        var auditEntry = _testEntityAuditEntries[0];
        Assert.That(auditEntry.ChangeDescription, Does.Contain("LookupId changed from 'Old look up result' to 'New look up result'."));
        Assert.That(auditEntry.ChangeDescription, Does.Not.Contain(oldLookupId.ToString()));
        Assert.That(auditEntry.ChangeDescription, Does.Not.Contain(newLookupId.ToString()));
    }
}
