CREATE PROCEDURE [dbo].[usp_GetCategory]
(
	@Id INT
)AS
BEGIN
	SELECT Id, [Name], [Description], IsPublished, CreatedOn, LastModified, PublishedOn
	FROM dbo.Categories
	WHERE Id = @Id;
END;
GO
