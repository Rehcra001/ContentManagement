CREATE PROCEDURE [dbo].[usp_UpdateSubCategory]
(
	@Id INT,
	@Name NVARCHAR(50),
	@Description NVARCHAR(250),
	@IsPublished BIT,
	@CreatedOn DATETIME2,
	@LastModified DATETIME2,
	@PublishedOn DATETIME2
)AS
BEGIN
	DECLARE @Updated BIT;
	BEGIN TRY
		BEGIN TRAN
			SET NOCOUNT ON;
			SET @Updated = 1;

			UPDATE dbo.SubCategories
			SET [Name] = @Name,
				[Description] = @Description,
				IsPublished = @IsPublished,
				CreatedOn = @CreatedOn,
				LastModified = @LastModified,
				PublishedOn = @PublishedOn
			WHERE Id = @Id;

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