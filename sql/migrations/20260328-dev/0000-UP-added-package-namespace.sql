USE [PackageManager]

ALTER TABLE [Package]
    DROP CONSTRAINT UQ_Package_NAME;

ALTER TABLE [Package]
    DROP CONSTRAINT UQ_Package_ALIAS;

ALTER TABLE [Package]
    ADD [Namespace] NVARCHAR(255) NOT NULL
    CONSTRAINT DF_Package_Namespace DEFAULT 'com.unknown.package'

ALTER TABLE [Package]
    ADD CONSTRAINT DF_Package_Namespace UNIQUE NONCLUSTERED ([Namespace])
        
ALTER TABLE [Package]
    ADD CONSTRAINT UQ_Package UNIQUE ([Name], [Namespace])