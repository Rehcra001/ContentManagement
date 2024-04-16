CREATE PROCEDURE [dbo].[usp_CanDeleteCategory]
(
	@CategoryId INT
)AS
BEGIN
	DECLARE @CanDelete bit;

	IF EXISTS (SELECT * FROM dbo.Posts WHERE CategoryId = @CategoryId)
		BEGIN
			--Cannot delete as it is used in a post
			SET @CanDelete = 0;
		END;
	ELSE
		BEGIN
			--Can delete as it is not used in a post
			SET @CanDelete = 1;
		END;

	SELECT @CanDelete AS CanDelete
END;
GO