IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'GUID' AND object_id = OBJECT_ID('dbo.MediaItem'))
ALTER TABLE dbo.MediaItem ADD [GUID] uniqueidentifier NOT NULL;
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = 'Path' AND object_id = OBJECT_ID('dbo.MediaItem'))
ALTER TABLE dbo.MediaItem DROP COLUMN [Path];
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = 'ThumbnailPath' AND object_id = OBJECT_ID('dbo.MediaItem'))
ALTER TABLE dbo.MediaItem DROP COLUMN [ThumbnailPath];
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = 'TinyPath' AND object_id = OBJECT_ID('dbo.MediaItem'))
ALTER TABLE dbo.MediaItem DROP COLUMN [TinyPath];
GO
