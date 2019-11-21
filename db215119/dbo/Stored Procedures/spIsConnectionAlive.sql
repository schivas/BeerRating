
CREATE PROCEDURE [dbo].[spIsConnectionAlive]
(
   @BlindTestId int,
   @ConnectionId nvarchar(100)
)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @id int = -1;
	DECLARE @ErrorMsg nvarchar(100) = '';
	IF (SELECT COUNT(1) from [dbo].[Participant] where [BlindTestId] = @BlindTestId AND [ConnectionId] = @ConnectionId) > 0
	BEGIN
		SET @id = (SELECT TOP(1) [Id] from [dbo].[Participant] where [BlindTestId] = @BlindTestId AND [ConnectionId] = @ConnectionId);
	END
	ELSE
	BEGIN
		SET @ErrorMsg = 'Fant ingen "hengende" tilkoblinger';
	END;
	SELECT Result = @id, Error = @ErrorMsg;
END
