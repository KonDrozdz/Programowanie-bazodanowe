using BLL.DTOModels;
using BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ServiceInterfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDTO>> GetProductsAsync(
            string? name = null,
            string? groupName = null,
            int? groupId = null,
            bool onlyActive = true,
            SortOrder sortOrder = SortOrder.NameAscending
        );


        Task<ProductResponseDTO> AddProductAsync(ProductRequestDTO productRequest);


        Task DeactivateProductAsync(int productId);


        Task ActivateProductAsync(int productId);


        Task DeleteProductAsync(int productId);
    }
}
