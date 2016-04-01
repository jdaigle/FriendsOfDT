IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'LargeFileIsSameAsOriginal' AND object_id = OBJECT_ID('dbo.Photo'))
ALTER TABLE dbo.Photo ADD LargeFileIsSameAsOriginal bit NOT NULL CONSTRAINT DF_Photo_LargeFileIsSameAsOriginal DEFAULT (1);

IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_Photo_LargeFileIsSameAsOriginal' AND parent_object_id = OBJECT_ID('dbo.Photo'))
ALTER TABLE dbo.Photo DROP CONSTRAINT DF_Photo_LargeFileIsSameAsOriginal;