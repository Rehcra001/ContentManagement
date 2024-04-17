CREATE TABLE [dbo].[PostVisualContent]
(
	Id INT NOT NULL PRIMARY KEY IDENTITY,
	PostId INT NOT NULL,
	AuthorVisualContentId INT NOT NULL,
	CONSTRAINT FK_PostVisualContent_Posts_PostId FOREIGN KEY (PostId) REFERENCES dbo.Posts(Id),
	CONSTRAINT FK_PostVisualContent_AuthorVisualContent_AuthorVisualContentId FOREIGN KEY (AuthorVisualContentId) REFERENCES dbo.AuthorVisualContent (Id)
)
