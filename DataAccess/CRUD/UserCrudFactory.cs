﻿using DataAccess.DAO;
using DTOs;
using Microsoft.Data.SqlClient;

namespace DataAccess.CRUD
{

    public class UserCrudFactory : CrudFactory
    {

        public UserCrudFactory()
        {
            _sqlDao = SqlDao.GetInstance();
        }


        public override void Create(BaseDTO baseDTO)
        {
            var user = baseDTO as User;

            var sqlOperation = new SqlOperation() { ProcedureName = "CRE_USER_PR" };
            sqlOperation.AddStringParameter("P_UserCode", user.UserCode);
            sqlOperation.AddStringParameter("P_Name", user.Name);
            sqlOperation.AddStringParameter("P_Email", user.Email);
            sqlOperation.AddStringParameter("P_Password", user.Password);
            sqlOperation.AddStringParameter("P_Status", user.Status);
            sqlOperation.AddDateTimeParam("P_BirthDate", user.BirthDate);

            _sqlDao.ExecuteProcedure(sqlOperation);

        }

        public override void Delete(BaseDTO baseDTO)
        {
            var user = baseDTO as User;
            if (user == null)
                throw new ArgumentException("Invalid DTO type - expected User");

            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "DEL_USER_PR"
            };
            sqlOperation.AddStringParameter("P_UserCode", user.UserCode);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        public override List<T> RetrieveAll<T>()
        {
            var lstUsers = new List<T>();

            var sqlOperation = new SqlOperation() { ProcedureName = "RET_ALL_USERS_PR" };
            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

           
            if (lstResults.Count > 0)
            {
                foreach (var row in lstResults)
                {
                    var user = BuildUser(row);  
                    lstUsers.Add((T)Convert.ChangeType(user, typeof(T)));
                }
            }

            Console.WriteLine($"[DEBUG] Retrieved {lstUsers.Count} users from database.");
            return lstUsers;
        }



        public override T RetrieveById<T>(int id)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_USER_BY_ID_PR" };
            sqlOperation.AddIntParam("P_ID", id);

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (lstResults.Count > 0)
            {
                var row = lstResults[0];
                var user = BuildUser(row);

                return (T)Convert.ChangeType(user, typeof(T));


            }
            return default(T);
        }

        public T RetrieveByUserCode<T>(User user)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_USER_BY_CODE_PR" };
            sqlOperation.AddStringParameter("P_Code", user.UserCode);

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (lstResults.Count > 0)
            {
                var row = lstResults[0];
                user = BuildUser(row);

                return (T)Convert.ChangeType(user, typeof(T));
            }
            return default(T);
        }

        public T RetrieveByEmail<T>(User user)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_USER_BY_EMAIL_PR" };
            sqlOperation.AddStringParameter("P_EMAIL", user.Email);

            var lstResults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (lstResults.Count > 0)
            {
                var row = lstResults[0];
                user = BuildUser(row);

                return (T)Convert.ChangeType(user, typeof(T));
            }
            return default(T);

        }

        public override T Retrieve<T>()
        {
        
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "RET_USER_PR"
            };

            var result = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (result.Count == 0)
                return default(T);

            var user = BuildUser(result[0]);
            return (T)Convert.ChangeType(user, typeof(T));
        }

        public override void Update(BaseDTO baseDTO)
        {
            var user = baseDTO as User;
            if (user == null)
                throw new ArgumentException("Invalid DTO type - expected User");

            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "UPD_USER_PR"
            };

            sqlOperation.AddIntParam("P_Id", user.Id);
            sqlOperation.AddStringParameter("P_UserCode", user.UserCode);
            sqlOperation.AddStringParameter("P_Name", user.Name);
            sqlOperation.AddStringParameter("P_Email", user.Email);
            sqlOperation.AddStringParameter("P_Status", user.Status);
            sqlOperation.AddDateTimeParam("P_BirthDate", user.BirthDate);

            // Optional: Only update password if provided
            if (!string.IsNullOrEmpty(user.Password))
            {
                sqlOperation.AddStringParameter("P_Password", user.Password);
            }

            _sqlDao.ExecuteProcedure(sqlOperation);
        }


        //Metodo que convierte el diccionario en un usuario
        private User BuildUser(Dictionary<string, object> row)
        {
            var user = new User()
            {
                Id = (int)row["Id"],
                Created = (DateTime)row["Created"],
                //Updated = (DateTime)row["Updated"],
                UserCode = (string)row["UserCode"],
                Name = (string)row["Name"],
                Email = (string)row["Email"],
                Password = (string)row["Password"],
                Status = (string)row["Status"],
                BirthDate = (DateTime)row["BirthDate"]
            };
            return user;

        }

    }

}
