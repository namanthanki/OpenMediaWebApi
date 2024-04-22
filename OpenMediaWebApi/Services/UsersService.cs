using MongoDB.Driver;
using OpenMediaWebApi.Models;
using Microsoft.Extensions.Options;

namespace OpenMediaWebApi.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UsersService(
            IOptions<OpenMediaDatabaseSettings> openMediaDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                openMediaDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                               openMediaDatabaseSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                openMediaDatabaseSettings.Value.UsersCollectionName);
        }

        public async Task<List<User>> GetAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        public async Task<User?> GetAsync(string id) =>
            await _usersCollection.Find(user => user.Id == id).FirstOrDefaultAsync();

        public async Task<User?> FindByField(string field, string value) {
            var filter = Builders<User>.Filter.Eq(field, value);

            return await _usersCollection.Find(filter)
                               .SingleOrDefaultAsync();
        }

        public async Task CreateAsync(User user) =>
            await _usersCollection.InsertOneAsync(user);

        public async Task UpdateAsync(string id, User updatedUser) =>
            await _usersCollection.ReplaceOneAsync(user => user.Id == id, updatedUser);

        public async Task RemoveAsync(string id) =>
            await _usersCollection.DeleteOneAsync(user => user.Id == id);
    }
}
