CREATE OR ALTER PROCEDURE dbo.GetNextPackageVersion
	@packageName NVARCHAR(255),
	@packageAlias NVARCHAR(255),
	@packageDescription NVARCHAR(2000),
	@versionPrefix NVARCHAR(50),
	@tags NVARCHAR(2000)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @packageId UNIQUEIDENTIFIER;
	DECLARE @revisionId INT;
	DECLARE @packageVersionId INT;
	DECLARE @trimmedPackageName NVARCHAR(255) = LTRIM(RTRIM(@packageName));
	DECLARE @parsedTags TABLE
	(
		TagName NVARCHAR(50) NOT NULL PRIMARY KEY
	);

	BEGIN TRY
		BEGIN TRANSACTION;
		SELECT TOP(1)
		@packageId = PackageId
	FROM dbo.Package WITH (UPDLOCK, HOLDLOCK)
	WHERE Name = @trimmedPackageName;

		IF @packageId IS NULL
		BEGIN
		SET @packageId = NEWID();
		INSERT INTO dbo.Package
			([PackageId], [Name], [Alias], [Description])
		VALUES
			(@packageId, @trimmedPackageName, @packageAlias, @packageDescription);
		SET @revisionId = 0;
	END
		ELSE BEGIN
		UPDATE dbo.Package
		SET [Alias] = @packageAlias,
			[Description] = @packageDescription
		WHERE [PackageId] = @packageId
			AND (
				([Alias] IS NULL AND @packageAlias IS NOT NULL)
				OR ([Alias] IS NOT NULL AND @packageAlias IS NULL)
				OR ([Alias] <> @packageAlias)
				OR ([Description] IS NULL AND @packageDescription IS NOT NULL)
				OR ([Description] IS NOT NULL AND @packageDescription IS NULL)
				OR ([Description] <> @packageDescription)
			);

		SELECT @revisionId = ISNULL(MAX(RevisionNumber), -1) + 1
		FROM dbo.PackageVersion WITH (UPDLOCK, HOLDLOCK)
		WHERE PackageId = @packageId AND VersionSuffix = @versionPrefix
	END

		IF NULLIF(LTRIM(RTRIM(@tags)), '') IS NOT NULL
		BEGIN
			INSERT INTO @parsedTags (TagName)
			SELECT DISTINCT CAST(LTRIM(RTRIM([value])) AS NVARCHAR(50))
			FROM STRING_SPLIT(@tags, ',')
			WHERE NULLIF(LTRIM(RTRIM([value])), '') IS NOT NULL
				AND LEN(LTRIM(RTRIM([value]))) <= 50;

			INSERT INTO dbo.Tag ([Name])
			SELECT pt.TagName
			FROM @parsedTags pt
			WHERE NOT EXISTS
				(
					SELECT 1
					FROM dbo.Tag t WITH (UPDLOCK, HOLDLOCK)
					WHERE t.[Name] = pt.TagName
				);
		END

		INSERT INTO [dbo].[PackageVersion]
		([PackageId], [VersionSuffix], [RevisionNumber])
	VALUES
		(@packageId, @versionPrefix, @revisionId);
		SET @packageVersionId = CAST(SCOPE_IDENTITY() AS INT);

		IF NULLIF(LTRIM(RTRIM(@tags)), '') IS NOT NULL
		BEGIN
			INSERT INTO dbo.PackageVersionTag
				([PackageVersionTagId], [PackageVersionId], [TagId])
			SELECT NEWID(), @packageVersionId, t.TagId
			FROM @parsedTags pt
			INNER JOIN dbo.Tag t ON t.[Name] = pt.TagName
			WHERE NOT EXISTS
			(
				SELECT 1
				FROM dbo.PackageVersionTag pvt WITH (UPDLOCK, HOLDLOCK)
				WHERE pvt.PackageVersionId = @packageVersionId
					AND pvt.TagId = t.TagId
			);
		END

		COMMIT TRANSACTION;
		SELECT @packageVersionId AS PackageVersionId;
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0
			ROLLBACK TRANSACTION;
		IF ERROR_NUMBER() = 1205
		BEGIN
			RAISERROR('A deadlock occurred. Please retry the operation.', 16, 1);
			RETURN;
		END
		ELSE IF ERROR_NUMBER() IN (2627, 2601)
		BEGIN
			RAISERROR('A duplicate key violation occurred. The operation cannot be retried with the same data.', 16, 1);
			RETURN;
		END
		ELSE
		BEGIN
			DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
			DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
			DECLARE @ErrorState INT = ERROR_STATE();
			RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
			RETURN;
		END
	END CATCH
END