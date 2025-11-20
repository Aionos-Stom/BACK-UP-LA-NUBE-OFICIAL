using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Dtos.PoliticaBackup
{
    public record SavePoliticaBackupDTO
    {
        public int OrganizacionId { get; init; }
        public string Nombre { get; init; }
        public string Frecuencia { get; init; }
        public string TipoBackup { get; init; }
        public int RetencionDias { get; init; }
        public int RpoMinutes { get; init; }
        public int RtoMinutes { get; init; }
        public string? VentanaEjecucion { get; init; }
    }
}
