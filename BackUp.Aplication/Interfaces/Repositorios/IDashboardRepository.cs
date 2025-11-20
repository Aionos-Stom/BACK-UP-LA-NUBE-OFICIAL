using BackUp.Application.Dtos.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Interfaces.Repositorios
{
    public interface IDashboardRepository
    {
        Task<MetricasDTO> ObtenerMetricasAsync();
        Task<List<ProveedorStorageDTO>> ObtenerProveedoresAsync();
        Task<List<BackupRecienteDTO>> ObtenerBackupsRecientesAsync();
        Task<bool> CrearBackupAsync(CrearBackupDTO backupDto);
    }
}
