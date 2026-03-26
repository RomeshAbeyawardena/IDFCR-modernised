USE master;

IF (@cleanRestore = 1)
BEGIN
	ALTER DATABASE [PackageManager] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

	DROP DATABASE IF EXISTS [PackageManager];
END


IF ((SELECT DB_ID('PackageManager')) IS NULL)
	BEGIN
		CREATE DATABASE [PackageManager];
	END


USE [PackageManager];

IF ((SELECT OBJECT_ID('[dbo].[Package]', 'U')) IS NULL)
BEGIN
CREATE TABLE [dbo].[Package]
(
	[PackageId] UNIQUEIDENTIFIER 
		CONSTRAINT PK_PackageId PRIMARY KEY,
	[Name] NVARCHAR(255) NOT NULL
		CONSTRAINT UQ_Package_NAME UNIQUE NONCLUSTERED,
	[Alias] NVARCHAR(255) NULL
		CONSTRAINT UQ_Package_ALIAS UNIQUE NONCLUSTERED,
	[Description] NVARCHAR(2000) NULL
)
END

IF ((SELECT OBJECT_ID('[dbo].[PackageVersion]', 'U')) IS NULL)
BEGIN
CREATE TABLE [dbo].[PackageVersion]
(
	[PackageVersionId] INT IDENTITY(0,1) 
		CONSTRAINT PK_PackageVersionId PRIMARY KEY,
	[PackageId] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT FK_PackageVersion_Package FOREIGN KEY REFERENCES [dbo].[Package](PackageId),
	[VersionSuffix] NVARCHAR(50) NOT NULL,
	[RevisionNumber] INT NOT NULL,
	[ReleaseDateTimestampUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
		CONSTRAINT UQ_PackageVersion UNIQUE NONCLUSTERED (PackageId, VersionSuffix, RevisionNumber)
)
END;