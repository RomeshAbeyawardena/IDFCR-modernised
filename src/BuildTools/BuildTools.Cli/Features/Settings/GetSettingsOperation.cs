using BuildTools.Cli.Extensions;
using BuildTools.Cli.Features.Settings;
using BuildTools.Cli.ManagedStreams;
using BuildTools.Cli.Operations;
using BuildTools.Shared.Features.Settings;
using IDFCR.Abstractions.Mapper;
using IDFCR.Abstractions.Results;

public record GetPagedSettingsQuery : PagedQuery
{

}

public class SettingDto : MapperBase<ISetting>, ISetting
{
    /// <inheritdoc/>
    public string Type { get; set; } = null!;
    /// <inheritdoc/>
    public string Key { get; set; } = null!;
    /// <inheritdoc/>
    public string? Value { get; set; } = null!;
    /// <inheritdoc/>
    public DateTime LastUpdatedTimestampUtc { get; set; }
    /// <inheritdoc/>
    public override void Map(ISetting source)
    {
        Type = source.Type;
        Key = source.Key;
        Value = source.Value;
        LastUpdatedTimestampUtc = source.LastUpdatedTimestampUtc;
    }
}

[FeatureCommand(SettingsRootOperation.Prefix, CommandName)]
public class GetSettingsOperation(IServiceProvider serviceProvider, IManagedStream managedStream, ISettingRepository settingRepository) 
    : InjectableCommandOperationBase<GetSettingsOperation>(serviceProvider, SettingsRootOperation.Prefix, CommandName, typeof(SettingsRootOperation))
{
    public const string CommandName = "get";
    protected override async Task InvokeWhenContextIsOwned(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var key = await this.GetOptionalField(managedStream, command, cancellationToken, false);
        var type = await this.GetOptionalField(managedStream, command, cancellationToken);
        if (string.IsNullOrWhiteSpace(key))
        {
            //generate paged list
            var pagedResult = await settingRepository.GetPagedAsync(new GetPagedSettingsQuery 
            {
                
            }, cancellationToken);

            if (pagedResult.HasValue)
            {
                await managedStream.DisplayPagedTable(pagedResult, t => t.Map<SettingDto>(t), cancellationToken, 
                    new TableField<SettingDto> { Field = s => s.Key, Title = "Setting Key", RowWidth = 32 },
                    new TableField<SettingDto> { Field = s => s.Value ?? "Not set", Title = "Setting Key", RowWidth = 32 },
                    new TableField<SettingDto> { Field = s => s.LastUpdatedTimestampUtc, Title = "Setting Key", RowWidth = 32, Format = x => x is DateTime date ? date.ToString() : string.Empty }
                );
            }

            return;
        }

        var valueResult = await settingRepository.GetValueAsync(key, type, cancellationToken);

        if (valueResult.HasValue)
        {
            await managedStream.Out.WriteLineAsync($"{key}: {valueResult.Result}", cancellationToken);
        }
    }
}