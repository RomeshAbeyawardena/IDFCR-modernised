namespace IDFCR.Abstractions.Cli.Operations;

public interface IInjectableCommandOperationRoot : IInjectableCommandOperation
{
    bool CanExecute(string command);
}
