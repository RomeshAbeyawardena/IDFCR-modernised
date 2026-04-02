namespace IDFCR.Abstractions.Cli.Extensions;

public static class Formatters
{
    public static Func<object?, string?>? FormatDate(string defaultPlaceholder, string invalidPlaceholder = "Invalid format")
    {
        return rd => rd is null
                            ? defaultPlaceholder
                            : rd is DateTimeOffset offset 
                            ? offset.DateTime.ToLocalTime().ToShortDateString()
                            : rd is DateTime date
                                ? date.ToLocalTime().ToShortDateString()
                                    : invalidPlaceholder;
    }
}
