CREATE TABLE [dbo].[PostComments]
(
	Id INT NOT NULL PRIMARY KEY IDENTITY,
	ParentId INT NULL,
	[Level] bit DEFAULT(0) NOT NULL,
	PersonId INT NOT NULL,
	CommentContent NVARCHAR(500) NOT NULL,
	IsPublished bit DEFAULT(0) NOT NULL,
	CreatedOn DATETIME2 DEFAULT(GETDATE()) NOT NULL,
	LastModified DATETIME2 NULL,
	PublishedOn DATETIME2 NULL,
	CONSTRAINT FK_PostComments_PostComments_ParentId FOREIGN KEY (ParentId) REFERENCES dbo.PostComments(Id),
	CONSTRAINT FK_PostComments_People_PersonId FOREIGN KEY (PersonId) REFERENCES dbo.People(Id)
)
