using DataAccess.DAO;
using DataAccess.CRUD;
using DTOs;
using System;
using System.IO;
using Newtonsoft.Json;
using CoreApp;

public class Program
{
    public static async Task Main(string[] args) 
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
                await um.Create(user);  // Await the async method call

                break;

            case 2:
                // You can add implementation here
                break;

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

                //Create a Movie object from input
                var movie = new Movie()
                {
                    Title = title,
                    Description = desc,
                    ReleaseDate = rDate,
                    Genre = genre,
                    Director = director
                };

                // Call the MovieManager
                var mm = new MovieManager();
               var createdMovie = await mm.Create(movie); // This will use CRUD factory and stored procedures

                Console.WriteLine("Película creada exitosamente.");



                break;
        }
    }
}
