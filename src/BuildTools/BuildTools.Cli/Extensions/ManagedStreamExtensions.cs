using BuildTools.Cli.ManagedStreams;
using IDFCR.Abstractions.Results;
using System.Text;

namespace BuildTools.Cli.Extensions;

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

public static class ManagedStreamExtensions
{
    public static async Task<string?> PromptAsync(this IManagedStream stream, string prompt, CancellationToken cancellationToken)
    {
        await stream.Out.WriteAsync($"{prompt}: ", cancellationToken);

        return await stream.In.ReadLineAsync(cancellationToken);
    }

    const int MinColumnWidth = 6;
    private static string FormatPagedTableData<T>(T model, params TableField<T>[] tableFields)
    {
        StringBuilder builder = new();
        Func<object?, string?>? defaultFormat = x => x?.ToString();
        FieldVisitor visitor = new ();
        foreach (var tableField in tableFields)
        {
            visitor.Visit(tableField.Field);

            if (!visitor.HasProperty)
            {
                continue;
            }

            if (builder.Length != 0)
            {
                builder.Append("\t|\t");
            }

            var modelValue = visitor.Property.GetValue(model);
            var format = tableField.Format ?? defaultFormat;
            var formattedValue = format(modelValue).Limit(tableField.RowWidth.GetValueOrDefault(MinColumnWidth));

            builder.Append($"{formattedValue}");
        }

        return builder.ToString();
    }

    public static async Task DisplayPagedTable<T, TDestination>(this IManagedStream managedStream, IUnitPagedResult<T> results,
        Func<T, TDestination> map, CancellationToken cancellationToken, params TableField<TDestination>[] tableFields)
    {
        StringBuilder builder = new();
        await managedStream.Out.WriteLineAsync(new string('-', (int)managedStream.Width), cancellationToken);
        foreach (var tableField in tableFields)
        {
            if (builder.Length != 0)
            {
                builder.Append("\t|\t");
            }
            var val = $"{tableField.Title.Limit(tableField.RowWidth.GetValueOrDefault(MinColumnWidth))}";
            builder.Append(val);
        }
        builder.AppendLine();
        builder.Append('-', Convert.ToInt32(managedStream.Width));

        await managedStream.Out.WriteLineAsync(builder.ToString(), cancellationToken);
        await DisplayPaged(managedStream, results, map, x => FormatPagedTableData(x, tableFields), cancellationToken);
    }

    public static async Task DisplayPaged<T, TDestination>(this IManagedStream managedStream, IUnitPagedResult<T> results, 
        Func<T, TDestination> map, Func<TDestination, string> formatData, CancellationToken cancellationToken)
    {
        if (!results.IsSuccess)
        {
            await managedStream.Error.WriteLineAsync($"Error retrieving data: {results.Exception?.Message ?? "Unknown error"}", cancellationToken);
            return;
        }

        if (results.Result is null || !results.Result.Any())
        {
            await managedStream.Out.WriteLineAsync("No rows returns found", cancellationToken);
            return;
        }

        var resultData = results.Result.Select(map).ToArray();

        await managedStream.Out.WriteAsync(b =>
        {
            foreach (var data in resultData)
            {
                b.AppendLine(formatData(data));
            }

            b.Append('-', (int)managedStream.Width);
            b.AppendLine($"{results.TotalRows} found. Displaying {resultData.Length} items");
        }, cancellationToken);
    }
}
