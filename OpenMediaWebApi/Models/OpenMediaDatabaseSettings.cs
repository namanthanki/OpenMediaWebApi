namespace OpenMediaWebApi.Models
{
    public class OpenMediaDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string UsersCollectionName { get; set; } = null!;
        public string PostsCollectionName { get; set; } = null!;
        public string CommentsCollectionName { get; set; } = null!;
        public string ConversationsCollectionName { get; set; } = null!;
        public string MessagesCollectionName { get; set; } = null!;
        public string FriendshipsCollectionName { get; set; } = null!;
    }
}