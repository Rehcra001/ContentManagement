﻿CREATE TABLE [dbo].[People]
(
	ID INT NOT NULL PRIMARY KEY IDENTITY,
	UserName NVARCHAR(100) NOT NULL,
	[Role] NVARCHAR(20) NOT NULL
)