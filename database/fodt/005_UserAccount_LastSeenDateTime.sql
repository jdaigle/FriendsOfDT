IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'LastSeenDateTime' AND object_id = OBJECT_ID('dbo.UserAccount'))
ALTER TABLE dbo.UserAccount ADD LastSeenDateTime datetime2 NULL;