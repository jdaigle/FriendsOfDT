USE fodt;
GO

CREATE TABLE [dbo].[MediaItem] (
	[MediaItemId] [int] IDENTITY(1,1) NOT NULL,
	[Path] [varchar](100) NOT NULL,
	[ThumbnailPath] [varchar](100) NOT NULL,
	[TinyPath] [varchar](100) NOT NULL,
	[InsertedDateTime] [datetime2] NOT NULL,
	CONSTRAINT [PK_MediaItem] PRIMARY KEY CLUSTERED 
    (
	    [MediaItemId] ASC
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
	[MediaItemId] [int] NOT NULL CONSTRAINT DF_Person_MediaItemId DEFAULT 1,
	[InsertedDateTime] [datetime2] NOT NULL,
	[LastModifiedDateTime] [datetime2] NOT NULL,
	CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
    (
	    [PersonId] ASC
    ),
    CONSTRAINT [FK_Person_MediaItem]
		FOREIGN KEY ([MediaItemId])
		REFERENCES [dbo].[MediaItem],
 -- `username` nvarchar(25) DEFAULT NULL,
 -- `password` nvarchar(32) DEFAULT NULL,
 -- `email` nvarchar(50) DEFAULT NULL,
 -- `level` nvarchar(50) DEFAULT NULL,
);
GO

CREATE TABLE [dbo].[PersonMedia] (
	[PersonMediaId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[MediaItemId] [int] NOT NULL,
	[InsertedDateTime] [datetime2] NOT NULL,
	CONSTRAINT [PK_PersonMedia] PRIMARY KEY CLUSTERED 
    (
	    [PersonMediaId] ASC
    ),
    CONSTRAINT [FK_PersonMedia_Person]
		FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[Person],
    CONSTRAINT [FK_PersonMedia_MediaItem]
		FOREIGN KEY ([MediaItemId])
		REFERENCES [dbo].[MediaItem],
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
	[MediaItemId] [int] NOT NULL CONSTRAINT DF_Show_MediaItemId DEFAULT 1,
	[InsertedDateTime] [datetime2] NOT NULL,
	[LastModifiedDateTime] [datetime2] NOT NULL,
	CONSTRAINT [PK_Show] PRIMARY KEY CLUSTERED 
    (
	    [ShowId] ASC
    ),
    CONSTRAINT [FK_Show_MediaItem]
		FOREIGN KEY ([MediaItemId])
		REFERENCES [dbo].[MediaItem],
);
GO

CREATE TABLE [dbo].[ShowMedia] (
	[ShowMediaId] [int] IDENTITY(1,1) NOT NULL,
	[ShowId] [int] NOT NULL,
	[MediaItemId] [int] NOT NULL,
	[InsertedDateTime] [datetime2] NOT NULL,
	CONSTRAINT [PK_ShowMedia] PRIMARY KEY CLUSTERED 
    (
	    [ShowMediaId] ASC
    ),
    CONSTRAINT [FK_ShowMedia_Show]
		FOREIGN KEY ([ShowId])
		REFERENCES [dbo].[Show],
    CONSTRAINT [FK_ShowMedia_MediaItem]
		FOREIGN KEY ([MediaItemId])
		REFERENCES [dbo].[MediaItem],
);
GO

CREATE TABLE [dbo].[Award] (
	[AwardId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	CONSTRAINT [PK_Award] PRIMARY KEY CLUSTERED 
    (
	    [AwardId] ASC
    ),
);
GO

CREATE TABLE [dbo].[ShowAward] (
	[ShowAwardId] [int] IDENTITY(1,1) NOT NULL,
	[ShowId] [int] NOT NULL,
	[PersonId] [int] NULL,
	[AwardId] [int] NOT NULL,
	[Year] smallint NOT NULL,
	[InsertedDateTime] [datetime2] NOT NULL,
	[LastModifiedDateTime] [datetime2] NOT NULL,
	CONSTRAINT [PK_ShowAward] PRIMARY KEY CLUSTERED 
    (
	    [ShowAwardId] ASC
    ),
	CONSTRAINT [FK_ShowAward_Award]
		FOREIGN KEY ([AwardId])
		REFERENCES [dbo].[Award],
	CONSTRAINT [FK_ShowAward_Show]
		FOREIGN KEY ([ShowId])
		REFERENCES [dbo].[Show],
	CONSTRAINT [FK_ShowAward_Person]
		FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[Person],
);
GO

CREATE TABLE [dbo].[PersonAward] (
	[PersonAwardId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[AwardId] [int] NOT NULL,
	[Year] smallint NOT NULL,
	[InsertedDateTime] [datetime2] NOT NULL,
	[LastModifiedDateTime] [datetime2] NOT NULL,
	CONSTRAINT [PK_PersonAward] PRIMARY KEY CLUSTERED 
    (
	    [PersonAwardId] ASC
    ),
	CONSTRAINT [FK_PersonAward_Award]
		FOREIGN KEY ([AwardId])
		REFERENCES [dbo].[Award],
	CONSTRAINT [FK_PersonAward_Person]
		FOREIGN KEY ([PersonId])
		REFERENCES [dbo].[Person],
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

CREATE TABLE [dbo].[UserAccount] (
	[UserAccountId] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](300) NOT NULL,
	[Name] [nvarchar](300) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Gender] [nvarchar](10) NOT NULL,
	[Locale] [nvarchar](10) NOT NULL,
    [FacebookId] [varchar](50) NOT NULL,
	[FacebookURL] [nvarchar](300) NOT NULL,
	[FacebookUsername] [nvarchar](300) NOT NULL,
	[FacebookPictureURL] [nvarchar](300) NOT NULL,
    [InsertedDateTime] [datetime2] NOT NULL,
	[LastModifiedDateTime] [datetime2] NOT NULL,
	CONSTRAINT [PK_UserAccount] PRIMARY KEY CLUSTERED 
    (
	    [UserAccountId] ASC
    ),
    CONSTRAINT [UC_FacebookId] UNIQUE NONCLUSTERED
    (
        [FacebookId]
    ),
);
GO

CREATE TABLE [dbo].[UserAccessToken] (
	[AccessToken] [varchar](255) NOT NULL,
	[UserAccountId] [int] NOT NULL,
	[InsertedDateTime] [datetime2] NOT NULL,
    [ExpiresDateTime] [datetime2] NOT NULL,
	CONSTRAINT [PK_AccessToken] PRIMARY KEY CLUSTERED 
    (
	    [AccessToken] ASC
    ),
	CONSTRAINT [FK_UserAccessToken_UserAccount]
		FOREIGN KEY ([UserAccountId])
		REFERENCES [dbo].[UserAccount],
);
GO