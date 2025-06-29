CREATE OR ALTER PROCEDURE [dbo].[RET_MOVIE_PR]
    @P_Id INT = NULL,              -- Optional: Get by ID
    @P_Title NVARCHAR(75) = NULL,  -- Optional: Filter by title
    @P_Genre NVARCHAR(20) = NULL   -- Optional: Filter by genre
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Created,
        Updated,
        Title,
        Description,
        ReleaseDate,
        Genre,
        Director
    FROM [dbo].[TBL_Movie]
    WHERE 
        (@P_Id IS NULL OR Id = @P_Id)
        AND (@P_Title IS NULL OR Title LIKE '%' + @P_Title + '%')
        AND (@P_Genre IS NULL OR Genre = @P_Genre)
    ORDER BY 
        Title ASC;
END