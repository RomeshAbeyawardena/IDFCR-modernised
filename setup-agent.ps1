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
    PackageVersionId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_PackageVersionID PRIMARY KEY
        CONSTRAINT DF_PackageVersionID DEFAULT NEWSEQUENTIALID(),
    PackageName NVARCHAR(255) NOT NULL,
    VersionPrefix NVARCHAR(50) NOT NULL, -- e.g., '1.0.0'
    CurrentVersion INT NOT NULL,
    LastUpdated DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_PackageVersions UNIQUE CLUSTERED (PackageName, VersionPrefix)
);"

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
    @PackageName NVARCHAR(255),
    @VersionPrefix NVARCHAR(50)
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
            DELETE FROM @Allocated;

            BEGIN TRANSACTION;

            -- Try to increment the specific version branch
            UPDATE dbo.PackageVersions WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
            SET CurrentVersion = CurrentVersion + 1,
                LastUpdated = SYSUTCDATETIME()
            OUTPUT inserted.CurrentVersion INTO @Allocated(Version)
            WHERE PackageName = @PackageName 
              AND VersionPrefix = @VersionPrefix;

            -- If this branch (e.g. 1.0.1) is new, start at 0
            IF NOT EXISTS (SELECT 1 FROM @Allocated)
            BEGIN
                INSERT INTO dbo.PackageVersions (PackageName, VersionPrefix, CurrentVersion)
                OUTPUT inserted.CurrentVersion INTO @Allocated(Version)
                VALUES (@PackageName, @VersionPrefix, 0);
            END

            COMMIT TRANSACTION;

            SELECT Version FROM @Allocated;
            RETURN;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

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
    }
}
finally {
    if ($null -ne $conn) {
        $conn.Dispose()
    }
}