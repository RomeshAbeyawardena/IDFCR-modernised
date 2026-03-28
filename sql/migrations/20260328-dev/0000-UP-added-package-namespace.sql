USE [PackageManager]

ALTER TABLE [Package]
    DROP CONSTRAINT UQ_Package_ALIAS;

ALTER TABLE [Package]
    ADD [Namespace] NVARCHAR(255) NOT NULL
    CONSTRAINT UQ_Package_Namespace UNIQUE NONCLUSTERED
        
ALTER TABLE [Package]
    ADD CONSTRAINT UQ_Package UNIQUE ([Name], [Namespace])