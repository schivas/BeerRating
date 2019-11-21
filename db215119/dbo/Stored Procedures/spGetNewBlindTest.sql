

CREATE PROCEDURE [dbo].[spGetNewBlindTest](@BlindTestName nvarchar(100))
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO [dbo].[BlindTest] ([Name]) 
	OUTPUT INSERTED.Id
	VALUES (ISNULL(@BlindTestName, '')); 
END
