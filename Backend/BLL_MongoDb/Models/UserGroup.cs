using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BLL_MongoDb.Models
{
    public class UserGroup
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}