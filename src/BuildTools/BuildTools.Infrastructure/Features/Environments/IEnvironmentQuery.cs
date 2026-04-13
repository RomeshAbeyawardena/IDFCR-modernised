namespace BuildTools.Infrastructure.Features.Environments;

public interface IEnvironmentQuery
{
    string? Name { get; }
    string? NameContains { get; }
    string? ExternalReference { get; }
}
