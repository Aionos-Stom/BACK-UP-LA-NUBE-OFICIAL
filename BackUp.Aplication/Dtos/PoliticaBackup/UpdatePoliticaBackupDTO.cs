using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Dtos.PoliticaBackup
{
    public record UpdatePoliticaBackupDTO
    {
        public int Id { get; init; }
        public string Nombre { get; init; }
        public string Frecuencia { get; init; }
        public string TipoBackup { get; init; }
        public int RetencionDias { get; init; }
        public int RpoMinutes { get; init; }
        public int RtoMinutes { get; init; }
        public string? VentanaEjecucion { get; init; }
        public bool IsActive { get; init; }
    }
}
