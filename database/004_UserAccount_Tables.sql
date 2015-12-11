IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserAccount]'))
CREATE TABLE [dbo].[UserAccount] (
    [UserAccountId] [int] IDENTITY(1,1) NOT NULL,
    [Email] [nvarchar](300) NOT NULL,
    [Name] [nvarchar](300) NOT NULL,
    [FacebookId] [int] NULL,
    [InsertedDateTime] [datetime2](7) NOT NULL,
    [LastModifiedDateTime] [datetime2](7) NOT NULL,
    CONSTRAINT [PK_UserAccount] PRIMARY KEY CLUSTERED 
    (
        [UserAccountId] ASC
    ),
);
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserFacebookAccessToken]'))
CREATE TABLE [dbo].[UserFacebookAccessToken] (
    [UserFacebookAccessTokenId] [int] IDENTITY(1,1) NOT NULL,
    [AccessToken] [varchar] (255) NOT NULL,
    [UserAccountId] [int] NOT NULL,
    [InsertedDateTime] [datetime2](7) NOT NULL,
    [ExpiresDateTime] [datetime2](7) NOT NULL,
    CONSTRAINT [PK_UserFacebookAccessToken] PRIMARY KEY CLUSTERED 
    (
        [UserFacebookAccessTokenId] ASC
    ),
    CONSTRAINT [FK_UserFacebookAccessToken_UserAccount]
        FOREIGN KEY ([UserAccountId])
        REFERENCES [dbo].[UserAccount] ([UserAccountId]),
);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_UserFacebookAccessToken_AccessToken' AND object_id = OBJECT_ID('dbo.[UserFacebookAccessToken]'))
    CREATE UNIQUE NONCLUSTERED INDEX UX_UserFacebookAccessToken_AccessToken ON dbo.[UserFacebookAccessToken] ([AccessToken]);