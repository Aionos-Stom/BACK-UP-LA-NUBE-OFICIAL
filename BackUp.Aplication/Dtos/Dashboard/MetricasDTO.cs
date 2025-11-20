using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Dtos.Dashboard
{
    public class MetricasDTO
    {
        public double TotalAlmacenadoTB { get; set; }
        public int BackupsHoy { get; set; }
        public double TasaExitoPorcentaje { get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public double IncrementoAlmacenamiento { get; set; }
        public double IncrementoBackups { get; set; }
        public double IncrementoTasaExito { get; set; }
    }
}
