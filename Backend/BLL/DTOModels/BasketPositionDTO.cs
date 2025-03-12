using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels
{
    public record BasketPositionRequestDTO
    {
        public int ProductId { get; init; }
        public int Amount { get; init; }
    }
    public record BasketPositionResponseDTO
    {
        public int ProductId { get; init; }
        public string ProductName { get; init; }
        public double ProductPrice { get; init; }
        public int Amount { get; init; }
        public double TotalPrice => ProductPrice * Amount;

    }
}
