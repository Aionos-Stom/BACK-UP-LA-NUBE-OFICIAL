using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Dtos.Dashboard
{
    public class CrearBackupDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string RutaOrigen { get; set; }
        public int PoliticaId { get; set; }
        public int CloudStorageId { get; set; }
        public DateTime FechaProgramada { get; set; }
    }
}
