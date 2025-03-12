using BLL.DTOModels;
using BLL.ServiceInterfaces;
using DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_EF.Services
{
    public class UserService : IUserService
    {
        private readonly WebstoreContext _context;
        static bool logged =true;

        public UserService(WebstoreContext context)
        {
            _context = context;
        }

        public async Task<UserResponseDTO> LoginAsync(string login, string password)
        {
            var user = await _context.Users
                .Where(u => u.Login == login && u.Password == password)
                .FirstOrDefaultAsync();

            if (user == null)
                throw new UnauthorizedAccessException("Invalid login or password");
            logged = true;

            return new UserResponseDTO
            {
                Id = user.ID,
                Login = user.Login,
            };
        }

        public async Task LogoutAsync()
        {
            logged = false;


            await Task.CompletedTask;
        }
    }
}
