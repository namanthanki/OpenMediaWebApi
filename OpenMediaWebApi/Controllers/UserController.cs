using OpenMediaWebApi.Services;
using OpenMediaWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace OpenMediaWebApi.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly UsersService _usersService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService userService, UsersService usersService, ILogger<UserController> logger)
        {
            _userService = userService;
            _usersService = usersService;
            _logger = logger;
        }

        [HttpPost("setup-profile")]
        public async Task<IActionResult> SetupProfile([FromForm] string bio,[FromForm] IFormFile profilePicture,[FromForm] IFormFile coverPicture)
        {
            var user = HttpContext.Items["User"] as User;
            _logger.LogInformation("Bio: {0}", bio);

            if (user == null)
            {
                   return Unauthorized(new { message = "Unauthorized" });
            }

            try
            {
                var response = await _userService.SetupProfile(user.Id, profilePicture, coverPicture, bio);
                if (response == null)
                {
                    return BadRequest(new { message = "Profile setup failed" });
                }
                return Ok(new { response, message = "Profile setup successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
    }
}
