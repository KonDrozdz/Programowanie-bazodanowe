using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels
{
    public record OrderRequestDTO
    {
        public int UserId { get; init; }
        public List<BasketPositionRequestDTO> BasketPositions { get; init; }
    }

    public record OrderResponseDTO
    {
        public int Id { get; init; }
        public int UserId { get; init; }
        public DateTime Date { get; init; }
        public double TotalAmount { get; init; }
        public bool IsPaid { get; init; }
    }

    public record OrderPositionResponseDTO
    {
        public int ProductId { get; init; }
        public string ProductName { get; init; }
        public double ProductPrice { get; init; }
        public int Amount { get; init; }
        public double TotalPrice => ProductPrice * Amount;
    }
}
