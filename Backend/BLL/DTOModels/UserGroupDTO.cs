using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels
{
    public record UserGroupRequestDTO
    {
        public string Name { get; init; }
    }
    public record UserGroupResponseDTO
    {
        public int Id { get; init; }
        public string Name { get; init; }
    }
}
