CREATE OR ALTER PROCEDURE [dbo].[UPD_USER_PR]
    @P_Id INT,
    @P_UserCode NVARCHAR(50),  -- Added UserCode parameter
    @P_Name NVARCHAR(100),
    @P_Email NVARCHAR(100),
    @P_Status NVARCHAR(20),
    @P_BirthDate DATETIME,
    @P_Password NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        UPDATE TBL_User SET
            UserCode = @P_UserCode,  
            Name = @P_Name,
            Email = @P_Email,
            Status = @P_Status,
            BirthDate = @P_BirthDate,
            Updated = GETDATE(),
            Password = CASE 
                          WHEN @P_Password IS NULL THEN Password 
                          ELSE @P_Password 
                       END
        WHERE Id = @P_Id;
        
        IF @@ROWCOUNT = 0
            RAISERROR('User with ID %d not found', 16, 1, @P_Id);
            
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END