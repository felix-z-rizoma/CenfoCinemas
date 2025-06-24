using DataAccess.CRUD;
using DTOs;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CoreApp
{
    public class UserManager : BaseManager
    {
        public void Create(User user)
        {
            try
            {

                //Validar la edad
                if (isOver18(user))
                {
                    var uCrud = new UserCrudFactory();
                    uCrud.Create(user);
                    //Consulatamos en la bd si existe un usuario con ese codigo
                    var uExist = uCrud.RetrieveByUserCode<User>(user);

                    if (uExist != null )
                    {
                        //Consultamos si en la bd existe un usuario con ese email.
                        uExist=uCrud.RetrieveByEmail<User>(user);

                        if(uExist != null )
                        {
                            uCrud.Create(user);
                            //Ahora sigue el envio de correo
                        }
                        else
                        {
                            throw new Exception("Este correo electronico ya se encuentra registrado");
                        }
                    }
                    else
                    {
                        throw new Exception("El codigo de usuario no esta disponible");
                    }

                }
                else
                {
                    throw new Exception("Usuario no cumple con la edad minima");
                }


            }
            catch (Exception ex)
            {
                ManageException(ex);
            }
        }

        private bool isOver18(User user)
        {
            var currentDate= DateTime.Now;
            int age=currentDate.Year - user.BirthDate.Year;

            if (user.BirthDate>currentDate.AddYears(-age).Date)
            {
                age--;
            }
            return age >= 18; 

           
        }

        async Task SendWelcomeEmail(User user)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("fzumbadoz@ucenfotec.ac.cr", name: "Felix Zumbado");
            var subject = $"Bienvenido a CenfoCinemas, es un placer conocerte, {user.Name}!";
            var to = new EmailAddress(user.Email, user.Name);
            var plainTextContent = $"Hola {user.Name},\n\nGracias por registrarte en Cenfocinemas. Estamos emocionados de tenerte con nosotros.\n\nSaludos, \nCenfoCinemas Team";
            var htmlContent = $"<strong>Hola {user.Name},</strong><br><br>Gracias por registrarte en CenfoCinemas. Estamos emocionados de tenerte con nosotros. <br> <br> Saludos, <br> CenfoCinemas Team";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

        }  
            

    }
}
