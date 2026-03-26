param (
    [string] $connectionString,
    [string] $packageName,
    [string] $packageAlias,
    [string] $packageDescription,
    [string] $versionPrefix
)
try {
    $conn = [System.Data.SqlClient.SqlConnection]::new($connectionString);
    $conn.Open();
    $command = $conn.CreateCommand();
    $command.CommandText = "USE PackageManager;
    
    EXEC dbo.GetNextPackageVersion
        @packageName = @packageName,
        @packageAlias = @packageAlias,
        @packageDescription = @packageDescription,
        @versionPrefix = @versionPrefix";

    [void]$command.Parameters.AddWithValue("packageName", $packageName)
    [void]$command.Parameters.AddWithValue("packageAlias", $packageAlias)
    [void]$command.Parameters.AddWithValue("packageDescription", $packageDescription)
    [void]$command.Parameters.AddWithValue("versionPrefix", $versionPrefix)

    $result = $command.ExecuteScalar();
    Write-Output $result;
    $command.Dispose();
}
finally {
    if ($null -ne $conn) {
        $conn.Dispose()
    }
}