param (
    [string] $connectionString,
    [bool] $cleanRestore
)
try {
    if ($cleanRestore -eq $true) {
        $answer = Read-Host("This will wipe all data, are you sure? Type 'YES' to proceed");
        
        if ($answer -ne "YES") {
            $cleanRestore = $false;
            Write-Verbose("Clean restore flag reset");
        }
        else {
            Write-Verbose("Clean restore flag remains");
        }
    }

    $conn = [System.Data.SqlClient.SqlConnection]::new($connectionString);

    $fileParts = ('initial-db-setup.sql', 'initial.sql', 'initial.storedproc.sql');
    $conn.Open();

    foreach ($filePart in $fileParts) {
        $command = $conn.CreateCommand();
        $currentDirectory = Get-Location;
        $sql = [System.IO.File]::ReadAllText([System.IO.Path]::Combine($currentDirectory, $filePart));
        if ($sql.Contains("@cleanRestore")) {
            [void]$command.Parameters.AddWithValue("cleanRestore", $cleanRestore);
        }

        $command.CommandText = $sql;
        
        [void]$command.ExecuteNonQuery()
        $command.Dispose();
    }
}
finally {
    if ($null -ne $conn) {
        $conn.Dispose()
    }
}