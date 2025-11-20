using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Dtos.Alerta
{
    public record SaveAlertaDTO
    {
        public int UsuarioId { get; init; }
        public int? JobId { get; init; }
        public string Tipo { get; init; } = string.Empty;
        public string Severidad { get; init; } = string.Empty; // 'baja', 'media', 'alta', 'crítica'
        public string Mensaje { get; init; } = string.Empty;
        public bool IsAcknowledged { get; init; } = false;
    }
}
