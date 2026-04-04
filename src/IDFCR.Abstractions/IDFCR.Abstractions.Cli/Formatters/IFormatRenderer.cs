using IDFCR.Abstractions.Cli.Extensions;

namespace IDFCR.Abstractions.Cli.Formatters;

public interface IFormatRenderer<T>
{
    TableField<T> Fields { get; }
    bool CanRender(T model);
    string Render(T model);
}
