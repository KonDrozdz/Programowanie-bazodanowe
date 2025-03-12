using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels
{
    public record ProductRequestDTO
    {
        public string Name { get; init; }
        public double Price { get; init; }
        public int? GroupId { get; init; }
    }

    public record ProductResponseDTO
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public double Price { get; init; }
        public string GroupName { get; init; }
        public bool IsActive { get; init; }
    }
}
