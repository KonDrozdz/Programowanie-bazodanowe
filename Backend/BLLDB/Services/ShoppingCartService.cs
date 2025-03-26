using BLL.ServiceInterfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLLDB.Services
{
    public class ShoppingCartService: IShoppingCartService
    {
        private readonly string _connectionString;

        public ShoppingCartService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddProductToBasketAsync(int productId, int userId, int amount)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("AddProductToBasket", new
            {
                UserId = userId,
                ProductId = productId,
                Amount = amount
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateProductQuantityInBasketAsync(int productId, int userId, int newAmount)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("UpdateBasketItem", new
            {
                UserId = userId,
                ProductId = productId,
                Amount = newAmount
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task RemoveProductFromBasketAsync(int productId, int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("RemoveFromBasket", new
            {
                UserId = userId,
                ProductId = productId
            }, commandType: CommandType.StoredProcedure);
        }
    }
}

