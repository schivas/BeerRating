CREATE PROCEDURE [dbo].[spDeleteBlindTest](@BlindTestId int, @allow_delete_completed bit = 0)
AS
BEGIN
	SET NOCOUNT ON;

	IF (SELECT SUM(1) FROM [dbo].[BlindTest] WHERE Id = @BlindTestId AND IsCompleted = 1) <> 0 AND @allow_delete_completed = 0
	BEGIN
		RAISERROR('Den spesifiserte runden er fullført. Du kan ikke slette slike runder med mindre parameteren @allow_delete_completed er satt til True (1).', 16, 1);
		RETURN;
	END;
	
	-- Denne inneholder ikke identity
	IF (SELECT SUM(1) FROM [dbo].[CurrentRound] WHERE [BlindTestId] <> @BlindTestId) <> 0
	BEGIN
		DELETE FROM [dbo].[CurrentRound] WHERE [BlindTestId] = @BlindTestId;
	END
	ELSE
	BEGIN
		TRUNCATE TABLE [dbo].[CurrentRound];
	END;

	DECLARE @seed int = 0;
	DELETE FROM [dbo].[Participant] WHERE [BlindTestId] = @BlindTestId;
	IF (SELECT SUM(1) FROM [dbo].[Participant]) <> 0
	BEGIN
		SET @seed = (SELECT MAX(Id) FROM [dbo].[Participant]);
		DBCC CHECKIDENT ('[dbo].[Participant]', RESEED, @seed);
	END
	ELSE
	BEGIN
		TRUNCATE TABLE [dbo].[Participant]; -- Denne vil resette identity
	END;

	DELETE FROM [dbo].[Round] WHERE [BlindTestId] = @BlindTestId;
	IF (SELECT SUM(1) FROM [dbo].[Round]) <> 0
	BEGIN
		SET @seed = (SELECT MAX(Id) FROM [dbo].[Round]);
		DBCC CHECKIDENT ('[dbo].[Round]', RESEED, @seed);
	END
	ELSE
	BEGIN
		TRUNCATE TABLE [dbo].[Round]; -- Denne vil resette identity
	END;

	DELETE FROM [dbo].[BlindTest] WHERE [Id] = @BlindTestId;
	IF (SELECT SUM(1) FROM [dbo].[BlindTest]) <> 0
	BEGIN
		SET @seed = (SELECT MAX(Id) FROM [dbo].[BlindTest]);
		DBCC CHECKIDENT ('[dbo].[BlindTest]', RESEED, @seed);
	END
	ELSE
	BEGIN
		TRUNCATE TABLE [dbo].[BlindTest]; -- Denne vil resette identity
	END;
END