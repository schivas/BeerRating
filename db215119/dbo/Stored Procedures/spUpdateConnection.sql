CREATE PROCEDURE [dbo].[spUpdateConnection]
(
   @BlindTestId int,
   @ParticipantId int,
   @ConnectionId nvarchar(100)
)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [dbo].[Participant]
	SET [ConnectionId] =  @ConnectionId, [Updated] = GETDATE(), [UpdateReason] = 'Updated'
	WHERE [BlindTestId] = @BlindTestId AND [Id] = @ParticipantId 
	--AND [ConnectionId] <> @ConnectionId;
END
