using BuildTools.Cli.StateManagement;

namespace BuildTools.Cli.Operations;

public interface ICommandOperation : IOperation
{
    bool ListOperations { set; }
    bool Equals(string? name);
    void Initialise(CommandOperationState? State = null);
    CommandOperationState State { get; }
    IEnumerable<ICommandOperation> SupportedOperations { get; }
    Task InvokeAsync(string command, CancellationToken cancellationToken);
    Task InvokeAsync(IEnumerable<string> commands, CancellationToken cancellationToken);
    void AddSupportedOperation(ICommandOperation operation);
    void AddSupportedOperations(params ICommandOperation[] operations);
}
