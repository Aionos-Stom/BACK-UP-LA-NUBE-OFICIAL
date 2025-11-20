using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Dtos.Usuario
{
    public record ObtenerUsuarioDTO
    {
        public int Id { get; init; }
        public int OrganizacionId { get; init; }
        public string Email { get; init; }
        public string Nombre { get; init; }
        public string Rol { get; init; }
        public bool MfaHabilitado { get; init; }
        public string? PhoneNumber { get; init; }
        public DateTime? LastLogin { get; init; }
        public bool IsActive { get; init; }
        public DateTime FechaCreacion { get; init; }
    }
}
