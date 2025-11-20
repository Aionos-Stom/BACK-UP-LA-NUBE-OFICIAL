using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Dtos.Alerta
{
    public record UpdateAlertaDTO
    {
        public int Id { get; init; }
        public int? JobId { get; init; }
        public string? Tipo { get; init; }
        public string? Severidad { get; init; } // 'baja', 'media', 'alta', 'crítica'
        public string? Mensaje { get; init; }
        public bool? IsAcknowledged { get; init; }
    }
}
