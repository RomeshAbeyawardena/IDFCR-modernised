namespace BuildTools.Cli.Operations;

public interface IOperation
{
    string Name { get; }
    IEnumerable<string> Aliases { get; }
}
