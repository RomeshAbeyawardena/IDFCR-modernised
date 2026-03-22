using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Persistence.Tests
{
    internal record PagedCustomerRequest : PagedQuery, IPagedGlobalRequest
    {
        public string? NameContains { get; set; }
        public bool ShowAll { get; set; }
    }
}
