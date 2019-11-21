CREATE PROCEDURE [dbo].[spAddParticipant]
(
   @BlindTestId int,
   @UserName nvarchar(100),
   @ConnectionId nvarchar(100),
   @ConnectionIdClient nvarchar(100),
   @getExistingUserFromUserName bit = 1
)
AS
BEGIN
	SET NOCOUNT ON;
	--SET @ConnectionId = ISNULL(NULLIF(@ConnectionId, ''), 'not set');
	SET @ConnectionIdClient = ISNULL(NULLIF(@ConnectionIdClient, ''), 'also not set');
	DECLARE @id int = -1;
	DECLARE @ErrorMsg nvarchar(100) = '';
	
	IF NULLIF(@UserName, '') IS NULL
	BEGIN
		SET @ErrorMsg = 'Brukernavn kan ikke være blankt.';
	END
	ELSE IF (SELECT COUNT(1) from [dbo].[BlindTest] where [Id] = @BlindTestId) = 0
	BEGIN
		SET @ErrorMsg = 'Blindtest (Id = ' + convert(nvarchar(10), @BlindTestId) + ') eksisterer ikke - kan ikke registrere deltaker!';
	END
	-- LAGT TIL: 2019-11-21
	ELSE IF (SELECT COUNT(1) from [dbo].[BlindTest] where [Id] = @BlindTestId AND [IsCompleted] = 1) <> 0
	BEGIN
		SET @ErrorMsg = 'Blindtest (Id = ' + convert(nvarchar(10), @BlindTestId) + ') er fullført - kan ikke registrere deltaker!';
	END
	ELSE IF ((@ConnectionId = @ConnectionIdClient) AND ((SELECT COUNT(1) from [dbo].[Participant] where [BlindTestId] = @BlindTestId AND [ConnectionId] = @ConnectionIdClient) > 0))
	BEGIN
		SET @id = (SELECT TOP(1) Id from [dbo].[Participant] where [BlindTestId] = @BlindTestId AND [ConnectionId] = @ConnectionIdClient);
	END
	ELSE IF (SELECT COUNT(1) from [dbo].[Participant] where [BlindTestId] = @BlindTestId AND [Name] = @UserName) > 0
	BEGIN
		IF @getExistingUserFromUserName = 0
		BEGIN
			SET @ErrorMsg = 'Brukernavnet er alt tatt!';
		END
		ELSE
		BEGIN
			SET @id = (SELECT TOP(1) Id from [dbo].[Participant] where [BlindTestId] = @BlindTestId AND [Name] = @UserName);
		END
	END
	ELSE
	BEGIN
		DECLARE @AllowAdHocParticipants bit = (SELECT AllowAdHocParticipants from [dbo].[BlindTest] where [Id] = @BlindTestId);
		DECLARE @Rounds int = (SELECT MAX(RoundNo) from [dbo].[Round] where [BlindTestId] = @BlindTestId);
		-- Sjekk om vi skal tillate nye brukere. Hvis vi har registrert noen som helst data i [dbo].[Round] - ja da er det ikke lov å legge å bli med
		IF (@AllowAdHocParticipants = 0) AND (@Rounds > 0)
		BEGIN
			SET @ErrorMsg = 'Avstemningen er alt i gang (har alt fullført ' + convert(nvarchar(10), @Rounds) + ' runder) - det er da ikke tillatt å bli med.';
		END;
	END;

	-- Most likely we must add the user here:
	IF (@id < 0) AND (LEN(@ErrorMsg) = 0)
	begin
		INSERT INTO [dbo].[Participant] ([Name], [BlindTestId], [ConnectionId], [UpdateReason]) 
		VALUES (@UserName, @BlindTestId, @ConnectionId, 'Created');
		SET @id = SCOPE_IDENTITY();

		UPDATE a 
		SET a.ImageIndex = IIF(b.img_id < 10, '0', '') + CONVERT(nvarchar(10), b.img_id)
		FROM dbo.Participant as a
		CROSS APPLY(SELECT img_id = ((a.Id) % 10)+1) AS b
		WHERE a.Id = @id;
	END;
	SELECT Result = @id, Error = @ErrorMsg;
END
