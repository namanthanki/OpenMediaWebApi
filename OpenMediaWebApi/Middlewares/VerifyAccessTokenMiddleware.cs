using Microsoft.IdentityModel.Tokens;
using OpenMediaWebApi.Models;
using OpenMediaWebApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace OpenMediaWebApi.Middlewares
{
    public class VerifyAccessTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UsersService _usersService;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<VerifyAccessTokenMiddleware> _logger;

        public VerifyAccessTokenMiddleware(RequestDelegate next, UsersService usersService, JwtSettings jwtSettings, ILogger<VerifyAccessTokenMiddleware> logger)
        {
            _next = next;
            _usersService = usersService;
            _jwtSettings = jwtSettings;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/v1/auth/login") || context.Request.Path.StartsWithSegments("/api/v1/auth/register"))
            {
                await _next(context);
                return;
            }


            var accessToken = context.Request.Cookies["accessToken"].ToString().Split(" ").Last() ?? context.Request.Headers["Authorization"].ToString().Split(" ").Last();

            if (string.IsNullOrEmpty(accessToken))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = new JwtSecurityTokenHandler().ValidateToken(accessToken, tokenValidationParameters, out var validatedToken);

                var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _usersService.GetAsync(userId);

                if (user == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }

                context.Items["User"] = user;
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            await _next(context);
        }
    }
}
