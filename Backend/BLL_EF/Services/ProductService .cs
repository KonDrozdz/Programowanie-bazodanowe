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
using SortOrder = BLL.Enums.SortOrder;

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
            var query = _context.Products
                .Include(p => p.ProductGroup)
                .ThenInclude(pg => pg.ParentGroup)
                .AsQueryable();

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
                var groupIds = await _context.ProductGroups
                    .ToListAsync();

                var matchingGroupIds = new List<int>();
                foreach (var group in groupIds)
                {
                    var fullGroupName = await GetFullGroupNameAsync(_context, group.ID);
                    if (fullGroupName.Contains(groupName))
                    {
                        matchingGroupIds.Add(group.ID);
                    }
                }

                query = query.Where(p => matchingGroupIds.Contains(p.GroupID.Value));
            }

            if (groupId.HasValue)
            {
                query = query.Where(p => p.GroupID == groupId.Value);
            }

            query = sortOrder switch
            {
                SortOrder.NameAscending => query.OrderBy(p => p.Name),
                SortOrder.NameDescending => query.OrderByDescending(p => p.Name),
                SortOrder.PriceAscending => query.OrderBy(p => p.Price),
                SortOrder.PriceDescending => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Name),
            };

            var products = await query.ToListAsync();

            var productResponseDTOs = new List<ProductResponseDTO>();
            foreach (var product in products)
            {
                var fullGroupName = product.ProductGroup != null
                    ? await GetFullGroupNameAsync(_context, product.ProductGroup.ID)
                    : string.Empty;

                productResponseDTOs.Add(new ProductResponseDTO
                {
                    Id = product.ID,
                    Name = product.Name,
                    Price = product.Price,
                    GroupName = fullGroupName,
                    IsActive = product.IsActive
                });
            }

            return productResponseDTOs;
        }

        private static async Task<string> GetFullGroupNameAsync(WebstoreContext context, int groupId)
        {
            var group = await context.ProductGroups
                .Include(pg => pg.ParentGroup)
                .FirstOrDefaultAsync(pg => pg.ID == groupId);

            if (group == null)
            {
                throw new ArgumentException("Group not found.");
            }

            var groupNames = new List<string>();
            var currentGroup = group;

            while (currentGroup != null)
            {
                groupNames.Add(currentGroup.Name);

                currentGroup = await context.ProductGroups
                    .Include(pg => pg.ParentGroup)
                    .FirstOrDefaultAsync(pg => pg.ID == currentGroup.ParentID);
            }

            groupNames.Reverse();
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