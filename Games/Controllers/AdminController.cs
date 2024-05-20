using Games.Models;
using Games.Services.ControllerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Games.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private IAdminControllerService _adminService;

        public AdminController(IAdminControllerService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Get registered users
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns the users</response>
        /// <response code="403">Must be an Admin</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("Users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<IdentityUser>))]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _adminService.GetUsersAsync();

            return Ok(users);
        }

        /// <summary>
        /// Get a specific user
        /// </summary>
        /// <param name="email">The email of the user to get</param>
        /// <returns></returns>
        /// <response code="200">Returns the user</response>
        /// <response code="403">Must be an Admin</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">No user found</response>
        [HttpGet]
        [Route("User/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IdentityUser))]
        public async Task<IActionResult> GetUser([FromRoute] string email)
        {
            try
            {
                var user = await _adminService.GetUserAsync(email);
                return Ok(user);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="email">The email of the user to delete</param>
        /// <returns></returns>
        /// <response code="200">User deleted</response>
        /// <response code="403">Must be an Admin</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">No user found</response>
        [HttpDelete]
        [Route("User/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUser([FromRoute] string email)
        {
            try
            {
                await _adminService.DeleteUserAsync(email);
                return Ok();
            }
            catch (NotFoundException e) 
            {
                return NotFound(e.Message);
            }
        }

        /// <summary>
        /// Get all characters owned by a user
        /// </summary>
        /// <param name="email">The email of the user that owns the characters</param>
        /// <returns></returns>
        /// <response code="200">Returns the characters</response>
        /// <response code="403">Must be an Admin</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">No user found</response>
        [HttpGet]
        [Route("User/{email}/Characters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Character>))]
        public async Task<IActionResult> GetUserCharacters([FromRoute] string email)
        {
            try
            {
                var characters = await _adminService.GetUserCharactersAsync(email);
                return Ok(characters);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        // TODO get all battles

        // TODO get all battles a user is involved in
    }
}
