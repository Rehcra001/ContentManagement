CREATE PROCEDURE [dbo].[usp_GetCategories] AS
BEGIN
	SELECT Id, [Name], [Description], IsPublished, CreatedOn, LastModified, PublishedOn
	FROM dbo.Categories;
END;
GO
