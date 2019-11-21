
CREATE PROCEDURE [dbo].[spLog]
(
   @BlindTestId int,
   @ParticipantId int,
   @Vote decimal(18,1),
   @Kilde nvarchar(50) = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	insert into dbo.[Log]([BlindTestId], [ParticipantId], [Vote], [Kilde])
	select @BlindTestId, @ParticipantId, @Vote, @Kilde;
END
