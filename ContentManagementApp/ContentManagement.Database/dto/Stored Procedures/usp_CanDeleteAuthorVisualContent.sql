CREATE PROCEDURE [dbo].[usp_CanDeleteAuthorVisualContent]
(
	@AuthorVisualContentId INT
)AS
BEGIN
	DECLARE @CanDelete BIT;
	IF EXISTS (SELECT * 
			   FROM dbo.PostVisualContent 
			   WHERE AuthorVisualContentId = @AuthorVisualContentId)
		BEGIN
			SET @CanDelete = 0;
		END;
	ELSE
		BEGIN
			SET @CanDelete = 1;
		END;
	SELECT @CanDelete AS CanDelete;
END;
GO
