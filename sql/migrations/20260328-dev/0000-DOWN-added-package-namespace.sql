USE [PackageManager]

ALTER TABLE [Package]
    DROP CONSTRAINT UQ_Package;

ALTER TABLE [Package]
    DROP CONSTRAINT UQ_Package_Namespace;

ALTER TABLE [Package]
    DROP COLUMN [Namespace];

ALTER TABLE [Package]
    ADD CONSTRAINT UQ_Package_ALIAS UNIQUE NONCLUSTERED ([Alias]);

ALTER TABLE [Package]
    ADD CONSTRAINT UQ_Package_NAME UNIQUE NONCLUSTERED ([Name]);
