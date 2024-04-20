CREATE PROCEDURE [dbo].[usp_GetAuthorVisualContent]
(
	@Id INT
)AS
BEGIN
	SELECT Id, AuthorId, [Name], [Description], [FileName], VisualContentType, IsHttpLink
	FROM dbo.AuthorVisualContent
	WHERE Id = @Id;
END;
GO
