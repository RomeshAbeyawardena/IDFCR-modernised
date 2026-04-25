namespace IDFCR.Abstractions.Interceptors.Tests;

internal class TestEntityAudit
{
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? ChangeDescription { get; set; }
}
