namespace BuildTools.Cli.Operations;

[AttributeUsage(AttributeTargets.Class)]
public sealed class FeatureCommandAttribute(string prefix, string key) : Attribute
{
    public string Prefix { get; } = prefix;
    public string Key { get; } = key;

    public override string ToString()
    {
        return $"{Prefix}-{Key}";
    }
}
