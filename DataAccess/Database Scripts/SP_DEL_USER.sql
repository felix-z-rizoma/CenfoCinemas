CREATE PROCEDURE [dbo].[DEL_USER_PR]
    @P_UserCode NVARCHAR(50)
AS
BEGIN
    DELETE FROM [dbo].[TBL_User]
    WHERE UserCode = @P_UserCode;
END
