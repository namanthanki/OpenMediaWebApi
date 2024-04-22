using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenMediaWebApi.Models;

namespace OpenMediaWebApi.Services
{
    public class FollowUnfollowModel
    {
        public User _user;
        public User _follow;
    }

    public class UserService
    {
        private readonly UsersService _usersService;
        private readonly IMongoCollection<User> _usersCollection;

        public UserService(
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

        public async Task<User> SetupProfile(string userId, IFormFile profilePicture, IFormFile coverPicture, string bio)
        {
            try
            {
                var user = await _usersService.GetAsync(userId);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                user.ProfilePicture = await SaveFileAsync(profilePicture);
                user.CoverPicture = await SaveFileAsync(coverPicture);
                user.Bio = bio;
                user.IsProfileSetup = true;

                await _usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user);

                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<FollowUnfollowModel> Follow(string userId, string followId)
        {
            try
            {
                var user = await _usersService.GetAsync(userId);
                var follow = await _usersService.GetAsync(followId);

                if (user == null || follow == null)
                {
                    throw new Exception("User not found");
                }

                if (!user.Followings.Contains(MongoDB.Bson.ObjectId.Parse(followId)))
                {
                    user.Followings.Add(MongoDB.Bson.ObjectId.Parse(followId));
                    user.FollowingsCount++;
                }

                if (!follow.Followers.Contains(MongoDB.Bson.ObjectId.Parse(followId)))
                {
                    follow.Followers.Add(MongoDB.Bson.ObjectId.Parse(followId));
                    follow.FollowersCount++;
                }

                await _usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
                await _usersCollection.ReplaceOneAsync(u => u.Id == follow.Id, follow);

                return new FollowUnfollowModel
                {
                    _user = user,
                    _follow = follow
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<FollowUnfollowModel> Unfollow(string userId, string followId)
        {
            try
            {
                var user = await _usersService.GetAsync(userId);
                var follow = await _usersService.GetAsync(followId);

                if (user == null || follow == null)
                {
                    throw new Exception("User not found");
                }

                if (user.Followings.Contains(MongoDB.Bson.ObjectId.Parse(followId)))
                {
                    user.Followings.Remove(MongoDB.Bson.ObjectId.Parse(followId));
                    user.FollowingsCount--;
                }

                if (follow.Followers.Contains(MongoDB.Bson.ObjectId.Parse(followId)))
                {
                    follow.Followers.Remove(MongoDB.Bson.ObjectId.Parse(followId));
                    follow.FollowersCount--;
                }

                await _usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
                await _usersCollection.ReplaceOneAsync(u => u.Id == follow.Id, follow);

                return new FollowUnfollowModel
                {
                    _user = user,
                    _follow = follow
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var directoryPath = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}
