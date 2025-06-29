CREATE OR ALTER PROCEDURE [dbo].[UPD_MOVIE_PR]
    @P_Id INT,
    @P_Title NVARCHAR(75),
    @P_Description NVARCHAR(250),
    @P_ReleaseDate DATETIME,
    @P_Genre NVARCHAR(20),
    @P_Director NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        UPDATE [dbo].[TBL_Movie] SET
            Title = @P_Title,
            Description = @P_Description,
            ReleaseDate = @P_ReleaseDate,
            Genre = @P_Genre,
            Director = @P_Director,
            Updated = GETDATE()
        WHERE 
            Id = @P_Id;
        
        -- Verify update was successful
        IF @@ROWCOUNT = 0
            RAISERROR('Movie with ID %d not found', 16, 1, @P_Id);
            
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        -- Re-throw the error with details
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END