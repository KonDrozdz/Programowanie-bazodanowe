using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace BLL_MongoDb.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Login { get; set; }
        public string PasswordHash { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> UserGroupIds { get; set; } = new List<string>();
    }
}