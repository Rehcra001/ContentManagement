CREATE PROCEDURE [dbo].[usp_UpdateAuthorVisualContent]
(
	@Id INT,
	@AuthorId INT,
	@Name NVARCHAR(100),
	@Description NVARCHAR(250),
	@FileName NVARCHAR(2000),
	@VisualContentType NVARCHAR(50),
	@IsHttpLink BIT
)AS
BEGIN
	DECLARE @Updated BIT;
	BEGIN TRY
		BEGIN TRAN
			SET NOCOUNT ON;
			SET @Updated = 1;

			UPDATE dbo.AuthorVisualContent
			SET [Name] = @Name,
				[Description] = @Description,
				[FileName] = @FileName,
				VisualContentType = @VisualContentType,
				IsHttpLink = @IsHttpLink
			WHERE Id = @id;

			SELECT @Updated AS Updated;
		COMMIT TRAN;
	END TRY

	BEGIN CATCH
		IF (@@TRANCOUNT > 0)
			BEGIN
				ROLLBACK TRAN;
			END;
		SET @Updated = 0;
		SELECT @Updated AS Updated;
	END CATCH;
END;
GO
