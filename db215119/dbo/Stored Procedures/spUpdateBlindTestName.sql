
CREATE PROCEDURE [dbo].[spUpdateBlindTestName]
(
   @BlindTestId int,
   @BlindTestName nvarchar(100)
)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Result nvarchar(100) = 'Blindtestnavn fle ';
	
	DECLARE @cnt int = (SELECT COUNT(1) FROM [dbo].[BlindTest] WHERE [Id] = @BlindTestId);
	IF @cnt > 0
	BEGIN
		DECLARE @old_name nvarchar(100) = (SELECT [Name] FROM [dbo].[BlindTest] WHERE [Id] = @BlindTestId);
		UPDATE [dbo].[BlindTest]
		SET [Name] =  ISNULL(NULLIF(@BlindTestName, ''), [Name])
		WHERE [Id] = @BlindTestId;
		DECLARE @new_name nvarchar(100) = (SELECT [Name] FROM [dbo].[BlindTest] WHERE [Id] = @BlindTestId);
		IF @old_name = @new_name
		BEGIN
			SET @Result = 'Ingen navneendring detektert';
		END
		ELSE
		BEGIN
			SET @Result = 'Navn endret fra"' + @old_name + '" til "' + @new_name + '"';
		END;
	END
	ELSE
	BEGIN
		SET @Result = 'Fant ikke angitt blindtest-ID';
	END;
	SELECT Result = @Result;
END