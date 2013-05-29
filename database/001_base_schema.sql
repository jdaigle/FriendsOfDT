CREATE TABLE [Person] (
	[PersonId] [int] IDENTITY(1,1) NOT NULL,
	[Honorific] [varchar](50) NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[MiddleName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[Suffix] [varchar](50) NOT NULL,
	[Nickname] [varchar](100) NOT NULL,
	[Biography] [varchar](max) NOT NULL,
	[MediaId] [int] NOT NULL CONSTRAINT DF_Person_MediaId DEFAULT 1,
	[InsertedDateTime] [datetime2] NOT NULL,
	[LastModifiedDateTime] [datetime2] NOT NULL,
	CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
    (
	    [PersonId] ASC
    ),
 -- `username` varchar(25) DEFAULT NULL,
 -- `password` varchar(32) DEFAULT NULL,
 -- `email` varchar(50) DEFAULT NULL,
 -- `level` varchar(50) DEFAULT NULL,
);
GO

CREATE TABLE [Show] (
	[ShowId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](150) NOT NULL,
	[Author] [varchar](150) NOT NULL,
	[Quarter] tinyint NOT NULL,
	[Year] smallint NOT NULL,
	[Pictures] [varchar](100) NOT NULL,
	[FunFacts] [varchar](max) NOT NULL,
	[Toaster] [varchar](max) NOT NULL,
	[MediaId] [int] NOT NULL CONSTRAINT DF_Show_MediaId DEFAULT 1,
	[InsertedDateTime] [datetime2] NOT NULL,
	[LastModifiedDateTime] [datetime2] NOT NULL,
	CONSTRAINT [PK_Show] PRIMARY KEY CLUSTERED 
    (
	    [ShowId] ASC
    ),
);
GO

CREATE TABLE [Award] (
	[AwardId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	CONSTRAINT [PK_Award] PRIMARY KEY CLUSTERED 
    (
	    [AwardId] ASC
    ),
);
GO

CREATE TABLE [ShowAward] (
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

CREATE TABLE [PersonAward] (
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

CREATE TABLE [ShowCast] (
	[ShowCastId] [int] IDENTITY(1,1) NOT NULL,
	[ShowId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[Role] [varchar](75) NOT NULL,
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

CREATE TABLE [ShowCrew] (
	[ShowCrewId] [int] IDENTITY(1,1) NOT NULL,
	[ShowId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[Position] [varchar](75) NOT NULL,
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

CREATE TABLE [PersonClubPosition] (
	[PersonClubPositionId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[Position] [varchar](75) NOT NULL,
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