using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Dtos.JobBackup
{
    public record UpdateJobBackupDTO
    {
        public int Id { get; init; }
        public string Estado { get; init; }
        public DateTime? FechaEjecucion { get; init; }
        public DateTime? FechaCompletado { get; init; }
        public long? TamanoBytes { get; init; }
        public int? DuracionSegundos { get; init; }
        public string? ErrorMessage { get; init; }
    }
}
