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
    public class ProductService : IProductService
    {
        private readonly string _connectionString;

        public ProductService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetProductsAsync(
            string? name = null,
            string? groupName = null,
            int? groupId = null,
            bool onlyActive = true,
            SortOrder sortOrder = SortOrder.NameAscending)
        {
            using var conn = new SqlConnection(_connectionString);
            string sql = @"
        WITH GroupHierarchy AS (
            SELECT 
                ID,
                Name,
                ParentID,
                CAST(Name AS NVARCHAR(MAX)) AS FullPath
            FROM ProductGroups
            WHERE ParentID IS NULL

            UNION ALL

            SELECT 
                pg.ID,
                pg.Name,
                pg.ParentID,
                CAST(gh.FullPath + ' / ' + pg.Name AS NVARCHAR(MAX)) AS FullPath
            FROM ProductGroups pg
            INNER JOIN GroupHierarchy gh ON pg.ParentID = gh.ID
        )

        SELECT 
            p.ID AS Id,
            p.Name,
            p.Price,
            ISNULL(gh.FullPath, '') AS GroupName,
            p.IsActive
        FROM Products p
        LEFT JOIN GroupHierarchy gh ON p.GroupID = gh.ID
        WHERE
            (@IncludeInactive = 1 OR p.IsActive = 1)
            AND (@NameFilter IS NULL OR p.Name LIKE '%' + @NameFilter + '%')
            AND (@GroupNameFilter IS NULL OR gh.FullPath LIKE '%' + @GroupNameFilter + '%')
            AND (
                @GroupIdFilter IS NULL
                OR EXISTS (
                    SELECT 1
                    FROM GroupHierarchy gh2
                    WHERE gh2.ID = p.GroupID
                      AND (
                        gh2.ID = @GroupIdFilter
                        OR gh2.FullPath LIKE (
                            SELECT FullPath + '%' FROM GroupHierarchy WHERE ID = @GroupIdFilter
                        )
                      )
                )
            )
        ORDER BY
            CASE WHEN @SortBy = 'name' AND @SortOrder = 1 THEN p.Name END ASC,
            CASE WHEN @SortBy = 'name' AND @SortOrder = 0 THEN p.Name END DESC,
            CASE WHEN @SortBy = 'price' AND @SortOrder = 1 THEN p.Price END ASC,
            CASE WHEN @SortBy = 'price' AND @SortOrder = 0 THEN p.Price END DESC,
            CASE WHEN @SortBy = 'group' AND @SortOrder = 1 THEN gh.FullPath END ASC,
            CASE WHEN @SortBy = 'group' AND @SortOrder = 0 THEN gh.FullPath END DESC;";

            var parameters = new
            {
                NameFilter = name,
                GroupNameFilter = groupName,
                GroupIdFilter = groupId,
                SortBy = sortOrder switch
                {
                    SortOrder.NameAscending => "name",
                    SortOrder.NameDescending => "name",
                    SortOrder.PriceAscending => "price",
                    SortOrder.PriceDescending => "price",
                    _ => "name"
                },
                SortOrder = sortOrder switch
                {
                    SortOrder.NameAscending => 1,
                    SortOrder.NameDescending => 0,
                    SortOrder.PriceAscending => 1,
                    SortOrder.PriceDescending => 0,
                    _ => 1
                },
                IncludeInactive = onlyActive ? 0 : 1
            };

            var products = await conn.QueryAsync<ProductResponseDTO>(sql, parameters);

            return products;
        }

        public async Task<ProductResponseDTO> AddProductAsync(ProductRequestDTO productRequest)
        {
            using var conn = new SqlConnection(_connectionString);
            var newProduct = await conn.QuerySingleAsync<ProductResponseDTO>("AddProduct", new
            {
                Name = productRequest.Name,
                Price = productRequest.Price,
                GroupId = productRequest.GroupId
            }, commandType: CommandType.StoredProcedure);

            return newProduct;
        }

        public async Task DeactivateProductAsync(int productId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("DeactivateProduct", new { ProductId = productId }, commandType: CommandType.StoredProcedure);
        }

        public async Task ActivateProductAsync(int productId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("ActivateProduct", new { ProductId = productId }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteProductAsync(int productId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("DeleteProduct", new { ProductId = productId }, commandType: CommandType.StoredProcedure);
        }
    }
}
