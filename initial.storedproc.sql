CREATE OR ALTER PROCEDURE dbo.GetNextPackageVersion
	@packageName NVARCHAR(255),
	@packageAlias NVARCHAR(255),
	@packageDescription NVARCHAR(2000),
	@versionPrefix NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @packageId UNIQUEIDENTIFIER;
	DECLARE @revisionId INT;
	DECLARE @trimmedPackageName NVARCHAR(255) = LTRIM(RTRIM(@packageName));

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
		SELECT @revisionId = ISNULL(MAX(RevisionNumber), -1) + 1
		FROM dbo.PackageVersion WITH (UPDLOCK, HOLDLOCK)
		WHERE PackageId = @packageId AND VersionSuffix = @versionPrefix
	END

		INSERT INTO [dbo].[PackageVersion]
		([PackageId], [VersionSuffix], [RevisionNumber])
	VALUES
		(@packageId, @versionPrefix, @revisionId);
		COMMIT TRANSACTION;
		SELECT SCOPE_IDENTITY() AS PackageVersionId;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		IF ERROR_NUMBER() IN (1205, 2627, 2601)
		BEGIN
		RAISERROR('A concurrency conflict occurred. Please retry the operation.', 16, 1);
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