CREATE PROCEDURE [dbo].[spApplyVotes]
(
   @BlindTestId int,
   @BrandName nvarchar(100),
   @ABV decimal(18,1),
   @OverrideMode int = 0,
   @DefaultVote decimal(18,1) = 3.5,
   @NumberOfDecimals int = 0
)
AS
BEGIN
	SET NOCOUNT ON;
	SET @BrandName = RTRIM(LTRIM(@BrandName));
	SET @ABV = ISNULL(@ABV, 0);
	SET @OverrideMode = ISNULL(IIF(@OverrideMode > 2, 2, IIF(@OverrideMode < 0, 0, @OverrideMode)), 1);
	-- OverrideMode
	-- 0: Ikke tillat lagring av stemmer hvis ikke alle har stemt. En finner ut hvor mange som er med fra dbo.Round
	--    Hvis det er første runde, har ikke denne noe å si
	-- 1: Tillat lagring av stemmer. Setter inn NULL i Vote - noe som gjør at snittet senere ikke tar med seg denne stemmen.
	-- 2: Tillat lagring av stemmer. For bruker(e) som ikke har avgitt stemme setter vi inn default verdi som er 3,5 (kan overstyres)

	DECLARE @inserted_rows int = 0;
	DECLARE @ErrorMsg nvarchar(100) = '';

	IF LEN(@BrandName) = 0
	BEGIN
		SET @ErrorMsg = 'Det var ikke oppgitt ølsort!';
	END
	ELSE IF (SELECT COUNT(1) from [dbo].[Round] WHERE [BlindTestId] = @BlindTestId AND BrandName = @BrandName) > 0
	BEGIN
		SET @ErrorMsg = 'Oppgitt ølsort har allerede vært testet!';
	END
	ELSE IF (@ABV <= 0)
	BEGIN
		SET @ErrorMsg = 'Oppgitt ABV er på tryne!';
	END
	ELSE IF (SELECT COUNT(1) from [dbo].[BlindTest] WHERE [Id] = @BlindTestId) = 0
	BEGIN
		SET @ErrorMsg = 'Blindtest (Id = ' + CONVERT(nvarchar(10), @BlindTestId) + ') eksisterer ikke - kan ikke registrere stemmer!';
	END
	-- LAGT TIL: 2019-11-21
	ELSE IF (SELECT COUNT(1) from [dbo].[BlindTest] WHERE [Id] = @BlindTestId AND [IsCompleted] = 1) <> 0
	BEGIN
		SET @ErrorMsg = 'Blindtest (Id = ' + CONVERT(nvarchar(10), @BlindTestId) + ') er fullført - kan ikke registrere stemmer!';
	END


	ELSE IF (SELECT COUNT(1) from [dbo].[CurrentRound] WHERE [BlindTestId] = @BlindTestId) < 2
	BEGIN
		-- Ingen vits i å lagre unna noe mindre enn to stemmer
		SET @ErrorMsg = 'Mottatt runde inneholder ikke nok stemmer, må være minimum to stemmer (BlindtestId = ' + convert(nvarchar(10), @BlindTestId) + ')';
	END
	ELSE
	BEGIN
		-- Vi finner neste rundenummer fra ferskeste forrige
		DECLARE @round_no int = (SELECT MAX(RoundNo) FROM [dbo].[Round] WHERE BlindTestId = @BlindTestId);
		SET @round_no = ISNULL(@round_no, 0) + 1;

		DECLARE @cnt_curr_round int = (SELECT COUNT(1) FROM [dbo].[CurrentRound] WHERE BlindTestId = @BlindTestId);
		DECLARE @cnt_round int = (SELECT MAX(antall) FROM (SELECT RoundNo, Antall = COUNT(1) FROM [dbo].[Round] WHERE BlindTestId = @BlindTestId GROUP BY RoundNo) as a);
		SET @cnt_round = ISNULL(@cnt_round, @cnt_curr_round);
		IF (@cnt_curr_round < @cnt_round)
		BEGIN 
			-- Hmm -en eller flere har ikke avgitt stemmer...
			IF (@OverrideMode = 0)
			BEGIN
				SET @ErrorMsg = 'Alle må ha avgitt stemmer for å registrere en runde - her er kun ' + convert(nvarchar(10), @cnt_curr_round) + ' av ' + convert(nvarchar(10), @cnt_round) + ' stemmer avgitt!';
			END
			ELSE
			BEGIN
				INSERT INTO [dbo].[Round] ([BlindTestId], [ParticipantId], [RoundNo], [BrandName], [ABV], [Vote], [Overridden])
				SELECT
					a.BlindTestId
					,a.ParticipantId
					,@round_no
					,@BrandName
					,@ABV
					,Vote = IIF(b.Vote IS NULL, IIF(@OverrideMode = 1, NULL, @DefaultVote), b.Vote)
					,Overridden = IIF(b.Vote IS NULL, 1, 0)
				FROM (SELECT DISTINCT [BlindTestId], [ParticipantId] FROM [dbo].[Round] where BlindTestId = @BlindTestId) AS a
				LEFT JOIN [dbo].[CurrentRound] AS b
					ON a.BlindTestId = b.BlindTestId
					AND a.ParticipantId = b.ParticipantId;
				SET @inserted_rows = @@ROWCOUNT;
			END;
		END
		ELSE
		BEGIN
			-- Alle har avgitt stemmer
			INSERT INTO [dbo].[Round] ([BlindTestId], [ParticipantId], [RoundNo], [BrandName], [ABV], [Vote])
			SELECT
				a.BlindTestId
				,a.ParticipantId
				,@round_no
				,@BrandName
				,@ABV
				,a.Vote
			FROM [dbo].[CurrentRound] AS a
			WHERE a.BlindTestId = @BlindTestId;
			SET @inserted_rows = @@ROWCOUNT;
		END;
	END;

	IF (@inserted_rows > 0)
	BEGIN
		-- Clear current round
		DELETE FROM [dbo].[CurrentRound] WHERE BlindTestId = @BlindTestId;
	END;
	SELECT Result = @inserted_rows, Error = @ErrorMsg;
END
