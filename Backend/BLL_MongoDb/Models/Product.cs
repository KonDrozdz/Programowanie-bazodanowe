using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BLL_MongoDb.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public double Price { get; set; }
        public bool IsActive { get; set; } = true;

        [BsonRepresentation(BsonType.ObjectId)]
        public string GroupId { get; set; } 
    }
}