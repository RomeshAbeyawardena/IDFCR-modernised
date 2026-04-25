namespace BuildTools.Shared.Features.Audits;

public interface IJsonAudit
{
    string? ChangeDescription { get; }
    string? OldValueJson { get; }
    string? NewValueJson { get; }
}
