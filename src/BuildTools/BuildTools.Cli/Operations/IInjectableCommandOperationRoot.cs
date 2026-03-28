namespace BuildTools.Cli.Operations;

public interface IInjectableCommandOperationRoot : IInjectableCommandOperation
{
    bool CanExecute(string command);
}
