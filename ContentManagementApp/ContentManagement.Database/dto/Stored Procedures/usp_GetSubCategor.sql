CREATE PROCEDURE [dbo].[usp_GetSubCategory]
(
	@Id INT
)AS
BEGIN
	SELECT Id, [Name], [Description], IsPublished, CreatedOn, LastModified, PublishedOn
	FROM dbo.SubCategories
	WHERE Id = @Id;
END;
GO