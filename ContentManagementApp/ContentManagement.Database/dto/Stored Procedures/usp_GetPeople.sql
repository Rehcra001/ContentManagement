CREATE PROCEDURE [dbo].[usp_GetPeople] AS
BEGIN
	SET NOCOUNT ON;

	SELECT Id, UserName, DisplayName
	FROM dbo.People;
END;
GO
