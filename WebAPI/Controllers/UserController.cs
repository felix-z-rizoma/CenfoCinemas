using CoreApp;
using DataAccess.CRUD;
using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WebAPI.Controllers
{
    //Indicamos que la direccion de este controlador 
    //Sera http://servidor:puerto/api/User
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        [Route("Create")]

        public ActionResult Create(User user)
        {
           

            try
            {
                var um=new UserManager();
                um.Create(user);
                return Ok(user);
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult RetrieveAll()
        {

            try
            {
                var um = new UserManager();
                var listResults = um.RetrieveAll();
                return Ok(listResults);

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet]
        [Route("RetrieveById")]
        public ActionResult RetrieveById(int id)
        {
            try
            {
                var uCrud = new UserCrudFactory();
                var user = uCrud.RetrieveById<User>(id);

                if (user == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = $"Usuario con ID {id} no encontrado"
                    });

                return Ok(new
                {
                    Success = true,
                    User = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("RetrieveByEmail")]
        public ActionResult RetrieveByEmail(string email)
        {
            try
            {
                var uCrud = new UserCrudFactory();

                //Crea objeto User con solo el correo poblado.
                var filter = new User { Email = email };

                // Execute query
                var user = uCrud.RetrieveByEmail<User>(filter);

                if (user == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = $"Usuario con correo {email} no encontrado"
                    });

                // Por seguridad se devuelve el password en null
                user.Password = null;

                return Ok(new
                {
                    Success = true,
                    User = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("RetrieveByUserCode")]
        public ActionResult RetrieveByUserCode(string userCode) // Changed parameter to string
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userCode))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Codigo de Usuario requerido"
                    });
                }

                var uCrud = new UserCrudFactory();
                var tempUser = new User { UserCode = userCode };
                var user = uCrud.RetrieveByUserCode<User>(tempUser);

                if (user == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = $"Usuario con codigo '{userCode}' no encontrado"
                    });
                }

             
                user.Password = null;

                return Ok(new
                {
                    Success = true,
                    User = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Un error ha ocurrido al buscar usuario",
                    DetailedError = ex.Message // Only include in development
                });
            }
        }


        [HttpPut]
        [Route("Update")]
        public ActionResult Update(User user)
        {
            try
            {
                if (user == null || user.Id <= 0)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Usuario valido requerido"
                    });
                }

                var uCrud = new UserCrudFactory();
                var existingUser = uCrud.RetrieveById<User>(user.Id);

                if (existingUser == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = $"Usuario con ID {user.Id} no encontrado"
                    });
                }

                // Add business logic validation if needed
                var um = new UserManager();
                if (!um.IsOver18(user))
                {.
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "User must be at least 18 years old"
                    });
                }

                // Prevension de actualizacion no autorizada de datos
                user.Created = existingUser.Created;
                user.UserCode = existingUser.UserCode;

                uCrud.Update(user);

               
                user.Password = null;

                return Ok(new
                {
                    Success = true,
                    Message = "User actualizado exitosamente",
                    User = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "No se pudo actualizar el Usuario",
                    DetailedError = ex.Message // Desarrollo unicamente
                });
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public ActionResult Delete(int id)
        {
            try
            {
                var uCrud = new UserCrudFactory();
                var user = uCrud.RetrieveById<User>(id);

                if (user == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = $"Usuario con ID {id} no encontrado"
                    });

                uCrud.Delete(user);

                return NoContent(); // 204 No Content is standard for successful DELETE
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}
