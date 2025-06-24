using DataAccess.DAO;
using DataAccess.CRUD;
using DTOs;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection.Metadata;
using Newtonsoft.Json;
using CoreApp;


public class Program{
    public static void Main(string[] args)
    {
        Console.WriteLine("Seleccione la opcion deseada:");
        Console.WriteLine("1.Crear usuario.");
        Console.WriteLine("2. Consultar usuarios");
        Console.WriteLine("3. Actualizar usuarios");
        Console.WriteLine("4. Eliminar usuarios");
        Console.WriteLine("5. Crear pelicula");
        Console.WriteLine("6. Consultar peliculas");
        Console.WriteLine("7. Actualizar peliculas");
        Console.WriteLine("8. Eliminar peliculas");

        var option = Int32.Parse(Console.ReadLine());
        var sqlOperation = new SqlOperation();
        

        switch (option)
        {
            case 1:
                Console.WriteLine("Digite el codigo de usuario");
                var userCode = Console.ReadLine();

                Console.WriteLine("Digite el nombre");
                var name = Console.ReadLine();

                Console.WriteLine("Digite el email");
                var email = Console.ReadLine();

                Console.WriteLine("Digite el password");
                var password = Console.ReadLine();

                var status = "AC";

                Console.WriteLine("Digite la fecha de nacimiento");
                var bdate = DateTime.Parse(Console.ReadLine());

                //Creamos el objeto del usuario a partir de los valores capturados en cosnola
                var user = new User()
                {
                    UserCode = userCode,
                    Name = name,
                    Email = email,
                    Password = password,
                    Status = status,
                    BirthDate = bdate
                };

                var um = new UserManager();
                um.Create(user);
           

               
            break;
            
            case 2:

        /*     uCrud = new UserCrudFactory();
             var listUsers = uCrud.RetrieveAll<User>();
            foreach(var u in listUsers)
            {
            Console.WriteLine(JsonConvert.SerializeObject(u));
            }

             break;
*/


            case 5:
                Console.WriteLine("Digite el titulo");
                var title = Console.ReadLine();

                Console.WriteLine("Digite la descripcion");
                var desc = Console.ReadLine();

                Console.WriteLine("Digite la fecha de lanzamiento");
                var rDate = DateTime.Parse(Console.ReadLine());

                Console.WriteLine("Digite el genero de la pelicula");
                var genre = Console.ReadLine();

                Console.WriteLine("Digite el director");
                var director = Console.ReadLine();

                sqlOperation.ProcedureName = "CRE_MOVIE_PR";
                sqlOperation.AddStringParameter("P_Titlee", title);
                sqlOperation.AddStringParameter("P_Desc", desc);
                sqlOperation.AddDateTimeParam("P_ReleaseDate", rDate);
                sqlOperation.AddStringParameter("P_Genre", genre);
                sqlOperation.AddStringParameter("P_Director", director);

                break;
        


        }
        //Ejecucion del procedure
        //var sqlDao = SqlDao.GetInstance();
        //sqlDao.ExecuteProcedure(sqlOperation);

    }


}