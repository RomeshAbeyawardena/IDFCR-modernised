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
		[PackageId] UNIQUEIDENTIFIER NOT NULL
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

IF ((SELECT OBJECT_ID('[dbo].[Tag]', 'U')) IS NULL)
BEGIN
	CREATE TABLE [dbo].[Tag]
	(
		[TagId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT PK_Tag PRIMARY KEY
			CONSTRAINT DF_Tag_TagId DEFAULT GETSEQUENTIALID(),
		[Name] NVARCHAR(50) NOT NULL
			CONSTRAINT UQ_Tag_Name UNIQUE ([Name]),
		[DisplayName] NVARCHAR(2000) NULL,
	)
END

IF ((SELECT OBJECT_ID('[dbo].[PackageTag]', 'U')) IS NULL)
BEGIN
	CREATE TABLE [dbo].[PackageTag]
	(
		[PackageTagId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT PK_PackageVersionTag PRIMARY KEY,
		[PackageId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT FK_PackageTag_Package FOREIGN KEY 
			REFERENCES [dbo].[Package].[PackageId],
		[TagId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT FK_PackageTag_Tag FOREIGN KEY 
			REFERENCES [dbo].[Tag].[TagId],
		CONSTRAINT UQ_PackageVersionTag UNIQUE ([PackageId], [TagId])
	)
END

IF ((SELECT OBJECT_ID('[dbo].[PackageVersionTag]', 'U')) IS NULL)
BEGIN
	CREATE TABLE [dbo].[PackageVersionTag]
	(
		[PackageVersionTagId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT PK_PackageVersionTag PRIMARY KEY,
		[PackageVersionId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT FK_PackageVersionTag_PackageVersion FOREIGN KEY
			REFERENCES [dbo].[PackageVersion].[PackageVersionId],
		[TagId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT FK_PackageVersionTag_Tag FOREIGN KEY 
			REFERENCES [dbo].[Tag].[TagId],
		CONSTRAINT UQ_PackageVersionTag UNIQUE ([PackageVersionId], [TagId])
	)
END