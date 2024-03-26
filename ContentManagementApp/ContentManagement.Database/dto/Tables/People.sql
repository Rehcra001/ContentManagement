﻿CREATE TABLE [dbo].[People]
(
	ID INT NOT NULL PRIMARY KEY IDENTITY,
	UserName NVARCHAR(100) NOT NULL,
	DisplayName NVARCHAR(50) NOT NULL,
	[Role] NVARCHAR(20) NOT NULL,
	CONSTRAINT UN_People_UserName UNIQUE (UserName),
	CONSTRAINT UN_People_DisplayName UNIQUE (DisplayName)
)
