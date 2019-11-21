


CREATE PROCEDURE [dbo].[spGetNewBlindTest2](@BlindTestName nvarchar(100))
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @id int = -1;
	declare @ErrorMsg nvarchar(100) = '';
	--declare @cnt int = (SELECT COUNT(1) from [dbo].[BlindTest] where [Name] = @BlindTestName);
	IF (SELECT COUNT(1) from [dbo].[BlindTest] where [Name] = @BlindTestName) > 0
	begin
		SET @ErrorMsg = 'Navnet på Blindtesten ("' + @BlindTestName + '") er alt i bruk!';
	end
	ELSE
	begin
		INSERT INTO [dbo].[BlindTest] ([Name]) 
		VALUES (ISNULL(@BlindTestName, ''));
		set @id = SCOPE_IDENTITY();
	end;
	SELECT ID = @id, ErrorMsg = @ErrorMsg;
END
