USE master;

IF (@cleanRestore = 1)
BEGIN
    ALTER DATABASE [PackageManager] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

    DROP DATABASE IF EXISTS [PackageManager];
END

SELECT DB_ID('PackageManager')

IF ((SELECT DB_ID('PackageManager')) IS NULL)
	BEGIN
    CREATE DATABASE [PackageManager];
END

