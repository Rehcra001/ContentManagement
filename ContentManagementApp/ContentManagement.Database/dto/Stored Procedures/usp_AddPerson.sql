CREATE PROCEDURE [dbo].[usp_AddPerson]
(
	@UserName NVARCHAR(100),
	@DisplayName NVARCHAR(50),
	@Role NVARCHAR(20)
)AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
			SET NOCOUNT ON;

			--Add a new person
			IF NOT EXISTS (SELECT * FROM dbo.People WHERE UserName = @UserName)
			BEGIN
				DECLARE @Id INT;
				
				INSERT INTO dbo.People (UserName, DisplayName, Role)
				VALUES (@UserName, @DisplayName, @Role);

				SET @Id = SCOPE_IDENTITY();

				SELECT Id, UserName, DisplayName, [Role]
				FROM dbo.People
				WHERE Id = @Id;
			END

			ELSE
			BEGIN
				DECLARE @MessageText NVARCHAR(50);
				SET @MessageText = FORMATMESSAGE(51000, N'Record already exists');
				THROW 51000, @MessageText, 1;
			END;
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
