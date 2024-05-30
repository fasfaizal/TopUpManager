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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserRequestModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.CreateUserAsync(userModel);
            return Ok(user);
        }
    }
}
