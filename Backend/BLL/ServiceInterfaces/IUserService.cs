using BLL.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ServiceInterfaces
{
    public interface IUserService
    {

        Task<UserResponseDTO> LoginAsync(string login, string password);


        Task LogoutAsync();
    }
}
