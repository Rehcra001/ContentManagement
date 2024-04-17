CREATE TABLE [dbo].[PostSubCategories]
(
	Id INT NOT NULL PRIMARY KEY IDENTITY,
	PostId INT NOT NULL,
	SubCategoryId INT NOT NULL,
	CONSTRAINT FK_PostSubCategories_Posts_PostId FOREIGN KEY (PostId) REFERENCES dbo.Posts(Id),
	CONSTRAINT FK_PostSubCategories_SubCategories_SubCategoryId FOREIGN KEY (SubCategoryId) REFERENCES dbo.SubCategories (Id)
)
