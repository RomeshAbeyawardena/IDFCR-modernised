using IDFCR.Abstractions.Mapper;

namespace IDFCR.Abstractions.Results;

/// <summary>
/// Describes a paging request that uses skip/take semantics.
/// </summary>
public interface IConventionalPagedQuery : IMapper<IConventionalPagedQuery>, IMapper<IPagedQuery>
{
    /// <summary>
    /// Gets the number of items to take.
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Gets the number of items to skip.
    /// </summary>
    int? Skip { get; }
}
