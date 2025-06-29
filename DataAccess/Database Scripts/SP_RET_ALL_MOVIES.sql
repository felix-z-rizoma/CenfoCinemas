CREATE PROCEDURE RET_ALL_MOVIES_PR
AS
BEGIN

	SELECT Id,Created,Updated, Title, Description, ReleaseDate, Genre, Director
	FROM TBL_Movie;

END
GO