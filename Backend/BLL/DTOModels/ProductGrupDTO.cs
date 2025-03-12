using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels
{
    public record ProductGroupRequestDTO
    {
        public string Name { get; init; }
        public int? ParentId { get; init; }
    }
    public record ProductGroupResponseDTO
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public int? ParentId { get; init; }
    }

}
