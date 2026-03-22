using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Persistence.Tests
{
    internal record PagedCustomerRequest : PagedQuery
    {
        public string? NameContains { get; set; }
    }
}
