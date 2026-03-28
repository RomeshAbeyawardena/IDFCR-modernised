namespace BuildTools.Cli.Extensions;

public static class StringExtensions
{
    public static string? Limit(this string? value, int limit)
    {
        if (string.IsNullOrWhiteSpace(value) || limit == 0)
        {
            return value;
        }

        if (limit < 4)
        {
            return "...";
        }

        if (value.Length > limit)
        {
            return string.Concat(value.AsSpan(0, limit - 3), "..."); 
        }

        if (value.Length < limit)
        {
            var len = limit - value.Length;
            var r = string.Concat(value, new string(' ', len));
            return r;
        }

        return value;
    }
}
