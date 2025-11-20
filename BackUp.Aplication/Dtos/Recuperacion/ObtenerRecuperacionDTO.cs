using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Dtos.Recuperacion
{
    public record ObtenerRecuperacionDTO
    {
        public int Id { get; init; }
        public int UsuarioId { get; init; }
        public int JobId { get; init; }
        public string? TipoRecuperacion { get; init; }
        public DateTime? PuntoTiempo { get; init; }
        public string? InputPath { get; init; }
        public string? Estado { get; init; }
        public bool IsSimulacion { get; init; }
        public DateTime FechaCreacion { get; init; }
        public DateTime? CompletedAt { get; init; }
    }
}
