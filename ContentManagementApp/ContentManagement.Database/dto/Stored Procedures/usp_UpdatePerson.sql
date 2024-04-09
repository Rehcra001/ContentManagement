CREATE PROCEDURE [dbo].[usp_UpdatePerson]
(
	@UserName NVARCHAR(100),
	@DisplayName NVARCHAR(50)
)AS
BEGIN
	DECLARE @Updated BIT;
	BEGIN TRY
		BEGIN TRAN
			SET NOCOUNT ON;			

			UPDATE dbo.People
			SET DisplayName = @DisplayName
			WHERE UserName = @UserName;

			SET @Updated = 1;

			SELECT @Updated AS Updated;
		COMMIT TRAN;
	END TRY

	BEGIN CATCH
		SET @Updated = 0;
		IF (@@TRANCOUNT > 0)
		BEGIN
			ROLLBACK TRAN;
		END;

		SELECT @Updated AS Updated;
	END CATCH;
END;
GO
