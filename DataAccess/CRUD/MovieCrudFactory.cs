using DataAccess.DAO;
using DTOs;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DataAccess.CRUD
{
    public class MovieCrudFactory : CrudFactory
    {


        public MovieCrudFactory()
        {
            _sqlDao = SqlDao.GetInstance();
        }

        public override void Create(BaseDTO baseDTO)
        {
            var movie = baseDTO as Movie;

            var sqlOperation = new SqlOperation() { ProcedureName = "CRE_MOVIE_PR" };
            sqlOperation.AddStringParameter("P_Title", movie.Title);
            sqlOperation.AddStringParameter("P_Desc", movie.Description);
            sqlOperation.AddDateTimeParam("P_ReleaseDate", movie.ReleaseDate);
            sqlOperation.AddStringParameter("P_Genre", movie.Genre);
            sqlOperation.AddStringParameter("P_Director", movie.Director);

            _sqlDao.ExecuteProcedure(sqlOperation);


        }
        public override void Delete(BaseDTO baseDTO)
        {
            var movie = baseDTO as Movie;
            if (movie == null)
                throw new ArgumentException("Invalid DTO type - expected Movie");

            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "DEL_MOVIE_PR"
            };
            sqlOperation.AddStringParameter("P_Title", movie.Title);
            sqlOperation.AddDateTimeParam("P_ReleaseDate", movie.ReleaseDate);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        public override T Retrieve<T>()
        {

            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "RET_MOVIE_PR"
            };

            var result = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (result.Count == 0)
                return default(T);

            var user = BuildMovie(result[0]);
            return (T)Convert.ChangeType(user, typeof(T));
        }

        public override List<T> RetrieveAll<T>()
        {
            var lstMovies = new List<T>();

            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "RET_ALL_MOVIES_PR"
            };

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (lstResults.Count > 0)
            {
                foreach (var row in lstResults)
                {
                    var movie = BuildMovie(row);
                    lstMovies.Add((T)Convert.ChangeType(movie, typeof(T)));
                }
            }

            return lstMovies;
        }

        private Movie BuildMovie(Dictionary<string, object> row)
        {
            return new Movie()
            {
                Id = (int)row["Id"],
                Created = (DateTime)row["Created"],
                Title = (string)row["Title"],
                Description = row["Description"]?.ToString(),
                ReleaseDate = (DateTime)row["ReleaseDate"],
                Genre = (string)row["Genre"],
                Director = (string)row["Director"],
              
            };
        }

        public override T RetrieveById<T>(int id)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_MOVIE_BY_ID_PR" };
            sqlOperation.AddIntParam("P_ID", id);

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (lstResults.Count > 0)
            {
                var row = lstResults[0];
                var user = BuildMovie(row);

                return (T)Convert.ChangeType(user, typeof(T));


            }
            return default(T);
        }

        public override void Update(BaseDTO baseDTO)
        {
            var movie = baseDTO as Movie;
            if (movie == null)
                throw new ArgumentException("Invalid DTO type - expected Movie");

            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "UPD_MOVIE_PR"
            };

            // Required parameters
            sqlOperation.AddIntParam("P_Id", movie.Id);
            sqlOperation.AddStringParameter("P_Title", movie.Title);
            sqlOperation.AddDateTimeParam("P_ReleaseDate", movie.ReleaseDate);

            // Optional parameters
            sqlOperation.AddStringParameter("P_Description", movie.Description);
            sqlOperation.AddStringParameter("P_Genre", movie.Genre);
            sqlOperation.AddStringParameter("P_Director", movie.Director);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }
    }


    }
