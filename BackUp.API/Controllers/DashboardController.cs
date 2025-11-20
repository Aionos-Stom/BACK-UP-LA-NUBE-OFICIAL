using BackUp.API.Models;
using BackUp.Application.Dtos.Dashboard;
using BackUp.Application.Interfaces.IService;
using BackUp.Domain.Base;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardDTO>> ObtenerDashboard()
        {
            var dashboard = await _dashboardService.ObtenerDashboardAsync();
            return Ok(dashboard);
        }

        [HttpGet("metricas")]
        public async Task<ActionResult<MetricasDTO>> ObtenerMetricas()
        {
            var metricas = await _dashboardService.ObtenerMetricasAsync();
            return Ok(metricas);
        }

        [HttpGet("proveedores")]
        public async Task<ActionResult<List<ProveedorStorageDTO>>> ObtenerProveedores()
        {
            var proveedores = await _dashboardService.ObtenerProveedoresAsync();
            return Ok(proveedores);
        }

        [HttpGet("backups-recientes")]
        public async Task<ActionResult<List<BackupRecienteDTO>>> ObtenerBackupsRecientes()
        {
            var backups = await _dashboardService.ObtenerBackupsRecientesAsync();
            return Ok(backups);
        }

        [HttpPost("crear-backup")]
        public async Task<IActionResult> CrearBackup(CrearBackupDTO backupDto)
        {
            var result = await _dashboardService.CrearBackupAsync(backupDto);

            if (!result)
                return BadRequest("Error al crear el backup");

            return Ok("Backup creado correctamente");
        }

    }
}

