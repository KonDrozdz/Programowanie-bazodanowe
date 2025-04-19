using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BLL_MongoDb.Models
{
    
    [BsonIgnoreExtraElements]
    public class OrderPosition : OrderItem { }
}