using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BLL_MongoDb.Models
{
    public class Basket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }  

        public List<BasketItem> Items { get; set; } = new List<BasketItem>(); 
    }

    public class BasketItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }  

        public int Amount { get; set; }  

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;  
    }
}