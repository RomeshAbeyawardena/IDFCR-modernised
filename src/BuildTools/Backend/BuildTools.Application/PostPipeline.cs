using BuildTools.Shared.Contracts.Feature.Tags;
using IDFCR.Abstractions.Persistence;
using IDFCR.Abstractions.Results;
using System.Threading;
using System.Threading.Tasks;

namespace BuildTools.Application;

public class PostProcessUnitOfWorkPipeline<TRequest, TResponse>(IUnitOfWork unitOfWork) : MediatR.Pipeline.IRequestPostProcessor<TRequest, TResponse>
{
    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        if (request is IUnitOfWorkRequest workRequest && workRequest.CommitChanges)
        {
            if(response is IUnitResult unitResult && unitResult.IsSuccess)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
