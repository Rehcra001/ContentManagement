﻿CREATE TABLE [dbo].[SubCategories]
(
	Id INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(100) NOT NULL,
	[Description] NVARCHAR(250) NOT NULL,
	IsPublished bit DEFAULT(0) NOT NULL,
	CreatedOn DATETIME2 DEFAULT(GETDATE()) NOT NULL,
	LastModified DATETIME2 NULL,
	PublishedOn DATETIME2 NULL
)
