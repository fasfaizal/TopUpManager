using Microsoft.AspNetCore.Mvc;
using TopUpManager.Common.Interfaces.Services;
using TopUpManager.Common.Models.Request;

namespace TopUpManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Creates a new user based on the provided user request model asynchronously.
        /// </summary>
        /// <param name="userModel">
        /// The user request model containing the details of the user to be created.
        /// </param>
        /// <returns>
        /// Returns the newly created user.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserRequestModel userModel)
        {
            var user = await _userService.CreateUserAsync(userModel);
            return Ok(user);
        }
    }
}
