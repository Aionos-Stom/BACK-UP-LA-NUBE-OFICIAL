using BackUp.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackUp.Application.Dtos.Dashboard;

namespace BackUp.Application.Interfaces.IService
{
    public interface IDashboardService
    {
        Task<DashboardDTO> ObtenerDashboardAsync();
        Task<MetricasDTO> ObtenerMetricasAsync();
        Task<List<ProveedorStorageDTO>> ObtenerProveedoresAsync();
        Task<List<BackupRecienteDTO>> ObtenerBackupsRecientesAsync();
        Task<bool> CrearBackupAsync(CrearBackupDTO backupDto);
        Task<OperationResult> EjecutarAccionRapidaAsync(string accion);
    }
}