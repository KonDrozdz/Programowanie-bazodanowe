using BLL.DTOModels;
using BLL.Enums;
using BLL.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductGroupsController : ControllerBase
    {
        private readonly IProductGroupService _productGroupService;

        public ProductGroupsController(IProductGroupService productGroupService)
        {
            _productGroupService = productGroupService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductGroupResponseDTO>>> GetProductGroupsAsync(
            int? parentId = null,
            SortOrder sortOrder = SortOrder.NameAscending)
        {
            var productGroups = await _productGroupService.GetProductGroupsAsync(parentId, sortOrder);
            return Ok(productGroups);
        }

        [HttpPost]
        public async Task<ActionResult<ProductGroupResponseDTO>> AddProductGroupAsync(ProductGroupRequestDTO groupRequest)
        {
            var productGroup = await _productGroupService.AddProductGroupAsync(groupRequest);
            return CreatedAtAction(nameof(GetProductGroupsAsync), new { id = productGroup.Id }, productGroup);
        }
    }
}
