CREATE PROCEDURE [dbo].[usp_GetPersonIdFromUserName]
(
	@UserName NVARCHAR(100)
)AS
BEGIN
	SELECT Id
	FROM dbo.People
	WHERE UserName = @UserName;
END;
GO
