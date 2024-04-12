CREATE TABLE [dbo].[SubCategories]
(
	Id INT NOT NULL PRIMARY KEY IDENTITY,
	PostId INT NOT NULL,
	PostSubCategoryId INT NOT NULL,
	CONSTRAINT FK_SubCategories_Posts_PostId FOREIGN KEY (PostId) REFERENCES dbo.Posts(Id),
	CONSTRAINT FK_SubCategories_SubCategories_PostSubCategoryId FOREIGN KEY (PostSubCategoryId) REFERENCES dbo.PostSubCategories (Id)
)
