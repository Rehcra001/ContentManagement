CREATE PROCEDURE [dbo].[usp_AddSubCategory]
(
	@Name NVARCHAR(100),
	@Description NVARCHAR(250),
	@IsPublished BIT,
	@CreatedOn DATETIME2,
	@LastModifiedOn DATETIME2,
	@PublishedOn DATETIME2
)AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
			DECLARE @Id INT;

			INSERT INTO dbo.SubCategories([Name], [Description], IsPublished, CreatedOn, LastModified, PublishedOn)
			VALUES (
					@Name, @Description, @IsPublished, @CreatedOn, @LastModifiedOn, @PublishedOn
				   );
			SET @Id = SCOPE_IDENTITY();

			SELECT Id, [Name], [Description], IsPublished, CreatedOn, LastModified, PublishedOn
			FROM dbo.SubCategories
			WHERE Id = @Id;
		COMMIT TRAN
	END TRY

	BEGIN CATCH
		IF (@@TRANCOUNT > 0)
			BEGIN
				ROLLBACK TRAN;
			END;
	END CATCH;
END;
GO
