namespace IDFCR.Abstractions.Cli.Formatters;

public interface IFormatRendererFactory
{
    string Render<T>(T model);
    IEnumerable<IFormatRenderer<T>> GetRenderProviders<T>();
}
