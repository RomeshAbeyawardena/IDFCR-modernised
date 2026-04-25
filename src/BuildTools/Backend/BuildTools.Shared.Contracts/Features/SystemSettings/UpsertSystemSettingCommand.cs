using IDFCR.Abstractions.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildTools.Shared.Contracts.Features.SystemSettings
{
    public record UpsertSystemSettingCommand : IUnitResultRequest<object>
    {
        public SystemSettingDto? Setting { get; init; }
        public bool CommitChanges { get; init; }
    }
}
