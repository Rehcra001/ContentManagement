CREATE PROCEDURE [dbo].[usp_AddAuthorVisualContent]
(
	@AuthorId INT,
	@Name NVARCHAR(100),
	@Description NVARCHAR(250),
	@FileName NVARCHAR(2000),
	@VisualContentType NVARCHAR(50),
	@IsHttpLink BIT
)AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
			SET NOCOUNT ON;

			DECLARE @Id INT;

			INSERT INTO dbo.AuthorVisualContent (AuthorId, [Name], [Description], [FileName], VisualContentType, IsHttpLink)
			VALUES (@AuthorId, @Name, @Description, @FileName, @VisualContentType, @IsHttpLink);

			SET @Id = SCOPE_IDENTITY();

			SELECT Id, AuthorId, [Name], [Description], [FileName], VisualContentType, IsHttpLink
			FROM dbo.AuthorVisualContent
			WHERE Id = @Id;

		COMMIT TRAN;
	END TRY

	BEGIN CATCH
		IF (@@TRANCOUNT > 0)
			BEGIN
				ROLLBACK TRAN;
			END;
	END CATCH;
END;
GO