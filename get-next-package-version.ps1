param (
    [Parameter(Mandatory)]
    [string] $connectionString,
    [Parameter(Mandatory)]
    [string] $packageName,
    [Parameter(Mandatory)]
    [string] $packageAlias,
    [Parameter(Mandatory)]
    [string] $packageDescription,
    [Parameter(Mandatory)]
    [string] $versionPrefix,
    [Parameter(Mandatory)]
    [string[]] $packageTags,
    [Parameter(Mandatory)]
    [string[]] $packageVersionTags
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
        @versionPrefix = @versionPrefix,
        @tags = @tags";

    [void]$command.Parameters.AddWithValue("packageName", $packageName)
    [void]$command.Parameters.AddWithValue("packageAlias", $packageAlias)
    [void]$command.Parameters.AddWithValue("packageDescription", $packageDescription)
    [void]$command.Parameters.AddWithValue("versionPrefix", $versionPrefix)
    [void]$command.Parameters.AddWithValue("tags", [string]::Join(',', $packageVersionTags))

    $result = $command.ExecuteScalar();
    Write-Output $result;
    $command.Dispose();
}
finally {
    if ($null -ne $conn) {
        $conn.Dispose()
    }
}