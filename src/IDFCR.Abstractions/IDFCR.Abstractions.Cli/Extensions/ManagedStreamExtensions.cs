using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;
using System.Reflection;
using System.Text;

namespace IDFCR.Abstractions.Cli.Extensions;

/// <summary>
/// Defines extension methods for IManagedStream to facilitate common operations such as prompting for input and displaying paged results in a tabular format.
/// </summary>
public static class ManagedStreamExtensions
{
    /// <summary>
    /// Defines a method to prompt the user for input via the IManagedStream. It writes a prompt message to the output stream and waits for the user to enter a response, which is then read from the input stream and returned as a string. The method supports cancellation through a CancellationToken.
    /// </summary>
    /// <param name="stream">The managed stream to use for input and output.</param>
    /// <param name="prompt">The prompt message to display to the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The user's input as a string, or null if no input was provided.</returns>
    public static async Task<string?> PromptAsync(this IManagedStream stream, string prompt, CancellationToken cancellationToken)
    {
        await stream.Out.WriteAsync($"{prompt}: ", cancellationToken);

        return await stream.In.ReadLineAsync(cancellationToken);
    }

    const int MinColumnWidth = 6;

    /// <summary>
    /// Defines a method to format a model's data into a tabular string representation based on specified table fields. It uses a FieldVisitor to access the properties of the model defined by the TableField instances, applies any provided formatting functions, and constructs a string with the formatted values separated by tabs and pipes for display in a paged table format. The method ensures that each column adheres to a minimum width for better readability.
    /// </summary>
    /// <typeparam name="T">The type of the model to format.</typeparam>
    /// <param name="model">The model instance to format.</param>
    /// <param name="tableFields">An array of TableField instances defining the columns and formatting for the table.</param>
    /// <returns>A string representing the formatted table row for the given model.</returns>
    private static string FormatPagedTableData<T>(T model, params TableField<T>[] tableFields)
    {
        Func<object?, string?>? defaultFormat = x => x?.ToString() ?? string.Empty;
        StringBuilder builder = new();
        foreach (var tableField in tableFields)
        {
            if (!tableField.HasProperty)
            {
                continue;
            }

            if (builder.Length != 0)
            {
                builder.Append("\t|\t");
            }

            var formattedValue = string.Empty;

            var modelValue = tableField.Property.GetValue(model);
            var format = tableField.Format ?? defaultFormat;

            var raw = format(modelValue) ?? throw new InvalidOperationException(
                    $"TableField formatter for '{tableField.Property.Name}' returned null. " +
                    "Formatters must return a non-null string.");

            formattedValue = raw.Limit(tableField.RowWidth ?? MinColumnWidth);


            builder.Append(formattedValue);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Defines a method to display paged results in a tabular format using an IManagedStream. It takes a paged result set, maps the data to a destination type, and formats it according to specified table fields. The method writes the table header, formats each row of data using the provided mapping and formatting functions, and outputs the formatted table to the managed stream. It also handles cases where there are no results or if an error occurs during data retrieval, providing appropriate messages to the user. Cancellation support is included through a CancellationToken parameter.
    /// </summary>
    /// <typeparam name="T">The type of the source data.</typeparam>
    /// <typeparam name="TDestination">The type of the destination data after mapping.</typeparam>
    /// <param name="managedStream">The managed stream to write the output to.</param>
    /// <param name="results">The paged result set to display.</param>
    /// <param name="map">A function to map the source data to the destination type.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <param name="tableFields">An array of TableField instances defining the columns and formatting for the table.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task DisplayPagedTable<T, TDestination>(this IManagedStream managedStream, IUnitPagedResult<T> results,
        Func<T, TDestination> map, CancellationToken cancellationToken, params TableField<TDestination>[] tableFields)
    {
        var expression = MaximumLengthStringExpressionBuilder<T>.BuildExpression().Compile();
        
        Dictionary<string, int> stringColumnLengths = [];

        if (results.HasValue)
        {
            stringColumnLengths = results.Result.SelectMany(expression).GroupBy(x => x.Key).ToDictionary(x => x.Key, y => y.Max(x => x.Value));
        }

        FieldVisitor visitor = new();

        StringBuilder builder = new();
        await managedStream.Out.WriteLineAsync(new string('-', (int)managedStream.Width), cancellationToken);
        foreach (var tableField in tableFields)
        {
            visitor.Visit(tableField.Field);

            tableField.Property = visitor.Property;

            if (builder.Length != 0)
            {
                builder.Append("\t|\t");
            }

            if (visitor.HasProperty && stringColumnLengths.TryGetValue(visitor.Property.Name, out var columnLength))
            {
                tableField.RowWidth = tableField.RowWidth.HasValue && tableField.RowWidth > columnLength ? tableField.RowWidth : columnLength;
            }

            var val = $"{tableField.Title.Limit(tableField.RowWidth.GetValueOrDefault(MinColumnWidth))}";
            builder.Append(val);
        }
        builder.AppendLine();
        builder.Append('-', Convert.ToInt32(managedStream.Width));

        await managedStream.Out.WriteLineAsync(builder.ToString(), cancellationToken);
        await DisplayPaged(managedStream, results, map, x => FormatPagedTableData(x, tableFields), cancellationToken);
    }

    /// <summary>
    /// Defines a method to display paged results in a simple list format using an IManagedStream. It takes a paged result set, maps the data to a destination type, and formats it using a provided formatting function. The method writes each formatted item to the output stream, separated by lines, and provides a summary of the total rows found and displayed. It also handles cases where there are no results or if an error occurs during data retrieval, providing appropriate messages to the user. Cancellation support is included through a CancellationToken parameter.
    /// </summary>
    /// <typeparam name="T">The type of the source data.</typeparam>
    /// <typeparam name="TDestination">The type of the destination data after mapping.</typeparam>
    /// <param name="managedStream">The managed stream to write the output to.</param>
    /// <param name="results">The paged result set to display.</param>
    /// <param name="map">A function to map the source data to the destination type.</param>
    /// <param name="formatData">A function to format the mapped data for display.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
            b.AppendLine($"{Environment.NewLine}{results.TotalRows} found. Displaying {resultData.Length} items");
        }, cancellationToken);
    }
}
