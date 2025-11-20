using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Aplication.Dtos.JobBackup
{ 

    public record SaveJobBackupDTO
    {
        public int PoliticaId { get; init; }
        public int CloudStorageId { get; init; }
        public DateTime? FechaProgramada { get; init; }
        public string? SourceData { get; init; }
    }
}
