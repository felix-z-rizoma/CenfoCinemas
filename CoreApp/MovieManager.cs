using DataAccess.CRUD;
using DTOs;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 
public class MovieManager
{
    private readonly MovieCrudFactory _crudFactory;
    private readonly UserCrudFactory _userCrudFactory;

    public MovieManager()
    {
        _crudFactory = new MovieCrudFactory();
        _userCrudFactory = new UserCrudFactory();
    }

    public List<Movie> RetrieveAll()
    {
        return _crudFactory.RetrieveAll<Movie>();
    }

    public Movie RetrieveById(int id)
    {
        var movie = _crudFactory.RetrieveById<Movie>(id);
        if (movie == null)
            throw new KeyNotFoundException($"Movie with ID {id} not found");

        return movie;
    }

    public Movie Update(Movie movie)
    {
        var existingMovie = RetrieveById(movie.Id);

        movie.Created = existingMovie.Created;

        _crudFactory.Update(movie);
        return movie;
    }

    public void Delete(int id)
    {
        var movie = RetrieveById(id);
        _crudFactory.Delete(movie);
    }

    #region Private Methods

    public async Task<Movie> Create(Movie movie)
    {
        ValidateMovie(movie);
        CheckForDuplicates(movie);

        movie.Created = DateTime.Now;
        _crudFactory.Create(movie);

        Console.WriteLine("[DEBUG] About to send notifications…");
        await SendNotificationsToAllUsers(movie);
        Console.WriteLine("[DEBUG] Notifications sent successfully.");

        return movie;
    }



    private void ValidateMovie(Movie movie)
    {
        if (movie == null)
            throw new ArgumentNullException(nameof(movie));

        if (string.IsNullOrWhiteSpace(movie.Title) || movie.Title.Length > 50)
            throw new ArgumentException("Title must be 1-50 characters");

        if (movie.ReleaseDate < new DateTime(1888, 1, 1))
            throw new ArgumentException("Release date predates cinema");

        if (movie.ReleaseDate > DateTime.Now.AddYears(5))
            throw new ArgumentException("Release date too far in future");
    }

    private void CheckForDuplicates(Movie movie)
    {
        var existingMovies = _crudFactory.RetrieveAll<Movie>();
        if (existingMovies.Any(m =>
            m.Title.Equals(movie.Title, StringComparison.OrdinalIgnoreCase) &&
            m.ReleaseDate.Year == movie.ReleaseDate.Year))
        {
            throw new InvalidOperationException("Movie already exists");
        }
    }

    private async Task SendNotificationsToAllUsers(Movie movie)
    {
        Console.WriteLine("[DEBUG] ==> SendNotificationsToAllUsers START");
        try
        {
            var allUsers = _userCrudFactory.RetrieveAll<User>();
            Console.WriteLine($"[DEBUG] Retrieved {allUsers.Count} users for notifications");

            var batchSize = 50;
            for (int i = 0; i < allUsers.Count; i += batchSize)
            {
                Console.WriteLine($"[DEBUG] Processing batch starting at index {i}");
                var batch = allUsers.Skip(i).Take(batchSize);
                var emailTasks = batch.Select(user => SendNewMovieEmail(user, movie));
                await Task.WhenAll(emailTasks);
                Console.WriteLine("[DEBUG] Batch processed, pausing for rate limit");
                await Task.Delay(1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Notification error: {ex}");
        }
        Console.WriteLine("[DEBUG] <== SendNotificationsToAllUsers END");
    }

    private async Task SendNewMovieEmail(User user, Movie movie)
    {
        try
        {
            Console.WriteLine($"[DEBUG] -> SendNewMovieEmail START for {user.Email}");
            // Validate API key
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ??
                        throw new InvalidOperationException("SendGrid API key is not configured");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("SendGrid API key is empty");
            }

            // Initialize client
            var client = new SendGridClient(apiKey);

            // Prepare email content
            var from = new EmailAddress("fzumbadoz@ucenfotec.ac.cr", "CenfoCinemas");
            var to = new EmailAddress(user.Email, user.Name);
            var subject = $"Nuevo estreno: {movie.Title}";

            var plainTextContent = $"""
            Hola {user.Name},
            
            ¡Tenemos buenas noticias! La película "{movie.Title}" 
            ya está disponible en nuestro catálogo.
            
            Fecha de estreno: {movie.ReleaseDate:d}
            Género: {movie.Genre}
            Director: {movie.Director}
            
            ¡Te esperamos!
            
            El equipo de CenfoCinemas
            """;

            var htmlContent = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; line-height: 1.6;">
                <h2 style="color: #0066cc; border-bottom: 2px solid #0066cc; padding-bottom: 10px;">
                    ¡Nuevo estreno en CenfoCinemas!
                </h2>
                <p>Hola {user.Name},</p>
                
                <div style="background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 15px 0;">
                    <h3 style="margin-top: 0; color: #0066cc;">{movie.Title}</h3>
                    <div>
                        <p><strong>Fecha de estreno:</strong> {movie.ReleaseDate:dd MMMM yyyy}</p>
                        <p><strong>Género:</strong> {movie.Genre}</p>
                        <p><strong>Director:</strong> {movie.Director}</p>
                    </div>
                </div>
                
                <div style="text-align: center; margin: 20px 0;">
                    <a href="https://cenfocinemas.com/movies/{movie.Id}" 
                       style="background-color: #0066cc; color: white; padding: 12px 24px; 
                              text-decoration: none; border-radius: 5px; font-weight: bold;">
                        Ver detalles y reservar
                    </a>
                </div>
                
                <p style="color: #666; font-size: 0.9em; border-top: 1px solid #eee; padding-top: 15px;">
                    © {DateTime.Now.Year} CenfoCinemas - Todos los derechos reservados
                </p>
            </div>
            """;

            // Create and send email
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            // Check response
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send email. Status: {response.StatusCode}. Response: {responseBody}");
            }
        }
        catch (Exception ex)
        {
            // Enhanced error logging
            Console.WriteLine($"[ERROR] Failed to send new movie email to {user.Email} ({user.Id})");
            Console.WriteLine($"Movie: {movie.Title} ({movie.Id})");
            Console.WriteLine($"Error details: {ex}");

           
        }
    }
    #endregion
}