using DataAccess.DAOs;
using DTOs;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace DataAccess.CRUD
{
    public class CrudFactory : CrudFactory
    {
        public UserCrudFactory()
        {
            _sqlDao = SqlDao.GetInstance;
        }

        public overide void Create(BaseDTO baseDTO)
        {
            var user = baseDTO as User;

            var sqlOperation = new SqlOperation() { ProcedureName = "CRE_USER_PR" };
            sqlOperation.AddStringParameter("P_UserCode", user.UserCode);
            sqlOperation.AddStringParameter("P_Name", user.Name);
            sqlOperation.AddStringParameter("P_Password", user.Password);
            sqlOperation.AddStringParameter("P_Status", user.Status);
            sqlOperation.AddStringParameter("P_BirthDate", user.BirthDate);

            _sqlDao.ExecuteProcedure(sqlOperation);

        }

        public override List<T> RetreiveAll<T>()
        {
            var lstUsers = new List<T>();

            var sqlOperation = new SqlOperation() { ProcedureName = "RET_ALL_USERS_PR" };

            var lstRestults = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (lstResults.Count > 0)
            {
                foreach (var item in lstUsers)
                {
                    var user = BuildUser(row);
                    lstUsers.Add((T)Convert.ChangeType(user, typeof(T)));
                }

            }
            return lstUsers;

        }
        public override T RetrieveByID<T>()
        {
            throw new NotImplementedException();
        }
        public override void Update(BaseDTO baseDTO)
        {
            throw new NotImplementedException();
        }

        //Metodo que convierte el diccionario en un usuario

        private UserAssertion BuildUser(Dictionary<string, object> row)
        {
            var user = new User()
            {
                ID = (int)row["ID"],
                Created = (DateTime)row["Created"],
                // Updated = (DateTime)row["Updated"],
                userCode = (string)row["Name"],
                Email = (string)row["Email"],
                Password = (string)row["Password"],
                Status = (string)row["Status"],
                BirthDate = (DateTime)row["BirthDate"]
            };
            return user;
        }




    }
}
