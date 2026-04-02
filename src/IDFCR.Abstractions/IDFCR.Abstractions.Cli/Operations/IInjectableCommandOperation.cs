namespace IDFCR.Abstractions.Cli.Operations;

public interface IInjectableCommandOperation : ICommandOperation
{
    IEnumerable<IInjectableCommandOperation> CachedOperations { set; }
    IServiceProvider Services { get; }
    Type? MemberOfType { get; }
    string QualifiedName { get; }
}
