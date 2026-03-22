using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Persistence.Tests.Assets;

internal interface IPagedGlobalRequest : IPagedQuery
{
    bool ShowAll { get; set; }
}
