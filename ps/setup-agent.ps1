param (
    [string] $connectionString,
    [bool] $cleanRestore
)
try {
    $conn = [System.Data.SqlClient.SqlConnection]::new($connectionString);
    $conn.Open();
    if ($cleanRestore -eq $true) {
        $currentDirectory = Get-Location;
        $sqlPath = [System.IO.Path]::Combine($currentDirectory, 'sql', 'verify-wipe-process.sql') 
        $sql = Get-Content $sqlPath -Raw
        $command = $conn.CreateCommand();
        $command.CommandText = $sql;
        [void]$command.Parameters.AddWithValue("settingKey", "CAN_AGENTS_WIPE_DATA");
        $settingValue = $command.ExecuteScalar();
        $command.Dispose();
        
        if ($settingValue -eq $false) {
            Write-Error("Unable to proceed with wipe down, the database does not support this operation")
            exit 1
        }

        $answer = Read-Host("This will wipe all data, are you sure? Type 'YES' as it is written to proceed");
        
        if ($answer -ne "YES") {
            $cleanRestore = $false;
            Write-Verbose("Clean restore flag reset");
        }
        else {
            Write-Verbose("Clean restore flag remains");
        }
    }

    $fileParts = ('initial-db-setup.sql', 'initial.schema.sql', 'initial.sql', 'initial.storedproc.sql');

    foreach ($filePart in $fileParts) {
        $command = $conn.CreateCommand();
        $currentDirectory = Get-Location;
        $sql = [System.IO.File]::ReadAllText([System.IO.Path]::Combine($currentDirectory, 'sql', $filePart));
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