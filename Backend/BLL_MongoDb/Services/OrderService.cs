using BLL.ServiceInterfaces;
using BLL.DTOModels;
using BLL_MongoDb.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using BLL.Enums;

namespace BLL_MongoDb.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<Basket> _baskets;
        private readonly IMongoCollection<Product> _products;

        public OrderService(IMongoDatabase db)
        {
            _orders = db.GetCollection<Order>("Orders");
            _baskets = db.GetCollection<Basket>("Baskets");
            _products = db.GetCollection<Product>("Products");
        }

        public async Task<OrderResponseDTO> GenerateOrderAsync(int userId)
        {
            var basket = await _baskets.Find(b => b.UserId == userId.ToString()).FirstOrDefaultAsync();
            if (basket == null || !basket.Items.Any())
                throw new InvalidOperationException("Basket is empty");

            
            var productIds = basket.Items.Select(i => i.ProductId).ToList();
            var products = await _products.Find(p => productIds.Contains(p.Id))
                                        .ToListAsync();
            var productDict = products.ToDictionary(p => p.Id);

            
            var totalAmount = basket.Items.Sum(item =>
                productDict.TryGetValue(item.ProductId, out var product)
                    ? product.Price * item.Amount
                    : 0);

            
            var order = new Order
            {
                UserId = userId.ToString(),
                Date = DateTime.UtcNow,
                IsPaid = false,
                TotalPrice = totalAmount,
                Items = basket.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = productDict.TryGetValue(item.ProductId, out var p) ? p.Name : "Unknown",
                    Price = (double)(productDict.TryGetValue(item.ProductId, out var prod) ? prod.Price : 0),
                    Amount = item.Amount
                }).ToList()
            };

            await _orders.InsertOneAsync(order);
            await _baskets.DeleteOneAsync(b => b.UserId == userId.ToString());

            return new OrderResponseDTO
            {
                Id = int.Parse(order.Id),
                UserId = userId,
                Date = order.Date,
                TotalAmount = (double)order.TotalPrice,
                IsPaid = order.IsPaid
            };
        }

        public async Task<IEnumerable<OrderPositionResponseDTO>> GetOrderPositionsAsync(int orderId)
        {
            var order = await _orders.Find(o => o.Id == orderId.ToString()).FirstOrDefaultAsync();
            if (order == null)
                return Enumerable.Empty<OrderPositionResponseDTO>();

            return order.Items.Select(item => new OrderPositionResponseDTO
            {
                ProductId = int.Parse(item.ProductId),
                ProductName = item.ProductName,
                ProductPrice = item.Price,
                Amount = item.Amount
            });
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersAsync(
            int? orderId = null,
            bool? isPaid = null,
            SortOrder sortOrder = SortOrder.DateAscending)
        {
            var filter = Builders<Order>.Filter.Empty;

            if (orderId.HasValue)
                filter &= Builders<Order>.Filter.Eq(o => o.Id, orderId.Value.ToString());

            if (isPaid.HasValue)
                filter &= Builders<Order>.Filter.Eq(o => o.IsPaid, isPaid.Value);

            var sortDefinition = sortOrder switch
            {
                SortOrder.DateDescending => Builders<Order>.Sort.Descending(o => o.Date),
                SortOrder.AmountAscending => Builders<Order>.Sort.Ascending(o => o.TotalPrice),
                SortOrder.AmountDesc => Builders<Order>.Sort.Descending(o => o.TotalPrice),
                _ => Builders<Order>.Sort.Ascending(o => o.Date) 
            };

            var orders = await _orders.Find(filter)
                                    .Sort(sortDefinition)
                                    .ToListAsync();

            return orders.Select(o => new OrderResponseDTO
            {
                Id = int.Parse(o.Id),
                UserId = int.Parse(o.UserId),
                Date = o.Date,
                TotalAmount = (double)o.TotalPrice,
                IsPaid = o.IsPaid
            });
        }

        public async Task PayForOrderAsync(int orderId, double amountPaid)
        {
            var order = await _orders.Find(o => o.Id == orderId.ToString()).FirstOrDefaultAsync();
            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.IsPaid)
                throw new InvalidOperationException("Order is already paid");

            if ((decimal)amountPaid < order.TotalPrice) 
                throw new ArgumentException("Paid amount is less than order total");

            var update = Builders<Order>.Update.Set(o => o.IsPaid, true);
            await _orders.UpdateOneAsync(o => o.Id == orderId.ToString(), update);
        }

    }
}