param (
    [Parameter(Mandatory)]
    [string] $connectionString,
    [Parameter(Mandatory)]
    [string] $packageName,
    [Parameter(Mandatory)]
    [string[]] $packageTags
)

try {
    $sql = Get-Content 'UpdatePackageTags.sql' -Raw
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
