using IDFCR.Abstractions.Metadata;

namespace IDFCR.Abstractions.Interceptors.Tests;

internal class TestEntity : IAuditable
{
    string IAuditable.AuditEntityName => nameof(TestEntity);
    public required string Name { get; set; }
    public string? DisplayName { get; set; }
    public string? UnitName { get; set; }
    public string? DepartmentName { get; set; }
    public string? DoesNotChange { get; set; }
}
