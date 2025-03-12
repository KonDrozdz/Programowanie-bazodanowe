using BLL.DTOModels;
using BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ServiceInterfaces
{
    public interface IOrderService
    {

        Task<OrderResponseDTO> GenerateOrderAsync(int userId);

        Task PayForOrderAsync(int orderId, double amountPaid);
        Task<IEnumerable<OrderResponseDTO>> GetOrdersAsync(
            int? orderId = null,
            bool? isPaid = null,
            SortOrder sortOrder = SortOrder.DateAscending
        );
        Task<IEnumerable<OrderPositionResponseDTO>> GetOrderPositionsAsync(int orderId);
    }
}
