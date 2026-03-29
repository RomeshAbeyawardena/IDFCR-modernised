USE [PackageManager];

IF ((SELECT OBJECT_ID('[SYSTEM_CONFIG].[Setting]', 'U')) IS NULL)
BEGIN
	CREATE TABLE [SYSTEM_CONFIG].[Setting] (
		 [SettingId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT PK_SettingId PRIMARY KEY
			CONSTRAINT DF_SettingId DEFAULT NEWSEQUENTIALID()
		,[Type] NVARCHAR(100) NOT NULL
		,[Key] NVARCHAR(200) NOT NULL
			CONSTRAINT UQ_SystemConfig_Setting_Key UNIQUE NONCLUSTERED
		,[Value] NVARCHAR(MAX) NULL
		,[CreatedTimestampUtc] DATETIMEOFFSET(7) NOT NULL
		,[LastUpdatedTimestampUtc] DATETIMEOFFSET(7) NOT NULL
			CONSTRAINT DF_SystemConfig_Setting_LastUpdatedTimestampUtc 
			DEFAULT GETUTCDATE()
		,CONSTRAINT CK_Setting_Boolean CHECK (
    		[Type] != 'Boolean'
    		OR [Value] IN ('true','false'))
	)
END

IF ((SELECT OBJECT_ID('[dbo].[Package]', 'U')) IS NULL)
BEGIN
	CREATE TABLE [dbo].[Package]
	(
		[PackageId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT PK_PackageId PRIMARY KEY,
		[Name] NVARCHAR(255) NOT NULL,
		[Alias] NVARCHAR(255) NULL,
		[Namespace] NVARCHAR(255) NOT NULL,
		[Description] NVARCHAR(2000) NULL,
		CONSTRAINT UQ_Package UNIQUE ([Name], [Namespace]),
		INDEX IDX_Package_Namespace NONCLUSTERED ([Namespace])
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
		[ReleaseDateTimestampUtc] DATETIMEOFFSET(7) NOT NULL DEFAULT GETUTCDATE(),
		[CommitId] NVARCHAR(80) NOT NULL,
		[PublishedToFeed] BIT NULL,
		[LastErrorOnPublishAttempt] NVARCHAR(2000) NULL,
		[PublishedTimestampUtc] DATETIMEOFFSET(7) NULL,

		INDEX IX_PackageVersion_PackageId_CommitId (PackageId, CommitId),
		CONSTRAINT UQ_PackageVersion UNIQUE NONCLUSTERED (PackageId, VersionSuffix, RevisionNumber)
	)
END;

IF ((SELECT OBJECT_ID('[dbo].[Tag]', 'U')) IS NULL)
BEGIN
	CREATE TABLE [dbo].[Tag]
	(
		[TagId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT PK_Tag PRIMARY KEY
			CONSTRAINT DF_Tag_TagId DEFAULT NEWSEQUENTIALID(),
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
			CONSTRAINT PK_PackageTag PRIMARY KEY,
		[PackageId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT FK_PackageTag_Package FOREIGN KEY 
			REFERENCES [dbo].[Package]([PackageId]),
		[TagId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT FK_PackageTag_Tag FOREIGN KEY 
			REFERENCES [dbo].[Tag]([TagId]),
		CONSTRAINT UQ_PackageTag UNIQUE ([PackageId], [TagId])
	)
END

IF ((SELECT OBJECT_ID('[dbo].[PackageVersionTag]', 'U')) IS NULL)
BEGIN
	CREATE TABLE [dbo].[PackageVersionTag]
	(
		[PackageVersionTagId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT PK_PackageVersionTag PRIMARY KEY,
		[PackageVersionId] INT NOT NULL
			CONSTRAINT FK_PackageVersionTag_PackageVersion FOREIGN KEY
			REFERENCES [dbo].[PackageVersion]([PackageVersionId]),
		[TagId] UNIQUEIDENTIFIER NOT NULL
			CONSTRAINT FK_PackageVersionTag_Tag FOREIGN KEY 
			REFERENCES [dbo].[Tag]([TagId]),
		CONSTRAINT UQ_PackageVersionTag UNIQUE ([PackageVersionId], [TagId])
	)
END