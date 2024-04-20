CREATE PROCEDURE [dbo].[usp_GetPerson]
(
	@UserName NVARCHAR(100)
)AS
BEGIN
	SELECT Id, UserName, DisplayName
	FROM dbo.People
	WHERE UserName = @UserName;
END;
GO