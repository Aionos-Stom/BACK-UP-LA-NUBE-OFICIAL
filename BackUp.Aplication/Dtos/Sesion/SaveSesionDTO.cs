using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Dtos.Sesion
{
    public record SaveSesionDTO
    {
        public int UsuarioId { get; init; }
        public string Token { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public string? IpAddress { get; init; }
        public string? UserAgent { get; init; }
    }
}
