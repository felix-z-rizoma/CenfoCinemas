CREATE PROCEDURE DEL_MOVIE_PR
    @P_Title NVARCHAR(100),
    @P_ReleaseDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Then delete the movie(s)
        DELETE FROM Movies 
        WHERE Title = @P_Title 
        AND ReleaseDate = @P_ReleaseDate;
        
        -- Verify deletion
        IF @@ROWCOUNT = 0
            RAISERROR('No movie found with specified title and release date', 16, 1);
            
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END