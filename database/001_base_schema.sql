USE fodt;
GO

CREATE SCHEMA [imdt] AUTHORIZATION [dbo]
GO

CREATE TABLE [imdt].[Person] (
	[PersonId] [int] IDENTITY(1,1) NOT NULL,
	[Honorific] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[MiddleName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Suffix] [nvarchar](50) NOT NULL,
	[Nickname] [nvarchar](100) NOT NULL,
	[Biography] [nvarchar](max) NOT NULL,
	[MediaId] [int] NOT NULL CONSTRAINT DF_Person_MediaId DEFAULT 1,
	[InsertedDateTime] [datetime2] NOT NULL,
	[LastModifiedDateTime] [datetime2] NOT NULL,
	CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
    (
	    [PersonId] ASC
    ),
 -- `username` nvarchar(25) DEFAULT NULL,
 -- `password` nvarchar(32) DEFAULT NULL,
 -- `email` nvarchar(50) DEFAULT NULL,
 -- `level` nvarchar(50) DEFAULT NULL,
);
GO

CREATE TABLE [imdt].[Show] (
	[ShowId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[Author] [nvarchar](150) NOT NULL,
	[Quarter] tinyint NOT NULL,
	[Year] smallint NOT NULL,
	[Pictures] [nvarchar](100) NOT NULL,
	[FunFacts] [nvarchar](max) NOT NULL,
	[Toaster] [nvarchar](max) NOT NULL,
	[MediaId] [int] NOT NULL CONSTRAINT DF_Show_MediaId DEFAULT 1,
	[InsertedDateTime] [datetime2] NOT NULL,
	[LastModifiedDateTime] [datetime2] NOT NULL,
	CONSTRAINT [PK_Show] PRIMARY KEY CLUSTERED 
    (
	    [ShowId] ASC
    ),
);
GO

CREATE TABLE [imdt].[Award] (
	[AwardId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	CONSTRAINT [PK_Award] PRIMARY KEY CLUSTERED 
    (
	    [AwardId] ASC
    ),
);
GO

CREATE TABLE [imdt].[ShowAward] (
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
		REFERENCES [imdt].[Award],
	CONSTRAINT [FK_ShowAward_Show]
		FOREIGN KEY ([ShowId])
		REFERENCES [imdt].[Show],
	CONSTRAINT [FK_ShowAward_Person]
		FOREIGN KEY ([PersonId])
		REFERENCES [imdt].[Person],
);
GO

CREATE TABLE [imdt].[PersonAward] (
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
		REFERENCES [imdt].[Award],
	CONSTRAINT [FK_PersonAward_Person]
		FOREIGN KEY ([PersonId])
		REFERENCES [imdt].[Person],
);
GO

CREATE TABLE [imdt].[ShowCast] (
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
		REFERENCES [imdt].[Show],
	CONSTRAINT [FK_ShowCast_Person]
		FOREIGN KEY ([PersonId])
		REFERENCES [imdt].[Person],
);
GO

CREATE TABLE [imdt].[ShowCrew] (
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
		REFERENCES [imdt].[Show],
	CONSTRAINT [FK_ShowCrew_Person]
		FOREIGN KEY ([PersonId])
		REFERENCES [imdt].[Person],
);
GO

CREATE TABLE [imdt].[PersonClubPosition] (
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
		REFERENCES [imdt].[Person],
);
GO