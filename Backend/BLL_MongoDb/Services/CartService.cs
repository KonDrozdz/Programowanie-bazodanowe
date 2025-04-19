using BLL.ServiceInterfaces;
using BLL.DTOModels;
using BLL_MongoDb.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace BLL_MongoDb.Services
{
    public class BasketService : IShoppingCartService
    {
        private readonly IMongoCollection<Basket> _baskets;
        private readonly IMongoCollection<Product> _products;

        public BasketService(IMongoDatabase db)
        {
            _baskets = db.GetCollection<Basket>("Baskets");
            _products = db.GetCollection<Product>("Products");
        }

        public async Task AddProductToBasketAsync(int productId, int userId, int amount)
        {
            
            var product = await _products.Find(p => p.Id == productId.ToString() && p.IsActive).FirstOrDefaultAsync();
            if (product == null)
                throw new KeyNotFoundException("Product not found or inactive");

            var filter = Builders<Basket>.Filter.Eq(b => b.UserId, userId.ToString());
            var update = Builders<Basket>.Update.Push(b => b.Items, new BasketItem
            {
                ProductId = productId.ToString(),
                Amount = amount,
                AddedAt = DateTime.UtcNow
            });

            await _baskets.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }

        public async Task UpdateProductQuantityInBasketAsync(int productId, int userId, int newAmount)
        {
            var filter = Builders<Basket>.Filter.And(
                Builders<Basket>.Filter.Eq(b => b.UserId, userId.ToString()),
                Builders<Basket>.Filter.ElemMatch(b => b.Items, i => i.ProductId == productId.ToString())
            );

            var update = Builders<Basket>.Update.Set("Items.$.Amount", newAmount);
            var result = await _baskets.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                throw new KeyNotFoundException("Product not found in basket");
        }

        public async Task RemoveProductFromBasketAsync(int productId, int userId)
        {
            var filter = Builders<Basket>.Filter.Eq(b => b.UserId, userId.ToString());
            var update = Builders<Basket>.Update.PullFilter(b => b.Items,
                i => i.ProductId == productId.ToString());

            var result = await _baskets.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                throw new KeyNotFoundException("Basket or product not found");
        }

    }
}