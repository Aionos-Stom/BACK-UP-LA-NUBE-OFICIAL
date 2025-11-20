using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Dtos.Sesion
{
    public record ObtenerSesionDTO
    {
        public int Id { get; init; }
        public int UsuarioId { get; init; }
        public string UsuarioNombre { get; init; } = string.Empty;
        public string UsuarioEmail { get; init; } = string.Empty;
        public string Token { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public string? IpAddress { get; init; }
        public string? UserAgent { get; init; }
        public DateTime CreatedAt { get; init; }
        public bool IsExpired => ExpiresAt < DateTime.UtcNow;
    }
}
