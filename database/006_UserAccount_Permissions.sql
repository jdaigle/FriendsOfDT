IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'IsContributor' AND object_id = OBJECT_ID('dbo.UserAccount'))
ALTER TABLE dbo.UserAccount ADD IsContributor bit NOT NULL CONSTRAINT DF_UserAccount_IsContributor DEFAULT (0);

IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_UserAccount_IsContributor' AND parent_object_id = OBJECT_ID('dbo.UserAccount'))
ALTER TABLE dbo.UserAccount DROP CONSTRAINT DF_UserAccount_IsContributor;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'IsArchivist' AND object_id = OBJECT_ID('dbo.UserAccount'))
ALTER TABLE dbo.UserAccount ADD IsArchivist bit NOT NULL CONSTRAINT DF_UserAccount_IsArchivist DEFAULT (0);

IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_UserAccount_IsArchivist' AND parent_object_id = OBJECT_ID('dbo.UserAccount'))
ALTER TABLE dbo.UserAccount DROP CONSTRAINT DF_UserAccount_IsArchivist;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'IsAdmin' AND object_id = OBJECT_ID('dbo.UserAccount'))
ALTER TABLE dbo.UserAccount ADD IsAdmin bit NOT NULL CONSTRAINT DF_UserAccount_IsAdmin DEFAULT (0);

IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_UserAccount_IsAdmin' AND parent_object_id = OBJECT_ID('dbo.UserAccount'))
ALTER TABLE dbo.UserAccount DROP CONSTRAINT DF_UserAccount_IsAdmin;