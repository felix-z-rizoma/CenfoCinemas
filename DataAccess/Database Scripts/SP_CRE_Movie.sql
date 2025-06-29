/****** Object:  StoredProcedure [dbo].[CRE_MOVIE_PR]    Script Date: 6/29/2025 9:03:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--SP para crear una pelicula
ALTER PROCEDURE [dbo].[CRE_MOVIE_PR]
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