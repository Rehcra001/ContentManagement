CREATE TABLE [dbo].[VisualContent]
(
	Id INT NOT NULL PRIMARY KEY IDENTITY,
	PostId INT NOT NULL,
	PostVisualContentId INT NOT NULL,
	CONSTRAINT FK_VisualContent_Posts_PostId FOREIGN KEY (PostId) REFERENCES dbo.Posts(Id),
	CONSTRAINT FK_VisualContent_PostVisualContentId FOREIGN KEY (PostVisualContentId) REFERENCES dbo.PostVisualContent(Id)
)
