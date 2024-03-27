CREATE PROCEDURE [dbo].[usp_PersonExists]
(
	@UserName NVARCHAR(100)
)AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Exists BIT = 0;
	IF EXISTS (SELECT * FROM dbo.People WHERE UserName = @UserName)
		BEGIN
			SET @Exists = 1;
			SELECT @Exists AS [Exists];
		END
	ELSE
		BEGIN
			SELECT @Exists AS [Exists];
		END;
END;
GO