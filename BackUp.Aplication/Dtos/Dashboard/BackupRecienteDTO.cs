using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Dtos.Dashboard
{
    public class BackupRecienteDTO
    {
        public string Nombre { get; set; }
        public double TamanioGB { get; set; }
        public string Proveedor { get; set; }
        public string Estado { get; set; }
        public DateTime? HoraEjecucion { get; set; }
    }
}
