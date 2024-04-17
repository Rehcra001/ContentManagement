CREATE TABLE [dbo].[AuthorVisualContent]
(
	Id INT NOT NULL PRIMARY KEY IDENTITY,
	AuthorId INT NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	[Description] NVARCHAR(250) NOT NULL,
	ServerLocation NVARCHAR(250) NOT NULL,
	LocalLocation NVARCHAR(250) NOT NULL,
	CONSTRAINT FK_AuthorVisualContent_People_AuthorId FOREIGN KEY (AuthorId) REFERENCES dbo.People(Id)
)
