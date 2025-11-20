using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Dtos.Dashboard
{
    public class DashboardDTO
    {
        public MetricasDTO Metricas { get; set; }
        public List<ProveedorStorageDTO> Proveedores { get; set; }
        public List<BackupRecienteDTO> BackupsRecientes { get; set; }
    }
}
