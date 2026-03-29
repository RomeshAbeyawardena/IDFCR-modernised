using System.Linq.Expressions;

namespace BuildTools.Cli.Extensions;

public class TableField<T>
{
    public required Expression<Func<T, object?>> Field { get; init; }

    public Func<object?, string?>? Format { get; init; }
    
    public string? Title { get; init; }
    public int? RowWidth { get; init; }
}
