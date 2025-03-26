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
    public class ProductGroupService : IProductGroupService
    {
        private readonly string _connectionString;

        public ProductGroupService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<ProductGroupResponseDTO>> GetProductGroupsAsync(int? parentId = null, SortOrder sortOrder = SortOrder.NameAscending)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = new StringBuilder("SELECT ID, Name, ParentID FROM ProductGroups WHERE 1=1");

            if (parentId.HasValue)
            {
                query.Append(" AND ParentID = @ParentId");
            }

            query.Append(sortOrder switch
            {
                SortOrder.NameAscending => " ORDER BY Name ASC",
                SortOrder.NameDescending => " ORDER BY Name DESC",
                _ => " ORDER BY Name ASC",
            });

            var result = await conn.QueryAsync<ProductGroupResponseDTO>(query.ToString(), new
            {
                ParentId = parentId
            });

            return result;
        }

        public async Task<ProductGroupResponseDTO> AddProductGroupAsync(ProductGroupRequestDTO groupRequest)
        {
            using var conn = new SqlConnection(_connectionString);
            var newGroup = await conn.QuerySingleAsync<ProductGroupResponseDTO>("AddProductGroup", new
            {
                Name = groupRequest.Name,
                ParentId = groupRequest.ParentId
            }, commandType: CommandType.StoredProcedure);

            return newGroup;
        }
    }
}
