IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Person_MediaItem' AND object_id = OBJECT_ID('dbo.PersonMedia'))
    CREATE UNIQUE NONCLUSTERED INDEX UX_Person_MediaItem ON dbo.PersonMedia (PersonId, MediaItemId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Show_MediaItem' AND object_id = OBJECT_ID('dbo.ShowMedia'))
    CREATE UNIQUE NONCLUSTERED INDEX UX_Show_MediaItem ON dbo.ShowMedia (ShowId, MediaItemId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Person_AwardType_Year' AND object_id = OBJECT_ID('dbo.Award'))
    CREATE UNIQUE NONCLUSTERED INDEX UX_Person_AwardType_Year ON dbo.Award (PersonId, AwardTypeId, [Year]) WHERE (PersonId IS NOT NULL AND ShowId IS NULL);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Show_AwardType' AND object_id = OBJECT_ID('dbo.Award'))
    CREATE UNIQUE NONCLUSTERED INDEX UX_Show_AwardType ON dbo.Award (ShowId, AwardTypeId) WHERE (ShowId IS NOT NULL AND PersonId IS NULL);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Show_Person_AwardType' AND object_id = OBJECT_ID('dbo.Award'))
    CREATE UNIQUE NONCLUSTERED INDEX UX_Show_Person_AwardType ON dbo.Award (ShowId, PersonId, AwardTypeId) WHERE (ShowId IS NOT NULL AND PersonId IS NOT NULL);