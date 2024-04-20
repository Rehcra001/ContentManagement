CREATE PROCEDURE [dbo].[usp_GetAllAuthorVisualContent]
(
	@AuthorId INT
)AS
BEGIN
	SELECT Id, AuthorId, [Name], [Description], [FileName], VisualContentType, IsHttpLink
	FROM dbo.AuthorVisualContent
	WHERE AuthorId = @AuthorId;
END;
GO
