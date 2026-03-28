namespace BuildTools.Cli.StateManagement;

public interface IState
{
    string Name { get; }
    DateTimeOffset CreatedTimestampUtc { get; }
    DateTimeOffset? UpdatedTimestampUtc { get; }

}
