using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Driver.Core.WireProtocol.Messages;
using OpenMediaWebApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OpenMediaWebApi.Services
{
    public class UserRegistrationResponse
    {
        public User User { get; set; }
        public string Token { get; set; }
    }

    public class UserLoginResponse
    {
        public User User { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool IsProfileSetup { get; set; }
    }

    public class AuthenticationService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UsersService _usersService;
        private readonly IMongoCollection<User> _usersCollection;

        public AuthenticationService(
            JwtSettings jwtSettings,
            UsersService usersService,
            IOptions<OpenMediaDatabaseSettings> openMediaDatabaseSettings
        )
        {
            _jwtSettings = jwtSettings;

            var mongoClient = new MongoClient(
                openMediaDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                               openMediaDatabaseSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                openMediaDatabaseSettings.Value.UsersCollectionName);

            _usersService = usersService;
        }

        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserRegistrationResponse> Register(string firstName, string lastName, string username, string email, string password, DateTime dateOfBirth, string gender)
        {

            var existingUser = await _usersCollection.Find(user => user.Email == email || user.Username == username).FirstOrDefaultAsync();

            if (existingUser != null)
            {
                throw new Exception("User with this email or username already exists");
            }

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                DateOfBirth = dateOfBirth,
                Gender = gender,
            };

            await _usersService.CreateAsync(user);

            var token = GenerateToken(user);

            return new UserRegistrationResponse
            {
                User = new User {
                    Id = user.Id,
                },
                Token = token
            };
        }

        public async Task<UserLoginResponse> Login(string email, string password)
        {
            var user = await _usersCollection.Find(user => user.Email == email).FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new Exception("Invalid email or password");
            }

            var accessToken = GenerateToken(user);
            var refreshToken = GenerateRefreshToken(user);

            user.RefreshToken = refreshToken;
            await _usersService.UpdateAsync(user.Id, user);

            accessToken = "Bearer " + accessToken;
            refreshToken = "Bearer " + refreshToken;


            return new UserLoginResponse
            {
                User = user,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IsProfileSetup = user.IsProfileSetup
            };
        }
    }
}
