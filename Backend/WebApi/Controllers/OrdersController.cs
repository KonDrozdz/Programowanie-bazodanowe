using BLL.DTOModels;
using BLL.Enums;
using BLL.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetOrdersAsync(
            int? orderId = null,
            bool? isPaid = null,
            SortOrder sortOrder = SortOrder.DateAscending)
        {
            var orders = await _orderService.GetOrdersAsync(orderId, isPaid, sortOrder);
            return Ok(orders);
        }

        [HttpGet("{orderId}/positions")]
        public async Task<ActionResult<IEnumerable<OrderPositionResponseDTO>>> GetOrderPositionsAsync(int orderId)
        {
            var orderPositions = await _orderService.GetOrderPositionsAsync(orderId);
            return Ok(orderPositions);
        }

        [HttpPost("generate/{userId}")]
        public async Task<ActionResult<OrderResponseDTO>> GenerateOrderAsync(int userId)
        {
            var order = await _orderService.GenerateOrderAsync(userId);
            return Ok(order);
        }

        [HttpPut("pay/{orderId}")]
        public async Task<ActionResult> PayForOrderAsync(int orderId, double amountPaid)
        {
            await _orderService.PayForOrderAsync(orderId, amountPaid);
            return NoContent();
        }
    }
}
