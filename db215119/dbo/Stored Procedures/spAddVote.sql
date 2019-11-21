CREATE PROCEDURE [dbo].[spAddVote]
(
   @BlindTestId int,
   @ParticipantId int,
   @Vote decimal(18,1)
)
AS
BEGIN
	SET NOCOUNT ON;

	EXEC [dbo].[spLog] @BlindTestId, @ParticipantId, @Vote, N'spAddVote';

	SET @Vote = ISNULL(@Vote, 0);

	DECLARE @user_vote_deleted bit = 0; -- Settes til 1 bruker ble slettet
	DECLARE @inform_others bit = 0;
	DECLARE @status nvarchar(100) = '';
	DECLARE @image_index nvarchar(50);
	IF (@Vote < 0 OR @Vote > 6)
	BEGIN
		SET @status = 'Stemmen ble forkastet (ugyldig)';
	END
	ELSE IF (SELECT COUNT(1) from [dbo].[BlindTest] where [Id] = @BlindTestId) = 0
	BEGIN
		SET @status = 'Stemmen ble forkastet da oppgitt blindtest ikke eksisterer';
	END
	-- LAGT TIL: 2019-11-21
	ELSE IF (SELECT COUNT(1) from [dbo].[BlindTest] where [Id] = @BlindTestId AND [IsCompleted] = 1) <> 0
	BEGIN
		SET @status = 'Stemmen ble forkastet da oppgitt blindtest er fullført.';
	END
	ElSE IF (SELECT COUNT(1) from [dbo].[Participant] where [BlindTestId] = @BlindTestId AND [Id] = @ParticipantId) = 0
	BEGIN
		SET @status = 'Du er ikke registrert som bruker i denne blindtesten';
	END
	ELSE IF (SELECT COUNT(1) from [dbo].[CurrentRound] where [BlindTestId] = @BlindTestId AND [ParticipantId] = @ParticipantId) = 0
	BEGIN
		-- Det er IKKE lagt inn en stemme tidligere - legg inn stemme så sant den er større enn 0
		IF @Vote > 0
		BEGIN
			INSERT INTO [dbo].[CurrentRound] ([BlindTestId], [ParticipantId], [Vote], [Updated])
			VALUES (@BlindTestId, @ParticipantId, @Vote, GETDATE());
			SET @status = 'Din stemme (' + CONVERT(nvarchar(10), @Vote) + ') ble registrert';
			SET @inform_others = 1;
		END
		ELSE
		BEGIN
			SET @status = 'Stemmen ble ignorert';
		END;
	END
	ELSE
	BEGIN
		-- Det eksisterer alt en stemme, foreta en update eller slett eksisterende hvis ny stemme er null
		IF @Vote > 0
		BEGIN
			UPDATE [dbo].[CurrentRound]
			SET Vote = @Vote, Updated = GETDATE()
			WHERE [BlindTestId] = @BlindTestId AND [ParticipantId] = @ParticipantId AND Vote <> @Vote;
			SET @status = 'Din stemme (' + CONVERT(nvarchar(10), @Vote) + ') ble registrert';
		END
		ELSE
		BEGIN
			DELETE FROM [dbo].[CurrentRound]
			WHERE [BlindTestId] = @BlindTestId AND [ParticipantId] = @ParticipantId;
			SET @status = 'Din stemme ble slettet';
			SET @user_vote_deleted = 1;
		END;
		SET @inform_others = 1;
	END;

	IF @inform_others = 1
	BEGIN
		--SET ImageIndex
		SET @image_index = (SELECT TOP(1) ImageIndex FROM [dbo].[Participant] where [BlindTestId] = @BlindTestId AND [Id] = @ParticipantId);
		SET @image_index = ISNULL(@image_index, '01');
	END;
	SELECT BlindTestId = @BlindTestId, ParticipantId = @ParticipantId, ImageIndex = @image_index, Vote = @Vote, InformClient = convert(bit, 1), InformOthers = @inform_others, UserVoteDeleted = @user_vote_deleted, Result = @status;
END
