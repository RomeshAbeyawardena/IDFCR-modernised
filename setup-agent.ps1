param (
    [string] $connectionString,
    [bool] $cleanRestore
)
try {
    $conn = [System.Data.SqlClient.SqlConnection]::new($connectionString);
    $conn.Open();
    if ($cleanRestore -eq $true) {

        $command = $conn.CreateCommand();
        $command.CommandText = "USE [PackageManager]; SELECT [Value] FROM SYSTEM_CONFIG.Setting WHERE [Key] = @settingKey";
        [void]$command.Parameters.AddWithValue("settingKey", "CAN_AGENTS_WIPE_DATA");
        $settingValue = $command.ExecuteScalar();
        $command.Dispose();
        $parsed = $true
        
        if ($settingValue -ne [DBNull]::Value) {
            if (-not [bool]::TryParse($settingValue, [ref]$parsed)) {
                Write-Error("Invalid CAN_AGENTS_WIPE_DATA value in DB")
                exit 1
            }
        }

        if ($parsed -eq $false) {
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