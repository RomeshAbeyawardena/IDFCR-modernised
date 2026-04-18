using BuildTools.GRPC.Shared.Contracts.Common;
using BuildTools.GRPC.Shared.Contracts.Feature.Tags;
using TagsFeature = BuildTools.Shared.Contracts.Feature.Tags;
using Grpc.Core;
using MediatR;
using System.Threading.Tasks;
using System;
using IDFCR.Abstractions.Results.Extensions;

namespace BuildTools.GRPC.Application;

public class DefaultUpsertTagCommandService(IMediator mediator) : UpsertTagCommandService.UpsertTagCommandServiceBase
{
    public override async Task<UpsertTagCommandResult> UpsertTag(UpsertTagCommand request, ServerCallContext context)
    {
        var tag = new TagsFeature.TagDto
        {
            DisplayName = request.Tag.DisplayName,
            Name = request.Tag.Name,
            Id = request.Tag.Id
        };

        var result = await mediator.Send(new TagsFeature.UpsertTagCommand { Tag = tag, CommitChanges = true }, context.CancellationToken);

        if (!Enum.TryParse<UnitAction>(result.Action.ToString(), false, out var unitAction))
        {
            unitAction = UnitAction.None;
        }

        if (!Enum.TryParse<FailureReason>(result.FailureReason.ToString(), false, out var failureReason))
        {
            failureReason = FailureReason.Unknown;
        }

        return new UpsertTagCommandResult
        {
            TagId = result.GetResultOrDefault()?.ToString(), //is a guid
            Result = new UnitResult()
            {
                IsSuccess = result.IsSuccess,
                Action = unitAction,
                ErrorMessage = result.Exception?.Message,
                FailureReason = failureReason
            }
        };
    }
}
