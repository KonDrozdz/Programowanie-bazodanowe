using BLL.ServiceInterfaces;
using BLL.DTOModels;
using BLL_MongoDb.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

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

        public IEnumerable<ProductResponseDTO> GetProducts(
            string sortBy = "Name",
            bool ascending = true,
            string nameFilter = null,
            string groupNameFilter = null,
            int? groupIdFilter = null,
            bool includeInactive = false)
        {
            // Filtrowanie
            var filterBuilder = Builders<Product>.Filter;
            var filter = includeInactive
                ? filterBuilder.Empty
                : filterBuilder.Eq(p => p.IsActive, true);

            if (!string.IsNullOrEmpty(nameFilter))
                filter &= filterBuilder.Regex(p => p.Name, new BsonRegularExpression(nameFilter, "i"));

            if (groupIdFilter.HasValue)
                filter &= filterBuilder.Eq(p => p.GroupId, groupIdFilter.Value.ToString());

            // Sortowanie
            var sort = ascending
                ? Builders<Product>.Sort.Ascending(sortBy)
                : Builders<Product>.Sort.Descending(sortBy);

            // Pobieranie danych
            var products = _products.Find(filter).Sort(sort).ToList();
            var groupIds = products.Select(p => p.GroupId).Distinct().ToList();
            var groups = _groups.Find(g => groupIds.Contains(g.Id)).ToList().ToDictionary(g => g.Id);

            // Mapowanie
            return products.Select(p => new ProductResponseDTO
            {
                Id = int.Parse(p.Id),
                Name = p.Name,
                Price = p.Price,
                GroupId = int.Parse(p.GroupId),
                GroupName = groups.TryGetValue(p.GroupId, out var group) ? group.Name : "Unknown",
                IsActive = p.IsActive
            });
        }

        public void AddProduct(ProductRequestDTO productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                GroupId = productDto.GroupId.ToString(),
                IsActive = true
            };
            _products.InsertOne(product);
        }

        public void DeactivateProduct(int productId)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, productId.ToString());
            var update = Builders<Product>.Update.Set(p => p.IsActive, false);
            _products.UpdateOne(filter, update);
        }

        public void ActivateProduct(int productId)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, productId.ToString());
            var update = Builders<Product>.Update.Set(p => p.IsActive, true);
            _products.UpdateOne(filter, update);
        }
    }
}