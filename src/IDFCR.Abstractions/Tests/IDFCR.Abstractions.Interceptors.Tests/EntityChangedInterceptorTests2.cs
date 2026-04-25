
using IDFCR.Abstractions.Interceptors.Interceptors;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Results;
using NUnit.Framework;
using System.ComponentModel;

namespace IDFCR.Abstractions.Interceptors.Tests;

internal class TestEntity3 : IAuditable
{
    string IAuditable.AuditEntityName => nameof(TestEntity2);

    [DeferredLookup("Lookup")]
    public Guid LookupId { get; set; }

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

internal class TestEntityAuditProcessor3(ICollection<TestEntityAudit> testEntityAuditEntries) : TestEntityAuditProcessorBase<TestEntity3>(nameof(TestEntity3), testEntityAuditEntries)
{
    public override Task<object?> LookupAsync(string key, object value, CancellationToken cancellationToken)
    {
        return key switch
        {
            "Lookup" => Task.FromResult<object?>("Lookup result"),
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

    [SetUp]
    public void SetUp()
    {
        _testEntityAuditEntries = [];
        _auditProcessorProvider = new([new TestEntityAuditProcessor(_testEntityAuditEntries), new TestEntityAuditProcessor2(_testEntityAuditEntries)]);
        _entityInterceptorList = [new AuditEntityChangesInterceptor(_auditProcessorProvider)];
        _entityInterceptorFactory = new(_entityInterceptorList);
    }

    [Test]
    public void Test1()
    {

    }
}
