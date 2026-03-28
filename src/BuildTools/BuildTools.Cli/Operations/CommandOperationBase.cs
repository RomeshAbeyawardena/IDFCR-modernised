using BuildTools.Cli.StateManagement;
using System.Text.RegularExpressions;

namespace BuildTools.Cli.Operations;

public abstract class CommandOperationBase(string name, params string[] aliases) : ICommandOperation
{
    IEnumerable<ICommandOperation> ICommandOperation.SupportedOperations => _operations;

    private readonly List<ICommandOperation> _operations = [];
    private CommandOperationState _state = new();
    private bool _isInitialised = false;

    protected internal virtual IDictionary<string, Parameter>? Parameters { get; private set; }

    protected virtual IEnumerable<string> GetTokens(string command)
    {
        return command.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    protected virtual IEnumerable<ICommandOperation> GetOperations(IEnumerable<string> commands)
    {
        Parameters ??= ArgumentSplitter.Split(commands);
        return _operations;
    }
    protected abstract Task OnInvokeAsync(IEnumerable<string> command, CancellationToken cancellationToken);

    public string Name => name;
    public IEnumerable<string> Aliases => aliases;

    public bool ListOperations { protected get; set; }

    public CommandOperationState State { 
        get => _state; 
        set { 
            if (!_isInitialised)
            {
                _state = value;
                _isInitialised = true;
            }
            else
            {
                throw new InvalidOperationException("Cannot set State after initialisation.");
            }
        } 
    }

    public bool Equals(string? name)
    {
        return Name
            .Equals(name, StringComparison.OrdinalIgnoreCase)
            || Aliases.Contains(name, StringComparer.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return obj is CommandOperationBase other &&
               Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
               Aliases.SequenceEqual(other.Aliases, StringComparer.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Aliases);
    }

    public virtual async Task InvokeAsync(IEnumerable<string> commands, CancellationToken cancellationToken)
    {
        var operations = GetOperations(commands);
        if (Equals(commands.FirstOrDefault()))
        {
            await OnInvokeAsync(commands.Skip(1), cancellationToken);
        }
        else
        {
            //fix for the wrong root does nothing
            return;
        }

        if (commands.Count() > 1)
        {
            var currentOperation = operations
                .FirstOrDefault(x => x.Equals(commands.ElementAtOrDefault(1)));

            if (currentOperation is null)
            {
                return;
            }

            var nextElements = commands.Skip(1);

            await currentOperation.InvokeAsync(nextElements, cancellationToken);
        }
    }

    public virtual Task InvokeAsync(string command, CancellationToken cancellationToken)
    {
        return InvokeAsync(GetTokens(command), cancellationToken);
    }

    public void AddSupportedOperations(params ICommandOperation[] operations)
    {
        foreach (var operation in operations)
        {
            AddSupportedOperation(operation);
        }
    }

    public void AddSupportedOperation(ICommandOperation operation)
    {
        _operations.Add(operation);
    }

    public void Initialise(CommandOperationState? state = null)
    {
        if (_isInitialised)
            throw new InvalidOperationException("Already initialised.");

        if (state is not null)
        {
            _state = state;
        }

        foreach (var operation in _operations)
        {
            operation.Initialise(_state);
        }

        _isInitialised = true;
    }
}