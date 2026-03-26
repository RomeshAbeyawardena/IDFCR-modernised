param (
    [string] $connectionString,
    [bool] $cleanRestore
)
try {
    $conn = [System.Data.SqlClient.SqlConnection]::new($connectionString);

    $fileParts = ('initial.sql', 'initial.storedproc.sql');
    $conn.Open();

    foreach ($filePart in $fileParts)
    {
        $command = $conn.CreateCommand();
        $currentDirectory = Get-Location;
        $sql = [System.IO.File]::ReadAllText([System.IO.Path]::Combine($currentDirectory, $filePart));
        if ($sql.Contains("@cleanRestore"))
        {
            $command.Parameters.AddWithValue("cleanRestore",$cleanRestore);
        }

        $command.CommandText = $sql;
        
        $command.ExecuteScalar()
        $command.Dispose();
    }
}
finally {
    if ($null -ne $conn) {
        $conn.Dispose()
    }
}