param (
    [string] $connectionString,
    [string] $packageName
)
try {
    $conn = [System.Data.SqlClient.SqlConnection]::new($connectionString);
    $conn.Open();
    $command = $conn.CreateCommand();
    $command.CommandText = "EXEC dbo.GetNextPackageVersion @PackageName = @PackageName";
    $parameter = $command.CreateParameter()
    $parameter.ParameterName = "PackageName"
    $parameter.DbType = 'AnsiString';
    $parameter.Value = $packageName;
    Write-Debug($command.Parameters.Add($parameter));

    $result = $command.ExecuteScalar();
    Write-Output $result;
    $command.Dispose();
}
finally {
    if ($null -ne $conn) {
        $conn.Dispose()
    }
}