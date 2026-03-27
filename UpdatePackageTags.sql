DECLARE @packageId UNIQUEIDENTIFIER;
DECLARE @trimmedPackageName NVARCHAR(255) = LTRIM(RTRIM(@packageName));
DECLARE @InsertedTagCount INT = 0;
DECLARE @UpdatedTagDisplayNameCount INT = 0;
DECLARE @InsertedPackageTagCount INT = 0;

DECLARE @rawTags TABLE
(
    TagName NVARCHAR(4000) NOT NULL PRIMARY KEY
);

DECLARE @parsedTags TABLE
(
    TagName NVARCHAR(50) NOT NULL PRIMARY KEY
);

IF NULLIF(@trimmedPackageName, '') IS NULL
BEGIN
    RAISERROR('Package name is required.', 16, 1);
    RETURN;
END

IF NULLIF(LTRIM(RTRIM(@tags)), '') IS NULL
BEGIN
    RAISERROR('At least one tag is required.', 16, 1);
    RETURN;
END

INSERT INTO @rawTags (TagName)
SELECT DISTINCT LTRIM(RTRIM([value]))
FROM STRING_SPLIT(@tags, ',')
WHERE NULLIF(LTRIM(RTRIM([value])), '') IS NOT NULL;

IF EXISTS (SELECT 1 FROM @rawTags WHERE LEN(TagName) > 50)
BEGIN
    RAISERROR('One or more tag names exceed the maximum length of 50 characters.', 16, 1);
    RETURN;
END

INSERT INTO @parsedTags (TagName)
SELECT TagName
FROM @rawTags;

BEGIN TRY
    BEGIN TRANSACTION;

    SELECT TOP(1)
        @packageId = p.PackageId
    FROM dbo.Package p WITH (UPDLOCK, HOLDLOCK)
    WHERE p.[Name] = @trimmedPackageName;

    IF @packageId IS NULL
    BEGIN
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        RAISERROR('Package ''%s'' not found.', 16, 1, @trimmedPackageName);
        RETURN;
    END

    INSERT INTO dbo.Tag ([Name], [DisplayName])
    SELECT pt.TagName, pt.TagName
    FROM @parsedTags pt
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Tag t WITH (UPDLOCK, HOLDLOCK)
        WHERE t.[Name] = pt.TagName
    );

    SET @InsertedTagCount = @@ROWCOUNT;

    UPDATE t
    SET t.[DisplayName] = pt.TagName
    FROM dbo.Tag t
    INNER JOIN @parsedTags pt ON pt.TagName = t.[Name]
    WHERE t.[DisplayName] IS NULL
        OR t.[DisplayName] <> pt.TagName;

    SET @UpdatedTagDisplayNameCount = @@ROWCOUNT;

    INSERT INTO dbo.PackageTag
        ([PackageTagId], [PackageId], [TagId])
    SELECT NEWID(), @packageId, t.TagId
    FROM @parsedTags pt
    INNER JOIN dbo.Tag t WITH (UPDLOCK, HOLDLOCK) ON t.[Name] = pt.TagName
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.PackageTag pkgTag WITH (UPDLOCK, HOLDLOCK)
        WHERE pkgTag.[PackageId] = @packageId
            AND pkgTag.[TagId] = t.[TagId]
    );

    SET @InsertedPackageTagCount = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT
        @InsertedTagCount AS InsertedTagCount,
        @UpdatedTagDisplayNameCount AS UpdatedTagDisplayNameCount,
        @InsertedPackageTagCount AS InsertedPackageTagCount;
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