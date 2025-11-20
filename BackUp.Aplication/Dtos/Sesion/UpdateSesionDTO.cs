using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Dtos.Sesion
{
    public record UpdateSesionDTO
    {
        public int Id { get; init; }
        public DateTime? ExpiresAt { get; init; }
        public string? IpAddress { get; init; }
        public string? UserAgent { get; init; }
    }
}
