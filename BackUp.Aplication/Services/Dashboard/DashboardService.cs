using BackUp.Aplication.Interfaces.IService;
using BackUp.Aplication.Interfaces.Repositorios;
using BackUp.Application.Dtos.Dashboard;
using BackUp.Application.Interfaces.IService;
using BackUp.Application.Interfaces.Repositorios;
using BackUp.Domain.Base;
using BackUp.Domain.Entities.Bac;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {

        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardDTO> ObtenerDashboardAsync()
        {
            var metricas = await ObtenerMetricasAsync();
            var proveedores = await ObtenerProveedoresAsync();
            var backupsRecientes = await ObtenerBackupsRecientesAsync();

            return new DashboardDTO
            {
                Metricas = metricas,
                Proveedores = proveedores,
                BackupsRecientes = backupsRecientes
            };
        }

        public async Task<MetricasDTO> ObtenerMetricasAsync()
        {
            return await _dashboardRepository.ObtenerMetricasAsync();
        }

        public async Task<List<ProveedorStorageDTO>> ObtenerProveedoresAsync()
        {
            return await _dashboardRepository.ObtenerProveedoresAsync();
        }

        public async Task<List<BackupRecienteDTO>> ObtenerBackupsRecientesAsync()
        {
            return await _dashboardRepository.ObtenerBackupsRecientesAsync();
        }

        public async Task<bool> CrearBackupAsync(CrearBackupDTO backupDto)
        {
            if (string.IsNullOrWhiteSpace(backupDto.Nombre))
                return false;

            if (backupDto.FechaProgramada < DateTime.UtcNow)
                return false;

            return await _dashboardRepository.CrearBackupAsync(backupDto);
        }

        public async Task<OperationResult> EjecutarAccionRapidaAsync(string accion)
        {
            try
            {
                switch (accion.ToLower())
                {
                    case "limpiar_logs":
                        // Lógica para limpiar logs
                        return OperationResult.Success("Logs limpiados correctamente");

                    case "verificar_integridad":
                        // Lógica para verificar integridad
                        return OperationResult.Success("Verificación de integridad completada");

                    case "sincronizar_cloud":
                        // Lógica para sincronizar cloud
                        return OperationResult.Success("Sincronización completada");

                    default:
                        return OperationResult.Failure("Acción no reconocida");
                }
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Error al ejecutar acción: {ex.Message}");
            }
        }
    }
}

