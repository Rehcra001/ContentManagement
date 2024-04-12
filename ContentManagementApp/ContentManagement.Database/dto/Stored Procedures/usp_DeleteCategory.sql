CREATE PROCEDURE [dbo].[usp_DeleteCategory]
(
	@Id INT
)AS
BEGIN
	DECLARE @Deleted BIT;
	BEGIN TRY
		BEGIN TRAN
			SET NOCOUNT ON;
			SET @Deleted = 1;

			DELETE FROM dbo.Categories
			WHERE Id = @Id;

			SELECT @Deleted AS Deleted;
		COMMIT TRAN;
	END TRY

	BEGIN CATCH
		IF (@@TRANCOUNT > 0)
		BEGIN
			ROLLBACK TRAN;
		END;

		SET @Deleted = 0;
		SELECT @Deleted AS Deleted;
	END CATCH;
END;
GO
