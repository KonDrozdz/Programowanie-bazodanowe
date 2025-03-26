using BLL.DTOModels;
using BLL.Enums;
using BLL.ServiceInterfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SortOrder = BLL.Enums.SortOrder;

namespace BLLDB.Services
{
    public class OrderService : IOrderService
    {
        private readonly string _connectionString;

        public OrderService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<OrderResponseDTO> GenerateOrderAsync(int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            var order = await conn.QuerySingleAsync<OrderResponseDTO>("GenerateOrder", new
            {
                UserId = userId
            }, commandType: CommandType.StoredProcedure);

            return order;
        }

        public async Task PayForOrderAsync(int orderId, double amountPaid)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("PayForOrder", new
            {
                OrderId = orderId,
                AmountPaid = amountPaid
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersAsync(int? orderId = null, bool? isPaid = null, SortOrder sortOrder = SortOrder.DateAscending)
        {
            using var conn = new SqlConnection(_connectionString);
            var orderByClause = sortOrder switch
            {
                SortOrder.DateAscending => "o.Date ASC",
                SortOrder.DateDescending => "o.Date DESC",
                SortOrder.PriceAscending => "TotalAmount ASC",
                SortOrder.PriceDescending => "TotalAmount DESC",
                SortOrder.IsPaidAscending => "o.IsPaid ASC",
                SortOrder.IsPaidDescending => "o.IsPaid DESC",
                _ => "o.Date ASC"
            };


            var query = $@"
            SELECT 
                o.ID,
                o.UserID,
                o.Date,
                SUM(op.Price * op.Amount) AS TotalAmount,
                o.IsPaid
            FROM 
                Orders o
            INNER JOIN 
                OrderPositions op ON o.ID = op.OrderID
            WHERE 
                (@OrderId IS NULL OR o.ID = @OrderId) AND
                (@IsPaid IS NULL OR o.IsPaid = @IsPaid)
            GROUP BY 
                o.ID, o.UserID, o.Date, o.IsPaid  -- Add o.UserID here
            ORDER BY {orderByClause};
            ";

            var orders = await conn.QueryAsync<OrderResponseDTO>(query, new
            {
                OrderId = orderId,
                IsPaid = isPaid
            });

            return orders;
        }

        public async Task<IEnumerable<OrderPositionResponseDTO>> GetOrderPositionsAsync(int orderId)
        {
            using var conn = new SqlConnection(_connectionString);

            var query = @"
            SELECT
                op.ProductID AS ProductId,
                p.Name AS ProductName, 
                op.Price AS ProductPrice, 
                op.Amount
            FROM 
                OrderPositions op
            INNER JOIN 
                Products p ON op.ProductID = p.Id
            WHERE 
                op.OrderID = @OrderId";

            var orderPositions = await conn.QueryAsync<OrderPositionResponseDTO>(query, new { OrderId = orderId });

            if (!orderPositions.Any())
                throw new ArgumentException("Order not found or has no positions");

            return orderPositions;
        }
    }
}
