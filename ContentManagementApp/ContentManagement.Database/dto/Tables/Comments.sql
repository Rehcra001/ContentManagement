CREATE TABLE [dbo].[Comments]
(
	Id INT NOT NULL PRIMARY KEY IDENTITY,
	PostId INT NOT NULL,
	PostCommentId INT NOT NULL,
	CONSTRAINT FK_Comments_Posts_PostId FOREIGN KEY (PostId) REFERENCES dbo.Posts(Id),
	CONSTRAINT FK_Commnents_PostComments_PostCommentsId FOREIGN KEY (PostCommentId) REFERENCES dbo.PostComments(Id)
)
