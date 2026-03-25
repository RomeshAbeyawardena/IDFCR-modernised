param (
    [string] $connectionString
)
try {
    $conn = [System.Data.SqlClient.SqlConnection]::new($connectionString);

    $command = $conn.CreateCommand();
    $command.CommandText = "SELECT OBJECT_ID('dbo.PackageVersions')";

    $conn.Open();

    $result = $command.ExecuteScalar();
    $command.Dispose();

    if ($result -eq [System.DBNull]::Value) {
        $command = $conn.CreateCommand();
        $command.CommandText = "CREATE TABLE dbo.PackageVersions (
    PackageName NVARCHAR(255) NOT NULL,
    CurrentVersion INT NOT NULL,
    LastUpdated DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_PackageVersions PRIMARY KEY CLUSTERED (PackageName));"

        $command.ExecuteNonQuery()
        $command.Dispose();
    }

    "Object ID: $result";

    $command = $conn.CreateCommand();
    $command.CommandText = "SELECT OBJECT_ID('dbo.GetNextPackageVersion')";

    $result = $command.ExecuteScalar();

    "Object ID: $result";

    $command.Dispose();

    if ($result -eq [System.DBNull]::Value) {
        $command = $conn.CreateCommand();
        $command.CommandText = "CREATE OR ALTER PROCEDURE dbo.GetNextPackageVersion
    @PackageName NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @RetryCount INT = 0;
    DECLARE @MaxRetries INT = 5;
    DECLARE @Allocated TABLE (Version INT);

    WHILE @RetryCount < @MaxRetries
    BEGIN
        BEGIN TRY
            DELETE FROM @Allocated; -- Reset for this attempt

            BEGIN TRANSACTION;

            UPDATE dbo.PackageVersions WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
            SET CurrentVersion = CurrentVersion + 1,
                LastUpdated = SYSUTCDATETIME()
            OUTPUT inserted.CurrentVersion INTO @Allocated(Version)
            WHERE PackageName = @PackageName;

            
            IF NOT EXISTS (SELECT 1 FROM @Allocated)
            BEGIN
                INSERT INTO dbo.PackageVersions (PackageName, CurrentVersion)
                OUTPUT inserted.CurrentVersion INTO @Allocated(Version)
                VALUES (@PackageName, 1);
            END

            COMMIT TRANSACTION;

            -- Hand the ticket to the agent
            SELECT Version FROM @Allocated;
            RETURN;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

            -- 1205: Deadlock (two agents reached for the same ticket at once)
            -- 2627/2601: Race condition where two agents tried to 'start' the dispenser at once
            IF ERROR_NUMBER() IN (1205, 2627, 2601)
            BEGIN
                SET @RetryCount += 1;
                
                DECLARE @Delay CHAR(12) = '00:00:00.' + CAST(25 + (ABS(CHECKSUM(NEWID())) % 75) AS VARCHAR(3));
                WAITFOR DELAY @Delay;
                CONTINUE;
            END
        END CATCH
    END
END";

        $command.ExecuteNonQuery()
        $command.Dispose();
        $conn.Close();
    }
}
finally {
    $conn.Dispose();
}