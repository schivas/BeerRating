
CREATE PROCEDURE [dbo].[spRemoveConnection]
(
   @ConnectionId nvarchar(100)
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @owners nvarchar(600) = '';
	SELECT @owners = @owners + IIF(LEN(@owners) > 0, ', ', '') + [Name]
	FROM [dbo].[Participant]
	WHERE [ConnectionId] = @ConnectionId;

	UPDATE [dbo].[Participant]
	SET [ConnectionId] = NULL, [Updated] = GETDATE(), [UpdateReason] = 'Removed'
	WHERE [ConnectionId] = @ConnectionId;
	-- Her er egentlig ikke @owners en feil, bruker likevel dette grensesnittet til å overføre data til applikasjonen
	SELECT Result = @owners, Error = '';
END
