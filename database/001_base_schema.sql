CREATE TABLE [dbo].[Photo] (
    [PhotoId] [int] IDENTITY(1,1) NOT NULL,
    [GUID] uniqueidentifier NOT NULL,
    [InsertedDateTime] [datetime2] NOT NULL,
    CONSTRAINT [PK_Photo] PRIMARY KEY CLUSTERED 
    (
        [PhotoId] ASC
    ),
);
GO

CREATE TABLE [dbo].[Person] (
    [PersonId] [int] IDENTITY(1,1) NOT NULL,
    [Honorific] [nvarchar](50) NOT NULL,
    [FirstName] [nvarchar](50) NOT NULL,
    [MiddleName] [nvarchar](50) NOT NULL,
    [LastName] [nvarchar](50) NOT NULL,
    [Suffix] [nvarchar](50) NOT NULL,
    [Nickname] [nvarchar](100) NOT NULL,
    [Biography] [nvarchar](max) NOT NULL,
    [PhotoId] [int] NOT NULL CONSTRAINT DF_Person_PhotoId DEFAULT 1,
    [InsertedDateTime] [datetime2] NOT NULL,
    [LastModifiedDateTime] [datetime2] NOT NULL,
    CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
    (
        [PersonId] ASC
    ),
    CONSTRAINT [FK_Person_Photo]
        FOREIGN KEY ([PhotoId])
        REFERENCES [dbo].[Photo],
);
GO

CREATE TABLE [dbo].[PersonPhoto] (
    [PersonPhotoId] [int] IDENTITY(1,1) NOT NULL,
    [PersonId] [int] NOT NULL,
    [PhotoId] [int] NOT NULL,
    [InsertedDateTime] [datetime2] NOT NULL,
    CONSTRAINT [PK_PersonPhoto] PRIMARY KEY CLUSTERED 
    (
        [PersonPhotoId] ASC
    ),
    CONSTRAINT [FK_PersonPhoto_Person]
        FOREIGN KEY ([PersonId])
        REFERENCES [dbo].[Person],
    CONSTRAINT [FK_PersonPhoto_Photo]
        FOREIGN KEY ([PhotoId])
        REFERENCES [dbo].[Photo],
);
GO

CREATE TABLE [dbo].[Show] (
    [ShowId] [int] IDENTITY(1,1) NOT NULL,
    [Title] [nvarchar](150) NOT NULL,
    [Author] [nvarchar](150) NOT NULL,
    [Quarter] tinyint NOT NULL,
    [Year] smallint NOT NULL,
    [Pictures] [nvarchar](100) NOT NULL,
    [FunFacts] [nvarchar](max) NOT NULL,
    [Toaster] [nvarchar](max) NOT NULL,
    [PhotoId] [int] NOT NULL CONSTRAINT DF_Show_PhotoId DEFAULT 1,
    [InsertedDateTime] [datetime2] NOT NULL,
    [LastModifiedDateTime] [datetime2] NOT NULL,
    CONSTRAINT [PK_Show] PRIMARY KEY CLUSTERED 
    (
        [ShowId] ASC
    ),
    CONSTRAINT [FK_Show_Photo]
        FOREIGN KEY ([PhotoId])
        REFERENCES [dbo].[Photo],
);
GO

CREATE TABLE [dbo].[ShowPhoto] (
    [ShowPhotoId] [int] IDENTITY(1,1) NOT NULL,
    [ShowId] [int] NOT NULL,
    [PhotoId] [int] NOT NULL,
    [InsertedDateTime] [datetime2] NOT NULL,
    CONSTRAINT [PK_ShowPhoto] PRIMARY KEY CLUSTERED 
    (
        [ShowPhotoId] ASC
    ),
    CONSTRAINT [FK_ShowPhoto_Show]
        FOREIGN KEY ([ShowId])
        REFERENCES [dbo].[Show],
    CONSTRAINT [FK_ShowPhoto_Photo]
        FOREIGN KEY ([PhotoId])
        REFERENCES [dbo].[Photo],
);
GO

CREATE TABLE [dbo].[AwardType] (
    [AwardTypeId] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](50) NOT NULL,
    CONSTRAINT [PK_AwardType] PRIMARY KEY CLUSTERED 
    (
        [AwardTypeId] ASC
    ),
);
GO

CREATE TABLE [dbo].[Award] (
    [AwardId] [int] IDENTITY(1,1) NOT NULL,
    [ShowId] [int] NULL,
    [PersonId] [int] NULL,
    [AwardTypeId] [int] NOT NULL,
    [Year] smallint NOT NULL,
    [InsertedDateTime] [datetime2] NOT NULL,
    [LastModifiedDateTime] [datetime2] NOT NULL,
    CONSTRAINT [PK_Award] PRIMARY KEY CLUSTERED 
    (
        [AwardId] ASC
    ),
    CONSTRAINT [FK_Award_AwardType]
        FOREIGN KEY ([AwardTypeId])
        REFERENCES [dbo].[AwardType],
    CONSTRAINT [FK_Award_Show]
        FOREIGN KEY ([ShowId])
        REFERENCES [dbo].[Show],
    CONSTRAINT [FK_Award_Person]
        FOREIGN KEY ([PersonId])
        REFERENCES [dbo].[Person],
    CONSTRAINT CHK_ShowOrPerson
        CHECK (ShowId IS NOT NULL OR PersonId IS NOT NULL)
);
GO

CREATE TABLE [dbo].[ShowCast] (
    [ShowCastId] [int] IDENTITY(1,1) NOT NULL,
    [ShowId] [int] NOT NULL,
    [PersonId] [int] NOT NULL,
    [Role] [nvarchar](75) NOT NULL,
    [InsertedDateTime] [datetime2] NOT NULL,
    [LastModifiedDateTime] [datetime2] NOT NULL,
    CONSTRAINT [PK_ShowCast] PRIMARY KEY CLUSTERED 
    (
        [ShowCastId] ASC
    ),
    CONSTRAINT [FK_ShowCast_Show]
        FOREIGN KEY ([ShowId])
        REFERENCES [dbo].[Show],
    CONSTRAINT [FK_ShowCast_Person]
        FOREIGN KEY ([PersonId])
        REFERENCES [dbo].[Person],
);
GO

CREATE TABLE [dbo].[ShowCrew] (
    [ShowCrewId] [int] IDENTITY(1,1) NOT NULL,
    [ShowId] [int] NOT NULL,
    [PersonId] [int] NOT NULL,
    [DisplayOrder] [int] NOT NULL,
    [Position] [nvarchar](75) NOT NULL,
    [InsertedDateTime] [datetime2] NOT NULL,
    [LastModifiedDateTime] [datetime2] NOT NULL,
    CONSTRAINT [PK_ShowCrew] PRIMARY KEY CLUSTERED 
    (
        [ShowCrewId] ASC
    ),
    CONSTRAINT [FK_ShowCrew_Show]
        FOREIGN KEY ([ShowId])
        REFERENCES [dbo].[Show],
    CONSTRAINT [FK_ShowCrew_Person]
        FOREIGN KEY ([PersonId])
        REFERENCES [dbo].[Person],
);
GO

CREATE TABLE [dbo].[PersonClubPosition] (
    [PersonClubPositionId] [int] IDENTITY(1,1) NOT NULL,
    [PersonId] [int] NOT NULL,
    [Position] [nvarchar](75) NOT NULL,
    [DisplayOrder] [int] NOT NULL,
    [Year] smallint NOT NULL,
    [InsertedDateTime] [datetime2] NOT NULL,
    [LastModifiedDateTime] [datetime2] NOT NULL,
    CONSTRAINT [PK_PersonClubPosition] PRIMARY KEY CLUSTERED 
    (
        [PersonClubPositionId] ASC
    ),
    CONSTRAINT [FK_PersonClubPosition_Person]
        FOREIGN KEY ([PersonId])
        REFERENCES [dbo].[Person],
);
GO