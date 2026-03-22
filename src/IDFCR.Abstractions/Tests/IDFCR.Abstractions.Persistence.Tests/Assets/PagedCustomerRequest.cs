using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Persistence.Tests.Assets
{
    internal record PagedCustomerRequest : PagedQuery, IPagedGlobalRequest
    {
        public string? NameContains { get; set; }
        public bool ShowAll { get; set; }
    }
}
