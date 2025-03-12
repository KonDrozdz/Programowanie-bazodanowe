using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels
{
    public record UserRequestDTO
    {
        public string Login { get; init; }
        public string Password { get; init; }
    }

    public record UserResponseDTO
    {
        public int Id { get; init; }
        public string Login { get; init; }
        public string UserType { get; init; }
        public bool IsActive { get; init; }
    }
}
