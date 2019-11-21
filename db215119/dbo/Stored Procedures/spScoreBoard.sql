

CREATE PROCEDURE [dbo].[spScoreBoard](@BlindTestId int)
AS
BEGIN
	SET NOCOUNT ON;
	WITH BASE AS
	(
		SELECT
			a.BlindTestId
			,b.FullName
			,Snitt = CONVERT(decimal(18,2), ROUND(AVG(a.Vote) , 2))
		FROM [dbo].[Round] AS a
		--OUTER APPLY (SELECT FullName = a.BrandName + ' (ABV: '+ CONVERT(nvarchar(10), a.ABV) + '%)') AS b
		OUTER APPLY (SELECT FullName = a.BrandName + ' ('+ CONVERT(nvarchar(10), a.ABV) + '%)') AS b
		WHERE a.BlindTestId = @BlindTestId
		GROUP BY a.BlindTestId, b.FullName
	)
		, X_RANK AS
	(
		SELECT 
			a.BlindTestId
			,a.FullName
			,a.Snitt
			,MinVal = MIN(Snitt) OVER (ORDER BY BlindTestId) - 0.5
			,MaxVal = MAX(Snitt) OVER (ORDER BY BlindTestId) + 0.5
			,Rangering = RANK() OVER(ORDER BY Snitt DESC)
		FROM BASE AS a
	)
	, LINES AS
	(
		SELECT 
			a.BlindTestId
			,Line = '{"name":"' + CONVERT(nvarchar(10), Rangering) + '. pl - ' + FullName + '","score":' + CONVERT(nvarchar(10), Snitt) + '}'
			,MinValue = CONVERT(nvarchar(10), MinVal)
			,MaxValue = CONVERT(nvarchar(10), MaxVal)
			,Rangering
		FROM X_RANK AS a
	)
	SELECT BlindTestId, Line, MinValue, MaxValue
	FROM LINES
	ORDER BY Rangering;
END