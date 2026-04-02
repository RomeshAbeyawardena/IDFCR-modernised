namespace IDFCR.Abstractions.Cli.Operations;

public abstract class InjectableCommandOperationRootBase<T>(IServiceProvider serviceProvider, string prefix, string name, Type? memberOfType, params string[] aliases)
    : InjectableCommandOperationBase<T>(serviceProvider, prefix, name, memberOfType, aliases),
    IInjectableCommandOperationRoot
    where T : IInjectableCommandOperation
{
    public virtual bool CanExecute(string command)
    {
        return Equals(GetTokens(command).FirstOrDefault());
    }
}
