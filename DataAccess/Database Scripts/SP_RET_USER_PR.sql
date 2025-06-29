CREATE OR ALTER PROCEDURE [dbo].[RET_USER_PR]
    @P_Id INT = NULL,
    @P_Email NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Created,
        Updated,
        UserCode,
        Name,
        Email,
        Password,
        Status,
        BirthDate
    FROM TBL_User
    WHERE 
        (@P_Id IS NULL OR Id = @P_Id)
        AND (@P_Email IS NULL OR Email = @P_Email)
       
END