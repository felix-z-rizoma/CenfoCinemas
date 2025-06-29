using CoreApp;
using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Create(Movie movie)
        {
            try
            {
                if (movie == null)
                    return BadRequest("Movie data is required");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Await the async version of Create
                var mm = new MovieManager();
                var createdMovie = await mm.Create(movie);

                return CreatedAtAction(
                    actionName: nameof(RetrieveById),
                    routeValues: new { id = createdMovie.Id },
                    value: new
                    {
                        Success = true,
                        Movie = createdMovie
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                return Conflict(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating movie: {ex}");

                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while creating the movie",
                    ReferenceId = Guid.NewGuid()
                });
            }
        }


        [HttpGet]
        [Route("RetrieveById/{id}")]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult RetrieveById(int id)
        {
            try
            {
                var mm = new MovieManager();
                var movie = mm.RetrieveById(id);

                if (movie == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = $"Movie with ID {id} not found"
                    });

                return Ok(new
                {
                    Success = true,
                    Movie = movie
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
        [Route("RetrieveAll")]
        [ProducesResponseType(typeof(Movie[]), StatusCodes.Status200OK)]
        public ActionResult RetrieveAll()
        {
            try
            {
                var mm = new MovieManager();
                var movies = mm.RetrieveAll();

                return Ok(new
                {
                    Success = true,
                    Count = movies.Count,
                    Movies = movies
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

        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Update(Movie movie)
        {
            try
            {
                if (movie == null)
                    return BadRequest("Movie data is required");

                var mm = new MovieManager();
                var updatedMovie = mm.Update(movie);

                return Ok(new
                {
                    Success = true,
                    Movie = updatedMovie
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
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

        [HttpDelete]
        [Route("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete(int id)
        {
            try
            {
                var mm = new MovieManager();
                mm.Delete(id);

                return NoContent(); // 204 for successful delete
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
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
    }
}