using BLL.ServiceInterfaces;
using BLL.DTOModels;
using BLL_MongoDb.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using BLL.Enums;

namespace BLL_MongoDb.Services
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<ProductGroup> _groups;

        public ProductService(IMongoDatabase db)
        {
            _products = db.GetCollection<Product>("Products");
            _groups = db.GetCollection<ProductGroup>("ProductGroups");
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetProductsAsync(
            string? name = null, 
            string? groupName = null, 
            int? groupId = null, 
            bool onlyActive = true,
            SortOrder sortOrder = SortOrder.NameAscending)
        {
            
            var filterBuilder = Builders<Product>.Filter;
            var filter = onlyActive 
                ? filterBuilder.Eq(p => p.IsActive, true) 
                : filterBuilder.Empty;

            if (!string.IsNullOrEmpty(name))
                filter &= filterBuilder.Regex(p => p.Name, new BsonRegularExpression(name, "i"));

            if (groupId.HasValue)
                filter &= filterBuilder.Eq(p => p.GroupId, groupId.Value.ToString());

            
            var sortDefinition = sortOrder switch
            {
                SortOrder.NameDescending => Builders<Product>.Sort.Descending(p => p.Name),
                SortOrder.PriceAscending => Builders<Product>.Sort.Ascending(p => p.Price),
                SortOrder.PriceDescending => Builders<Product>.Sort.Descending(p => p.Price),
                _ => Builders<Product>.Sort.Ascending(p => p.Name) 
            };

            
            var products = await _products.Find(filter)
                .Sort(sortDefinition)
                .ToListAsync();

            var groupIds = products.Select(p => p.GroupId).Distinct().ToList();
            var groups = await _groups.Find(g => groupIds.Contains(g.Id))
                .ToListAsync();

            var groupDict = groups.ToDictionary(g => g.Id);

            
            return products.Select(p => new ProductResponseDTO
            {
                Id = int.Parse(p.Id),
                Name = p.Name,
                Price = (double)p.Price,
                GroupName = groupDict.TryGetValue(p.GroupId, out var group) ? group.Name : "Unknown",
                IsActive = p.IsActive
            });
        }

        public async Task<ProductResponseDTO> AddProductAsync(ProductRequestDTO productRequest)
        {
            var product = new Product
            {
                Name = productRequest.Name,
                Price = (decimal)productRequest.Price,
                GroupId = productRequest.GroupId.ToString(),
                IsActive = true
            };

            await _products.InsertOneAsync(product);

            return new ProductResponseDTO
            {
                Id = int.Parse(product.Id),
                Name = product.Name,
                Price = (double)product.Price,
                IsActive = true
            };
        }

        public async Task DeactivateProductAsync(int productId)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, productId.ToString());
            var update = Builders<Product>.Update.Set(p => p.IsActive, false);
            await _products.UpdateOneAsync(filter, update);
        }

        public async Task ActivateProductAsync(int productId)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, productId.ToString());
            var update = Builders<Product>.Update.Set(p => p.IsActive, true);
            await _products.UpdateOneAsync(filter, update);
        }

        public async Task DeleteProductAsync(int productId)
        {
            await _products.DeleteOneAsync(p => p.Id == productId.ToString());
        }
    }
}