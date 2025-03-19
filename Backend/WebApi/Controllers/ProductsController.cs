using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using WebApi.Model;
using BLL.ServiceInterfaces;
using BLL.DTOModels;
using BLL.Enums;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetProductsAsync(
            string? name = null,
            string? groupName = null,
            int? groupId = null,
            bool onlyActive = true,
            SortOrder sortOrder = SortOrder.NameAscending)
        {
            var products = await _productService.GetProductsAsync(name, groupName, groupId, onlyActive, sortOrder);
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponseDTO>> AddProductAsync(ProductRequestDTO productRequest)
        {
            var product = await _productService.AddProductAsync(productRequest);
            return CreatedAtAction(nameof(GetProductsAsync), new { id = product.Id }, product);
        }

        [HttpPut("deactivate/{id}")]
        public async Task<ActionResult> DeactivateProductAsync(int id)
        {
            await _productService.DeactivateProductAsync(id);
            return NoContent();
        }

        [HttpPut("activate/{id}")]
        public async Task<ActionResult> ActivateProductAsync(int id)
        {
            await _productService.ActivateProductAsync(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProductAsync(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
