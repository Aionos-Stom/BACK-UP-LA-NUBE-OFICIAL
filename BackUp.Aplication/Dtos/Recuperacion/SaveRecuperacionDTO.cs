using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Dtos.Recuperacion
{
    public record SaveRecuperacionDTO
    {
        public int UsuarioId { get; init; }
        public int JobId { get; init; }
        public string? TipoRecuperacion { get; init; }
        public DateTime? PuntoTiempo { get; init; }
        public string? InputPath { get; init; }
        public bool IsSimulacion { get; init; }
    }
}
