using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Dtos.VerificacionIntegridad
{
    public record SaveVerificacionIntegridadDTO
    {
        public int JobId { get; init; }
        public string? ChecksumSha256 { get; init; }
        public bool? Resultado { get; init; }
        public string? Detalles { get; init; }
    }
}
