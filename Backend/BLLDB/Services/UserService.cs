using BLL.DTOModels;
using BLL.ServiceInterfaces;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using WebApi.Model;
using Dapper;
namespace BLLDB.Services
{
    public class UserService: IUserService
    {
        private readonly string _connectionString;
        static bool logged = true;

        public UserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<UserResponseDTO> LoginAsync(string login, string password)
        {
            using var conn = new SqlConnection(_connectionString);
            var user = await conn.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Login = @Login AND Password = @Password", new { Login = login, Password = password });

            if (user == null)
                throw new UnauthorizedAccessException("Invalid login or password");

            logged = true;

            return new UserResponseDTO
            {
                Id = user.ID,
                Login = user.Login,
                UserType = user.Type.ToString(),
                IsActive = user.IsActive
            };
        }

        public async Task LogoutAsync()
        {
            logged = false;
            await Task.CompletedTask;
        }
    }
}
