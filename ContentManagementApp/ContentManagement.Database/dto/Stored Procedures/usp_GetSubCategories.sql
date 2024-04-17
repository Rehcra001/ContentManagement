CREATE PROCEDURE [dbo].[usp_GetSubCategories] AS
BEGIN
	SELECT Id, [Name], [Description], IsPublished, CreatedOn, LastModified, PublishedOn
	FROM dbo.SubCategories;
END;
GO