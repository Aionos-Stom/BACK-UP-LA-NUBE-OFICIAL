using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Dtos.Alerta
{
    public record ObtenerAlertaDTO
    {
        public int Id { get; init; }
        public int UsuarioId { get; init; }
        public string UsuarioNombre { get; init; } = string.Empty;
        public int? JobId { get; init; }
        public string? JobNombre { get; init; }
        public string Tipo { get; init; } = string.Empty;
        public string Severidad { get; init; } = string.Empty;
        public string Mensaje { get; init; } = string.Empty;
        public bool IsAcknowledged { get; init; }
        public DateTime CreatedAt { get; init; }


    }
}
