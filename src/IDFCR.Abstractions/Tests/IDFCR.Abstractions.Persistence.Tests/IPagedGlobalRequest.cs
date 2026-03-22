using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Persistence.Tests;

internal interface IPagedGlobalRequest : IPagedQuery
{
    bool ShowAll { get; set; }
}
