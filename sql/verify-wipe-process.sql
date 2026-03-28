USE [PackageManager]
-- @settingKey is a stored procedure parameter; it is provided at execution time.
-- Returns 1 if: table missing, setting key not found, or setting value is true
-- Returns 0 otherwise
IF (OBJECT_ID('SYSTEM_CONFIG.Setting','U') IS NULL)
BEGIN
    SELECT 1;
END
ELSE IF ((SELECT COUNT([Value])
FROM SYSTEM_CONFIG.Setting
WHERE [Key] = @settingKey) <> 1) 
BEGIN
    SELECT 1;
END
ELSE
    SELECT CASE WHEN ISNULL([Value], 'true') = 'true'
        THEN 1
        ELSE 0
    END
FROM SYSTEM_CONFIG.Setting
WHERE [Key] = @settingKey