param (
    [string] $connectionString,
    [string] $packageName,
    [string] $versionPrefix
)
try {
    $conn = [System.Data.SqlClient.SqlConnection]::new($connectionString);
    $conn.Open();
    $command = $conn.CreateCommand();
    $command.CommandText = "EXEC dbo.GetNextPackageVersion 
         @PackageName = @PackageName
        ,@VersionPrefix=@VersionPrefix";
    $parameter = $command.CreateParameter();
    $parameter.ParameterName = "PackageName"
    $parameter.DbType = 'String';
    $parameter.Value = $packageName;

    [void]$command.Parameters.Add($parameter);

    $parameter = $command.CreateParameter();
    $parameter.ParameterName = "VersionPrefix"
    $parameter.DbType = 'String';
    $parameter.Value = $versionPrefix;

    [void]$command.Parameters.Add($parameter);

    $result = $command.ExecuteScalar();
    Write-Output $result;
    $command.Dispose();
}
finally {
    if ($null -ne $conn) {
        $conn.Dispose()
    }
}