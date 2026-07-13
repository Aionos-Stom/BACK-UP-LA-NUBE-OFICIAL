using BackUp.Aplication.Dtos.VerificacionIntegridad;
using BackUp.Aplication.Interfaces.IService;
using BackUp.Domain.Base;
using BackUp.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VerificacionIntegridadController : ControllerBase
    {
        private readonly IVerificacionIntegridadService _verificacionIntegridadService;
        private readonly BackUpDbContext _context;

        public VerificacionIntegridadController(IVerificacionIntegridadService verificacionIntegridadService, BackUpDbContext context)
        {
            _verificacionIntegridadService = verificacionIntegridadService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<OperationResult>> ObtenerTodos()
        {
            var result = await _verificacionIntegridadService.ObtenerTodosAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationResult>> ObtenerPorId(int id)
        {
            var result = await _verificacionIntegridadService.ObtenerPorIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("job/{jobId}")]
        public async Task<ActionResult<OperationResult>> ObtenerPorJob(int jobId)
        {
            var verificaciones = await _context.VerificacionIntegridad
                .Where(v => v.job_id == jobId)
                .OrderByDescending(v => v.FechaVerificacion)
                .ToListAsync();
            return Ok(OperationResult.Success(verificaciones));
        }

        [HttpGet("job/{jobId}/ultima")]
        public async Task<ActionResult<OperationResult>> ObtenerUltimaPorJob(int jobId)
        {
            var verificacion = await _context.VerificacionIntegridad
                .Where(v => v.job_id == jobId)
                .OrderByDescending(v => v.FechaVerificacion)
                .FirstOrDefaultAsync();
            if (verificacion == null) return NotFound(OperationResult.Failure("No encontrada"));
            return Ok(OperationResult.Success(verificacion));
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult>> Crear([FromBody] SaveVerificacionIntegridadDTO saveVerificacion)
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationResult.Failure("Datos inválidos"));

            var result = await _verificacionIntegridadService.AgregarAsync(saveVerificacion);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data }, result);
        }

        [HttpPost("job/{jobId}/verificar")]
        public async Task<ActionResult<OperationResult>> VerificarIntegridad(int jobId)
        {
            var result = await _verificacionIntegridadService.VerificarIntegridadAsync(jobId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<OperationResult>> Eliminar(int id)
        {
            var removeDto = new RemoveVerificacionIntegridadDTO { Id = id };
            var result = await _verificacionIntegridadService.EliminarAsync(removeDto);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
    }
}
