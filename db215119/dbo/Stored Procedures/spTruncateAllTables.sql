

CREATE PROCEDURE [dbo].[spTruncateAllTables]
(
   @token nvarchar(100) = NULL
)
AS
BEGIN
	SET NOCOUNT ON;

	IF @token = 'do it!' 
	BEGIN
		TRUNCATE TABLE [dbo].[Log];
		TRUNCATE TABLE [dbo].[CurrentRound];
		TRUNCATE TABLE [dbo].[Round];
		TRUNCATE TABLE [dbo].[Participant];
		TRUNCATE TABLE [dbo].[BlindTest];
	END;
	SELECT Tabell = 'Log', AntallRader = COUNT(*) FROM [dbo].[Log]
	UNION ALL
	SELECT Tabell = 'CurrentRound', AntallRader = COUNT(*) FROM [dbo].[CurrentRound]
	UNION ALL
	SELECT Tabell = 'Round', AntallRader = COUNT(*) FROM [dbo].[Round]
	UNION ALL
	SELECT Tabell = 'Participant', AntallRader = COUNT(*) FROM [dbo].[Participant]
	UNION ALL
	SELECT Tabell = 'BlindTest', AntallRader = COUNT(*) FROM [dbo].[BlindTest];
END