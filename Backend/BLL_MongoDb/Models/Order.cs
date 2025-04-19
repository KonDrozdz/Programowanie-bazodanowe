using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace BLL_MongoDb.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } 

        public DateTime Date { get; set; } = DateTime.UtcNow;
        public bool IsPaid { get; set; }
        public decimal TotalPrice { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        public string ProductName { get; set; } 
        public decimal Price { get; set; }
        public int Amount { get; set; }
    }
}