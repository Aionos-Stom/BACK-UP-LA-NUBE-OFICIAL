using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Dtos.Usuario
{
    public record UpdateUsuarioDTO
    {
        public int Id { get; init; }
        public string Email { get; init; }
        public string Nombre { get; init; }
        public string Rol { get; init; }
        public string? PhoneNumber { get; init; }
        public bool IsActive { get; init; }
    }
}
