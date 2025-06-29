using DataAccess.CRUD;
using DTOs;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp
{
    public class UserManager : BaseManager
    {
        private readonly UserCrudFactory _userCrudFactory;

        public UserManager()
        {
            _userCrudFactory = new UserCrudFactory();
        }



        public async Task Create(User user)
        {
            try
            {
                ValidateUser(user);
                await ValidateUniqueUser(user);

                user.Created = DateTime.Now;
                user.Status = "Active"; // Default status

                _userCrudFactory.Create(user);
                await SendWelcomeEmail(user);
            }
            catch (Exception ex)
            {
                ManageException(ex);
                throw; // Re-throw after logging
            }
        }

        public List<User> RetrieveAll()
        {
            return _userCrudFactory.RetrieveAll<User>();
        }

        public User RetrieveById(int id)
        {
            var user = _userCrudFactory.RetrieveById<User>(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            return user;
        }

        #region Private Methods

        private void ValidateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!IsOver18(user))
                throw new BusinessException("User must be at least 18 years old");

            if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@"))
                throw new BusinessException("Invalid email address");

            if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 8)
                throw new BusinessException("Password must be at least 8 characters");
        }

        private async Task ValidateUniqueUser(User user)
        {
            var existingByCode = _userCrudFactory.RetrieveByUserCode<User>(user);
            if (existingByCode != null)
                throw new BusinessException("User code already exists");

            var existingByEmail = _userCrudFactory.RetrieveByEmail<User>(user);
            if (existingByEmail != null)
                throw new BusinessException("Email already registered");
        }

        public bool IsOver18(User user)
        {
            var today = DateTime.Today;
            var age = today.Year - user.BirthDate.Year;

            // Handle leap years
            if (user.BirthDate.Date > today.AddYears(-age))
                age--;

            return age >= 18;
        }

        private static async Task<bool> SendWelcomeEmail(User user)
        {
            try
            {
                Console.WriteLine("[DEBUG] Starting SendWelcomeEmail");

                var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    Console.WriteLine("[ERROR] SendGrid API key not found in environment variables.");
                    return false;
                }

                if (!apiKey.StartsWith("SG."))
                {
                    Console.WriteLine("[ERROR] API key does not start with 'SG.'. Please check your SendGrid key.");
                    return false;
                }

                Console.WriteLine($"[DEBUG] Loaded API Key (first 5 chars): {apiKey.Substring(0, 5)}...");

                var client = new SendGridClient(apiKey);

                var from = new EmailAddress("fzumbadoz@ucenfotec.ac.cr", "CenfoCinemas");
                var to = new EmailAddress(user.Email, user.Name);
                var subject = "Welcome to CenfoCinemas!";
                var plainTextContent = "Thanks for signing up with CenfoCinemas!";
                var htmlContent = "<strong>Thanks for signing up with CenfoCinemas!</strong>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                var response = await client.SendEmailAsync(msg);

                Console.WriteLine($"[DEBUG] Email response status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Body.ReadAsStringAsync();
                    Console.WriteLine($"[DEBUG] SendGrid Response Body: {responseBody}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to send email: {ex.Message}");
                return false;
            }
        }





        #endregion
    }

    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
}