using BackUp.API.Models;
using BackUp.Application.Dtos.Dashboard;
using BackUp.Application.Interfaces.IService;
using BackUp.Domain.Base;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupController : ControllerBase
    {
        private readonly IBackupService _backupService;
        private readonly ILogger<BackupController> _logger;

        public BackupController(IBackupService backupService, ILogger<BackupController> logger)
        {
            _backupService = backupService;
            _logger = logger;
        }

        [HttpPost("{id}/restaurar")]
        public async Task<ActionResult> RestaurarBackup(int id, [FromBody] string destino)
        {
            try
            {
                var result = await _backupService.RestaurarBackupAsync(id, destino);

                if (result.IsSuccess)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar backup {Id}", id);
                return StatusCode(500, OperationResult.Failure("Error interno del servidor"));
            }
        }

        [HttpPost("{id}/verificar-integridad")]
        public async Task<ActionResult> VerificarIntegridad(int id)
        {
            try
            {
                var result = await _backupService.VerificarIntegridadAsync(id);

                if (result.IsSuccess)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar integridad del backup {Id}", id);
                return StatusCode(500, OperationResult.Failure("Error interno del servidor"));
            }
        }
    }
}
