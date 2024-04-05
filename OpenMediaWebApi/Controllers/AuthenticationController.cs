using Microsoft.AspNetCore.Mvc;
using OpenMediaWebApi.Services;
using OpenMediaWebApi.Models;

namespace OpenMediaWebApi.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticationService _authService;

        public AuthController(AuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterModel model)
        {
            try
            {
                var user = await _authService.Register(
                    model.FirstName, model.LastName, model.Username,
                    model.Email, model.Password, model.DateOfBirth, model.Gender
                );
                return CreatedAtAction(nameof(Register), user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _authService.Login(model.Email, model.Password);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
