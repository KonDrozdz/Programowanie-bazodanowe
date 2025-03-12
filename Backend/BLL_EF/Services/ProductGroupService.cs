using BLL.DTOModels;
using BLL.Enums;
using BLL.ServiceInterfaces;
using DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Model;

namespace BLL_EF.Services
{
    public class ProductGroupService : IProductGroupService
    {
        private readonly WebstoreContext _context;

        public ProductGroupService(WebstoreContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<ProductGroupResponseDTO>> GetProductGroupsAsync(int? parentId = null, SortOrder sortOrder = SortOrder.NameAscending)
        {
            var query = _context.ProductGroups.AsQueryable();

            if (parentId.HasValue)
            {
                query = query.Where(g => g.ParentID == parentId.Value);
            }

            var result = await query.Select(g => new ProductGroupResponseDTO
            {
                Id = g.ID,
                Name = g.Name
            }).ToListAsync();

            return result;
        }


        public async Task<ProductGroupResponseDTO> AddProductGroupAsync(ProductGroupRequestDTO groupRequest)
        {
            var newGroup = new ProductGroup
            {
                Name = groupRequest.Name,
                ParentID = groupRequest.ParentId
            };

            _context.ProductGroups.Add(newGroup);
            await _context.SaveChangesAsync();

            return new ProductGroupResponseDTO
            {
                Id = newGroup.ID,
                Name = newGroup.Name
            };
        }


    }
}
