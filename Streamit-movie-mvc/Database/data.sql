CREATE DATABASE StreamingMovie;

USE StreamingMovie;

CREATE TABLE [dbo].[User](
	[UserID] [uniqueidentifier] PRIMARY KEY,
	[UserName] [varchar](255) Unique,
	[Password] [VARBINARY](MAX) NOT NULL,
	[PasswordSalt] [VARBINARY](MAX) NOT NULL,
	[Role] [varchar](255) NOT NULL,
	[Status] [varchar](255) NULL,
	[Email] [varchar](255) NULL,
	[DisplayName] [varchar](255) null,
	[Avatar] [varchar](max) null,
	[DateCreated] [date] default GETDATE()
);

CREATE TABLE [dbo].[Category](
	[CategoryID] [int] IDENTITY(1,1) PRIMARY KEY,
	[Name] [nvarchar](255) NULL
);

CREATE TABLE [dbo].[Nation](
	[NationID] [varchar](255) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	PRIMARY KEY ([NationID])
);

CREATE TABLE [dbo].[FeatureFilm](
	[FeatureId] [int] IDENTITY(1,1) PRIMARY KEY,
	[Name] [nvarchar](255) NULL,
)

CREATE TABLE [dbo].[Person](
	[PersonID] [uniqueidentifier] PRIMARY KEY,
	[Image] [varchar](255) NULL,
	[NamePerson] [nvarchar](255) NULL,
	[NationID] [varchar](255) REFERENCES [dbo].[Nation]([NationID]),
	[Role] [varchar](255) NOT NULL,
	[DoB] [date] NULL,
	[DateCreated] [datetime] NULL
);

CREATE TABLE [dbo].[Movies](
	[MovieID] [uniqueidentifier] PRIMARY KEY,
	[FeatureId] [int] REFERENCES [dbo].[featurefilm]([FeatureId]),
	[NationID] [varchar](255) REFERENCES [dbo].[Nation]([NationID]),
	[Mark] [float] NULL,
	[Time] [int] NULL,
	[Viewer] [int] NULL,
	[Description] [nvarchar](max) NULL,
	[EnglishName] [varchar](255) NULL,
	[VietnamName] [nvarchar](255) NULL,
	[Thumbnail] [varchar](max) NULL,
	[Trailer] [varchar](255) NULL,
	[Status] [varchar](255) NULL,
	[ProducedDate] [date] NULL,
	[DateCreated] [datetime] NULL,
	[DateUpdated] [datetime] NULL,
	[DateDeleted] [datetime] NULL
);

CREATE TABLE [dbo].[MovieCategory](
	[CategoryID] [int] REFERENCES [dbo].[Category]([CategoryID]),
	[MovieID] [uniqueidentifier] REFERENCES [dbo].[Movies]([MovieID]),
);

CREATE TABLE [dbo].[Cast](
	[PersonID] [uniqueidentifier] NOT NULL,
	[MovieID] [uniqueidentifier] NOT NULL,
	[CharacterName] [nvarchar](255) NOT NULL,
	PRIMARY KEY ([PersonID], [MovieID]),
	FOREIGN KEY ([PersonID]) REFERENCES [dbo].[Person]([PersonID]),
	FOREIGN KEY ([MovieID]) REFERENCES [dbo].[Movies]([MovieID])
);

CREATE TABLE [dbo].[Season](
	[SeasonID] [uniqueidentifier] PRIMARY KEY,
	[MovieID] [uniqueidentifier] NOT NULL,
	[SeasonNumber] [int] NOT NULL,
	[Name] [nvarchar](255) NULL,
	FOREIGN KEY ([MovieID]) REFERENCES [dbo].[Movies]([MovieID]),
	[Status] [varchar](255) NULL
);

CREATE TABLE [dbo].[Episode](
	[EpisodeID] [uniqueidentifier] PRIMARY KEY,
	[SeasonID] [uniqueidentifier] REFERENCES [dbo].[Season]([SeasonID]),
	[EpisodeNumber] [int] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Video] [nvarchar](Max) NULL,
	[Status] [varchar](255) NULL,
	[DateCreated] [datetime] default GETDATE(),
	[DateUpdated] [datetime] NULL
);

