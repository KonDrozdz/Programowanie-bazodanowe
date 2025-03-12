using BLL.DTOModels;
using BLL.ServiceInterfaces;
using DAL;
using Microsoft.Data.SqlClient;
using BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Model;

namespace BLL_EF.Services
{
    public class ProductService : IProductService
    {
        private readonly WebstoreContext _context;

        public ProductService(WebstoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetProductsAsync(
            string? name = null,
            string? groupName = null,
            int? groupId = null,
            bool onlyActive = true,
            BLL.Enums.SortOrder sortOrder = BLL.Enums.SortOrder.NameAscending)
        {
            var query = _context.Products.AsQueryable();

            if (onlyActive)
            {
                query = query.Where(p => p.IsActive);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(groupName))
            {
                query = query.Where(p => p.ProductGroup.Name.Contains(groupName));
            }

            if (groupId.HasValue)
            {
                query = query.Where(p => p.GroupID == groupId.Value);
            }

            query = query.Include(p => p.ProductGroup)
                 .ThenInclude(pg => pg.ParentGroup);
            query = sortOrder switch
            {
                BLL.Enums.SortOrder.NameAscending => query.OrderBy(p => p.Name),
                BLL.Enums.SortOrder.NameDescending => query.OrderByDescending(p => p.Name),
                BLL.Enums.SortOrder.PriceAscending => query.OrderBy(p => p.Price),
                BLL.Enums.SortOrder.PriceDescending => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Name),
            };

            var result = await query.Select(p => new ProductResponseDTO
            {
                Id = p.ID,
                Name = p.Name,
                Price = p.Price,
                GroupName = GetFullGroupNameAsync(p.ProductGroup.ID).Result
            }).ToListAsync();

            return result;
        }
        private async Task<string> GetFullGroupNameAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductGroup) 
                .ThenInclude(pg => pg.ParentGroup) 
                .FirstOrDefaultAsync(p => p.ID == productId);

            if (product == null || product.ProductGroup == null)
            {
                throw new ArgumentException("Product not found or doesn't have a group.");
            }

            var groupNames = new List<string>();
            var currentGroup = product.ProductGroup;

            while (currentGroup != null)
            {
                groupNames.Insert(0, currentGroup.Name);

                currentGroup = await _context.ProductGroups
                    .Include(pg => pg.ParentGroup)
                    .FirstOrDefaultAsync(pg => pg.ID == currentGroup.ParentID);
            }

            return string.Join(" / ", groupNames);
        }


        public async Task<ProductResponseDTO> AddProductAsync(ProductRequestDTO productRequest)
        {
            var newProduct = new Product
            {
                Name = productRequest.Name,
                Price = productRequest.Price,
                GroupID = productRequest.GroupId,
                IsActive = true
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return new ProductResponseDTO
            {
                Id = newProduct.ID,
                Name = newProduct.Name,
                Price = newProduct.Price,
                GroupName = newProduct.ProductGroup.Name
            };
        }


        public async Task DeactivateProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }


        public async Task ActivateProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.IsActive = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }


    }
}
