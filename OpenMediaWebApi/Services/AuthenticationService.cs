using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenMediaWebApi.Models;

namespace OpenMediaWebApi.Services
{
    public class AuthenticationService
    {
        private readonly UsersService _usersService;
        private readonly IMongoCollection<User> _usersCollection;

        public AuthenticationService(
            UsersService usersService,
            IOptions<OpenMediaDatabaseSettings> openMediaDatabaseSettings
        )
        {
            var mongoClient = new MongoClient(
                openMediaDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                               openMediaDatabaseSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                openMediaDatabaseSettings.Value.UsersCollectionName);

            _usersService = usersService;
        }

        public async Task<User> Register(string firstName, string lastName, string username, string email, string password, DateTime dateOfBirth, string gender)
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

            return user;
        }

        public async Task<User> Login(string email, string password)
        {
            var user = await _usersCollection.Find(user => user.Email == email).FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new Exception("Invalid email or password");
            }

            return user;
        }
    }
}
