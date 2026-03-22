using IDFCR.Abstractions.Mapper;

namespace IDFCR.Abstractions.Results;

public interface IConventionalPagedQuery : IMapper<IConventionalPagedQuery>, IMapper<IPagedQuery>
{
    int? Take { get; }
    int? Skip { get; }
}
