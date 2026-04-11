param (
    [string]$migrationName
)

dotnet tool update --global dotnet-ef

dotnet ef --project .\BuildTools.Infrastructure.SqlServer\ migrations add $migrationName --startup-project .\BuildTools.DatabaseUpdater\