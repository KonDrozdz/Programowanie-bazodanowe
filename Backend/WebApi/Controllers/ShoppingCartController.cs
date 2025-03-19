using BLL.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddProductToBasketAsync(int productId, int userId, int amount)
        {
            await _shoppingCartService.AddProductToBasketAsync(productId, userId, amount);
            return NoContent();
        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateProductQuantityInBasketAsync(int productId, int userId, int newAmount)
        {
            await _shoppingCartService.UpdateProductQuantityInBasketAsync(productId, userId, newAmount);
            return NoContent();
        }

        [HttpDelete("remove")]
        public async Task<ActionResult> RemoveProductFromBasketAsync(int productId, int userId)
        {
            await _shoppingCartService.RemoveProductFromBasketAsync(productId, userId);
            return NoContent();
        }
    }
}
