using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Filters.Tests.Assets;

public interface ITestFilter : IFilter
{

}

public record TestPagedFilterRequest : PagedQuery
{
    
}

public record TestFilterRequest : ITestFilter
{

}
