using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace OpenMediaWebApi.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        [BsonElement("username")]
        public string Username { get; set; }

        [BsonRequired]
        [BsonElement("email")]
        public string Email { get; set; }

        [BsonRequired]
        [BsonElement("password")]
        public string Password { get; set; }

        [BsonRequired]
        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonRequired]
        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("bio")]
        [BsonRepresentation(BsonType.String)]
        public string Bio { get; set; }

        [BsonRequired]
        [BsonElement("dateOfBirth")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DateOfBirth { get; set; }

        [BsonRequired]
        [BsonElement("gender")]
        public string Gender { get; set; }

        [BsonElement("profilePicture")]
        public string ProfilePicture { get; set; }

        [BsonElement("coverPicture")]
        public string CoverPicture { get; set; }

        [BsonElement("isProfileSetup")]
        public bool IsProfileSetup { get; set; }

        [BsonElement("followers")]
        public List<ObjectId> Followers { get; set; }

        [BsonElement("followings")]
        public List<ObjectId> Followings { get; set; }

        [BsonElement("friends")]
        public List<ObjectId> Friends { get; set; }

        [BsonElement("friendships")]
        public List<ObjectId> Friendships { get; set; }

        [BsonElement("friendsCount")]
        public int FriendsCount { get; set; }

        [BsonElement("followersCount")]
        public int FollowersCount { get; set; }

        [BsonElement("followingsCount")]
        public int FollowingsCount { get; set; }

        [BsonElement("blockedUsers")]
        public List<ObjectId> BlockedUsers { get; set; }

        [BsonElement("posts")]
        public List<ObjectId> Posts { get; set; }

        [BsonElement("likedPosts")]
        public List<ObjectId> LikedPosts { get; set; }

        [BsonElement("savedPosts")]
        public List<ObjectId> SavedPosts { get; set; }

        [BsonElement("comments")]
        public List<ObjectId> Comments { get; set; }

        [BsonElement("likedComments")]
        public List<ObjectId> LikedComments { get; set; }

        [BsonElement("sharedPosts")]
        public List<ObjectId> SharedPosts { get; set; }

        [BsonElement("notifications")]
        public List<ObjectId> Notifications { get; set; }

        [BsonElement("messages")]
        public List<ObjectId> Messages { get; set; }

        [BsonElement("conversations")]
        public List<ObjectId> Conversations { get; set; }

        [BsonElement("isAdmin")]
        public bool IsAdmin { get; set; }

        [BsonElement("isOnline")]
        public bool IsOnline { get; set; }

        [BsonElement("lastSeen")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime LastSeen { get; set; }

        [BsonElement("refreshToken")]
        [BsonRepresentation(BsonType.String)]
        public string RefreshToken { get; set; }

        [BsonElement("createdAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdatedAt { get; set; }
    }
}
