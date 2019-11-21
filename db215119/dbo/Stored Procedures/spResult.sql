

CREATE PROCEDURE [dbo].[spResult](@BlindTestId int)
AS
BEGIN
	SET NOCOUNT ON;
	--WITH BASE AS
	--(
	--	SELECT
	--		a.BlindTestId
	--		,a.ParticipantId
	--		,a.RoundNo
	--		,a.BrandName
	--		,a.ABV
	--		,Snitt = CONVERT(decimal(18,2), ROUND(AVG(a.Vote) OVER (PARTITION BY a.RoundNo), 2))
	--		,b.[Name]
	--		,a.Vote
	--		,Overridden = ISNULL(a.Overridden, Overridden)
	--		,SnittPerson = CONVERT(decimal(18,2), ROUND(AVG(a.Vote) OVER (PARTITION BY a.ParticipantId), 2))
	--	FROM [dbo].[Round] AS a
	--	inner join [dbo].[Participant] as b
	--		on a.BlindTestId = b.BlindTestId
	--		and a.ParticipantId = b.Id
	--	where a.BlindTestId = @BlindTestId
	--) 
	--SELECT 
	--	a.BlindTestId
	--	,a.ParticipantId
	--	,a.RoundNo
	--	,a.BrandName
	--	,a.ABV
	--	,Rangering = DENSE_RANK() OVER (ORDER BY Snitt DESC)
	--	,Snitt = ISNULL(a.Snitt, 0)
	--	,a.[Name]
	--	,a.Vote
	--	,a.Overridden
	--	,SnittPerson = ISNULL(SnittPerson, 0)
	--FROM BASE AS a
	--ORDER BY a.RoundNo, a.ParticipantId;

	SELECT
		a.BlindTestId
		,a.ParticipantId
		,a.RoundNo
		,a.BrandName
		,a.ABV
		,c.Rangering
		,Snitt = CONVERT(decimal(18,2), ROUND(AVG(a.Vote) OVER (PARTITION BY a.RoundNo), 2))
		,b.[Name]
		,a.Vote
		,Overridden = ISNULL(a.Overridden, Overridden)
		,SnittPerson = CONVERT(decimal(18,2), ROUND(AVG(a.Vote) OVER (PARTITION BY a.ParticipantId), 2))
	FROM [dbo].[Round] AS a
	INNER JOIN [dbo].[Participant] as b
		ON a.BlindTestId = b.BlindTestId
		AND a.ParticipantId = b.Id
	INNER JOIN
	(
		SELECT 
			a.BrandName
			,Rangering = RANK() OVER(ORDER BY Snitt DESC)
		FROM 
		(
			SELECT
				BrandName
				,Snitt = CONVERT(decimal(18,2), ROUND(AVG(Vote) , 2))
			FROM [dbo].[Round]
			WHERE BlindTestId = @BlindTestId
			GROUP BY BrandName
		) AS a
	) AS c ON a.BrandName = c.BrandName
	WHERE a.BlindTestId = @BlindTestId
	ORDER BY a.RoundNo, a.ParticipantId;
END
