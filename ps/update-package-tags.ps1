$scriptRoot = $PSScriptRoot;

$metaScript = [System.IO.Path]::Combine($scriptRoot, 'meta.ps1');
. $metaScript;

param (
    [Parameter(Mandatory)]
    [string] $connectionString,
    [string] $packageName,
    [string] $packageNamespace,
    [Parameter(Mandatory)]
    [Tag[]] $packageTags
)
$currentDirectory = Get-Location;

try {
    $sqlPath = [System.IO.Path]::Combine($currentDirectory, 'sql', "UpdatePackageTags.sql")
    $sql = Get-Content $sqlPath -Raw
    $conn = [System.Data.SqlClient.SqlConnection]::new($connectionString);
        $conn.Open();
        $command = $conn.CreateCommand();
        $command.CommandText = "USE PackageManager;
        $sql";

        [void]$command.Parameters.AddWithValue("packageName", $packageName)
        [void]$command.Parameters.AddWithValue("tags", [string]::Join(',', $packageTags))

        [void]$command.ExecuteNonQuery();
        
        $command.Dispose();
}
finally {
    if ($null -ne $conn) {
        $conn.Dispose()
    }
}
