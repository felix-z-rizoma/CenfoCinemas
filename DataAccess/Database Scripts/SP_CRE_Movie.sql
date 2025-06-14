--SP para crear un usuario.
CREATE PROCEDURE CRE_MOVIE_PR
	@P_Title nvarchar(75),
	@P_Desc nvarchar(250),
	@P_ReleaseDate Datetime,
	@P_Genre nvarchar(20),
	@P_Director nvarchar(20)

	AS
	BEGIN
		INSERT INTO TBL_Movie(Created, Title, Description, ReleaseDate, Genre, Director)
		VALUES(GetDate(),@P_Title,@P_Desc,@P_ReleaseDate,@P_Genre,@P_Director)
	END
	GO