IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Person_Photo' AND object_id = OBJECT_ID('dbo.PersonPhoto'))
    CREATE UNIQUE NONCLUSTERED INDEX UX_Person_Photo ON dbo.PersonPhoto (PersonId, PhotoId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Show_Photo' AND object_id = OBJECT_ID('dbo.ShowPhoto'))
    CREATE UNIQUE NONCLUSTERED INDEX UX_Show_Photo ON dbo.ShowPhoto (ShowId, PhotoId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Person_AwardType_Year' AND object_id = OBJECT_ID('dbo.Award'))
    CREATE UNIQUE NONCLUSTERED INDEX UX_Person_AwardType_Year ON dbo.Award (PersonId, AwardTypeId, [Year]) WHERE (PersonId IS NOT NULL AND ShowId IS NULL);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Show_AwardType' AND object_id = OBJECT_ID('dbo.Award'))
    CREATE UNIQUE NONCLUSTERED INDEX UX_Show_AwardType ON dbo.Award (ShowId, AwardTypeId) WHERE (ShowId IS NOT NULL AND PersonId IS NULL);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Show_Person_AwardType' AND object_id = OBJECT_ID('dbo.Award'))
    CREATE UNIQUE NONCLUSTERED INDEX UX_Show_Person_AwardType ON dbo.Award (ShowId, PersonId, AwardTypeId) WHERE (ShowId IS NOT NULL AND PersonId IS NOT NULL);