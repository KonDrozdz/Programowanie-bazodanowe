using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BLL_MongoDb.Models
{
    // Dla zachowania spójności nazewnictwa (jeśli potrzebne)
    [BsonIgnoreExtraElements]
    public class OrderPosition : OrderItem { }
}