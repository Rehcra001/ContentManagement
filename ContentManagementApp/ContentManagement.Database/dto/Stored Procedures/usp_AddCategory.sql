CREATE PROCEDURE [dbo].[usp_AddCategory]
(
	@Name NVARCHAR(50),
	@Description NVARCHAR(250),
	@IsPublished BIT,
	@CreatedOn DATETIME2,
	@LastModified DATETIME2,
	@PublishedOn DATETIME2
)AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
			SET NOCOUNT ON;

			DECLARE @Id INT;

			INSERT INTO dbo.Categories([Name], [Description], IsPublished, CreatedOn, LastModified, PublishedOn)
			VALUES (@Name,
					@Description,
					@IsPublished,
					@CreatedOn,
					@LastModified,
					@PublishedOn);

			SET @Id = SCOPE_IDENTITY();

			SELECT Id, [Name], [Description], IsPublished, CreatedOn, LastModified, PublishedOn
			FROM dbo.Categories
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
