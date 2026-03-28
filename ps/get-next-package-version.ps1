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
    [string] $packageNamespace,
    [Parameter(Mandatory)]
    [string] $versionPrefix,
    [Parameter(Mandatory)]
    [string[]] $packageVersionTags
)
try {
    $commitID = git rev-parse HEAD
    $conn = [System.Data.SqlClient.SqlConnection]::new($connectionString);
    $conn.Open();
    $command = $conn.CreateCommand();
    $command.CommandText = "USE PackageManager;
    
    EXEC dbo.GetNextPackageVersion
        @packageName = @packageName,
        @packageAlias = @packageAlias,
        @packageNamespace = @packageNamespace,
        @packageDescription = @packageDescription,
        @versionPrefix = @versionPrefix,
        @commitId = @commitId,
        @tags = @tags";

    [void]$command.Parameters.AddWithValue("packageName", $packageName)
    [void]$command.Parameters.AddWithValue("packageAlias", $packageAlias)
    [void]$command.Parameters.AddWithValue("packageNamespace", $packageNamespace)
    [void]$command.Parameters.AddWithValue("packageDescription", $packageDescription)
    [void]$command.Parameters.AddWithValue("versionPrefix", $versionPrefix)
    [void]$command.Parameters.AddWithValue("commitId", $commitID)
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