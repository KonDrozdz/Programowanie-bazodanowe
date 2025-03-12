using BLL.DTOModels;
using BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ServiceInterfaces
{
    public interface IProductGroupService
    {

        Task<ProductGroupResponseDTO> AddProductGroupAsync(ProductGroupRequestDTO groupRequest);


        Task<IEnumerable<ProductGroupResponseDTO>> GetProductGroupsAsync(int? parentId = null, SortOrder sortOrder = SortOrder.NameAscending);
    }
}
