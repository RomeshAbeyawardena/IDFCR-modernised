param (
    [Parameter(Mandatory)]
    [string] $connectionString,
    [Parameter(Mandatory)]
    [string] $packageName,
    [Parameter(Mandatory)]
    [string[]] $packageTags
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

        [void]$command.Parameters.AddWithValue("packageName", [string]::Join(',', $packageName))
        [void]$command.Parameters.AddWithValue("tags", [string]::Join(',', $packageTags))

        [void]$command.ExecuteNonQuery();
        
        $command.Dispose();
}
finally {
    if ($null -ne $conn) {
        $conn.Dispose()
    }
}
