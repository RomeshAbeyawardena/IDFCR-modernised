using IDFCR.Abstractions.Metadata;
using System.ComponentModel;

namespace IDFCR.Abstractions.Interceptors.Tests;

internal class TestEntity2 : IAuditable
{
    string IAuditable.AuditEntityName => nameof(TestEntity2);

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
