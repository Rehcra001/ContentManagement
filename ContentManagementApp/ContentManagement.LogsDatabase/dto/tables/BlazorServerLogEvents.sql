﻿CREATE TABLE [dbo].[BlazorServerLogEvents]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[Message] NVARCHAR(MAX) NULl,
	[MessageTemplate] NVARCHAR(MAX) NULL,
	[Level] NVARCHAR(128) NULL,
	[TimeStamp] DATETIME2 NOT NULL,
	[Exception] NVARCHAR(MAX) NULL,
	[Properties] NVARCHAR(MAX) NULL
)
