using BackUp.Aplication.Dtos.politicaBackup;
using BackUp.Aplication.Dtos.PoliticaBackup;
using BackUp.Aplication.Interfaces.IService;
using BackUp.Domain.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoliticaBackupController : ControllerBase
    {
        private readonly IPoliticaBackupService _politicaBackupService;
        private readonly ILogger<PoliticaBackupController> _logger;

        public PoliticaBackupController(
            IPoliticaBackupService politicaBackupService,
            ILogger<PoliticaBackupController> logger)
        {
            _politicaBackupService = politicaBackupService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var resultado = await _politicaBackupService.ObtenerPorIdAsync(id);

                if (!resultado.IsSuccess)
                    return NotFound(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener política de backup por ID: {Id}", id);
                return StatusCode(500, OperationResult.Failure($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var resultado = await _politicaBackupService.ObtenerTodosAsync();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las políticas de backup");
                return StatusCode(500, OperationResult.Failure($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("organizacion/{organizacionId}")]
        public async Task<IActionResult> ObtenerPorOrganizacion(int organizacionId)
        {
            try
            {
                var resultado = await _politicaBackupService.ObtenerPorOrganizacionAsync(organizacionId);

                if (!resultado.IsSuccess)
                    return NotFound(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener políticas por organización: {OrganizacionId}", organizacionId);
                return StatusCode(500, OperationResult.Failure($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("activas")]
        public async Task<IActionResult> ObtenerActivas()
        {
            try
            {
                var resultado = await _politicaBackupService.ObtenerActivasAsync();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener políticas activas");
                return StatusCode(500, OperationResult.Failure($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("tipo/{tipoBackup}")]
        public async Task<IActionResult> ObtenerPorTipo(string tipoBackup)
        {
            try
            {
                var resultado = await _politicaBackupService.ObtenerPorTipoAsync(tipoBackup);

                if (!resultado.IsSuccess)
                    return NotFound(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener políticas por tipo: {TipoBackup}", tipoBackup);
                return StatusCode(500, OperationResult.Failure($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] SavePoliticaBackupDTO savePoliticaBackup)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(OperationResult.Failure("Datos de entrada inválidos"));

                var resultado = await _politicaBackupService.CrearAsync(savePoliticaBackup);

                if (!resultado.IsSuccess)
                    return BadRequest(resultado);

                return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Data }, resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear política de backup");
                return StatusCode(500, OperationResult.Failure($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] UpdatePoliticaBackupDTO updatePoliticaBackup)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(OperationResult.Failure("Datos de entrada inválidos"));

                var resultado = await _politicaBackupService.ActualizarAsync(updatePoliticaBackup);

                if (!resultado.IsSuccess)
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar política de backup: {Id}", updatePoliticaBackup?.Id);
                return StatusCode(500, OperationResult.Failure($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var removeDto = new RemovePoliticaBackupDTO { Id = id };
                var resultado = await _politicaBackupService.EliminarAsync(removeDto);

                if (!resultado.IsSuccess)
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar política de backup: {Id}", id);
                return StatusCode(500, OperationResult.Failure($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] bool activo)
        {
            try
            {
                var resultado = await _politicaBackupService.CambiarEstadoAsync(id, activo);

                if (!resultado.IsSuccess)
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de política de backup: {Id}", id);
                return StatusCode(500, OperationResult.Failure($"Error interno del servidor: {ex.Message}"));
            }
        }
    }
}