using Microsoft.AspNetCore.Mvc;
using OpenMediaWebApi.Services;
using OpenMediaWebApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace OpenMediaWebApi.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticationService _authService;
        private readonly UsersService _usersService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthenticationService authService, UsersService usersService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _usersService = usersService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var response = await _authService.Register(
                    model.FirstName, model.LastName, model.Username,
                    model.Email, model.Password, model.DateOfBirth, model.Gender
                );
                return CreatedAtAction(nameof(Register), new { user = response.User.Id, accessToken = response.Token }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var response = await _authService.Login(model.Email, model.Password);

                Response.Cookies.Append("accessToken", response.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                });

                Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                }); 

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            //var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = HttpContext.Items["User"] as User;
            //var user = await _usersService.GetAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.RefreshToken = "";

            await _usersService.UpdateAsync(user.Id, user);

            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
            return Ok(new { message = "User logged out successfully!" });
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"].ToString().Split(" ").Last();
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            //var user = await _usersService.GetAsync(refreshToken);

            var user = await _usersService.FindByField("refreshToken", refreshToken);
            if (user == null || user.RefreshToken != refreshToken)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            var accessToken = _authService.GenerateToken(user);
            var newRefreshToken = _authService.GenerateRefreshToken(user);

            user.RefreshToken = newRefreshToken;
            await _usersService.UpdateAsync(user.Id, user);

            Response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { message = "Token refreshed successfully!" });
        }   
    }
}
