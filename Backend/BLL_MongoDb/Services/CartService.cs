using BLL.ServiceInterfaces;
using BLL.DTOModels;
using BLL_MongoDb.Models;
using MongoDB.Driver;
using System.Linq;

namespace BLL_MongoDb.Services
{
    public class CartService : ICartService
    {
        private readonly IMongoCollection<Cart> _carts;
        private readonly IMongoCollection<Product> _products;

        public CartService(IMongoDatabase db)
        {
            _carts = db.GetCollection<Cart>("Carts");
            _products = db.GetCollection<Product>("Products");
        }

        public void AddToCart(int userId, int productId, int amount)
        {
            // Walidacja produktu
            if (!_products.Find(p => p.Id == productId.ToString() && p.IsActive).Any())
                return;

            var filter = Builders<Cart>.Filter.Eq(c => c.UserId, userId.ToString());
            var update = Builders<Cart>.Update.Push(c => c.Items, new CartItem
            {
                ProductId = productId.ToString(),
                Amount = amount
            });

            _carts.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
        }

        public void RemoveFromCart(int userId, int productId)
        {
            var filter = Builders<Cart>.Filter.Eq(c => c.UserId, userId.ToString());
            var update = Builders<Cart>.Update.PullFilter(c => c.Items,
                i => i.ProductId == productId.ToString());

            _carts.UpdateOne(filter, update);
        }

        public void UpdateCartItemAmount(int userId, int productId, int newAmount)
        {
            var filter = Builders<Cart>.Filter.And(
                Builders<Cart>.Filter.Eq(c => c.UserId, userId.ToString()),
                Builders<Cart>.Filter.ElemMatch(c => c.Items, i => i.ProductId == productId.ToString())
            );

            var update = Builders<Cart>.Update.Set("Items.$.Amount", newAmount);
            _carts.UpdateOne(filter, update);
        }

        public CartResponseDTO GetCart(int userId)
        {
            var cart = _carts.Find(c => c.UserId == userId.ToString()).FirstOrDefault();
            if (cart == null) return new CartResponseDTO { Items = new List<CartItemResponseDTO>() };

            var productIds = cart.Items.Select(i => i.ProductId).ToList();
            var products = _products.Find(p => productIds.Contains(p.Id))
                .ToList()
                .ToDictionary(p => p.Id);

            return new CartResponseDTO
            {
                Items = cart.Items.Select(item => new CartItemResponseDTO
                {
                    ProductId = int.Parse(item.ProductId),
                    ProductName = products.TryGetValue(item.ProductId, out var product)
                        ? product.Name
                        : "Unknown",
                    Price = products.TryGetValue(item.ProductId, out var p)
                        ? p.Price
                        : 0,
                    Amount = item.Amount
                }).ToList()
            };
        }
    }
}