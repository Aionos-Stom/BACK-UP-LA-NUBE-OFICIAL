using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Dtos.organizacion
{
    public record SaveOrganizacionDTO
    {
        public string Nombre { get; init; } = string.Empty;
        public string? Configuracion { get; init; }
        public DateTime? LicenciaValidaHasta { get; init; }
        public int MaxUsuarios { get; init; }
        public bool Activo { get; init; } = true;
    }
}
