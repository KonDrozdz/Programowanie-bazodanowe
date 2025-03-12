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
    public class OrderService : IOrderService
    {
        private readonly WebstoreContext _context;

        public OrderService(WebstoreContext context)
        {
            _context = context;
        }

        public async Task<OrderResponseDTO> GenerateOrderAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == userId);
            if (user == null)
                throw new ArgumentException("User not found");

            var basketPositions = await _context.BasketPositions
                .Where(bp => bp.UserID == userId)
                .Include(bp => bp.Product) 
                .ToListAsync();

            if (!basketPositions.Any())
                throw new ArgumentException("Basket is empty");

            var order = new Order
            {
                UserID = userId,
                Date = DateTime.UtcNow
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var orderPositions = basketPositions.Select(bp => new OrderPosition
            {
                Order = order,
                ProductID = bp.ProductID,
                Amount = bp.Amount,
                Price = bp.Product.Price
            }).ToList();

            await _context.OrderPositions.AddRangeAsync(orderPositions);
            _context.BasketPositions.RemoveRange(basketPositions);
            await _context.SaveChangesAsync();

            return new OrderResponseDTO
            {
                Id = order.ID,
                Date = order.Date,
                TotalAmount = orderPositions.Sum(op => op.Price * op.Amount)
            };
        }

        public async Task PayForOrderAsync(int orderId, double amountPaid)
        {
            var order = await _context.Orders
                .Include(o => o.OrderPositions)
                .FirstOrDefaultAsync(o => o.ID == orderId);

            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.OrderPositions.Sum(op => op.Price * op.Amount) > amountPaid)
                throw new ArgumentException("Insufficient payment");
            order.IsPaid = true;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersAsync(int? orderId = null, bool? isPaid = null, SortOrder sortOrder = SortOrder.DateAscending)
        {
            IQueryable<Order> query = _context.Orders.Include(o => o.OrderPositions);

            if (orderId.HasValue)
                query = query.Where(o => o.ID == orderId.Value);

            if (isPaid.HasValue)
                query = query.Where(o => o.IsPaid == isPaid.Value);

            query = sortOrder switch
            {
                SortOrder.DateAscending => query.OrderBy(o => o.Date),
                SortOrder.DateDescending => query.OrderByDescending(o => o.Date),
                SortOrder.PriceAscending => query.OrderBy(o => o.OrderPositions.Sum(op => op.Price * op.Amount)),
                SortOrder.PriceDescending => query.OrderByDescending(o => o.OrderPositions.Sum(op => op.Price * op.Amount)),
                SortOrder.IsPaidAscending => query.OrderBy(o => o.IsPaid), 
                SortOrder.IsPaidDescending => query.OrderByDescending(o => o.IsPaid),
                _ => query
            };

            var orders = await query.ToListAsync();

            return orders.Select(o => new OrderResponseDTO
            {
                Id = o.ID,
                Date = o.Date,
                TotalAmount = o.OrderPositions.Sum(op => op.Price * op.Amount),
                IsPaid = o.IsPaid 
            }).ToList();
        }

        public async Task<IEnumerable<OrderPositionResponseDTO>> GetOrderPositionsAsync(int orderId)
        {
            var orderPositions = await _context.OrderPositions
                .Where(op => op.OrderID == orderId)
                .Include(op => op.Product) 
                .ToListAsync();

            if (!orderPositions.Any())
                throw new ArgumentException("Order not found or has no positions");

            return orderPositions.Select(op => new OrderPositionResponseDTO
            {
                ProductName = op.Product.Name,
                ProductPrice = op.Price,
                Amount = op.Amount,
            }).ToList();
        }

    }
}
