using BackUp.Aplication.Dtos.VerificacionIntegridad;
using BackUp.Aplication.Interfaces.IService;
using BackUp.Domain.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerificacionIntegridadController : ControllerBase
    {
        private readonly IVerificacionIntegridadService _verificacionIntegridadService;

        public VerificacionIntegridadController(IVerificacionIntegridadService verificacionIntegridadService)
        {
            _verificacionIntegridadService = verificacionIntegridadService;
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
            var repository = (_verificacionIntegridadService as dynamic)._verificacionIntegridadRepository;
            var result = await repository.ObtenerPorJobAsync(jobId);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("job/{jobId}/ultima")]
        public async Task<ActionResult<ObtenerVerificacionIntegridadDTO>> ObtenerUltimaPorJob(int jobId)
        {
            var repository = (_verificacionIntegridadService as dynamic)._verificacionIntegridadRepository;
            var verificacion = await repository.ObtenerUltimaPorJobAsync(jobId);

            if (verificacion == null)
                return NotFound();

            return Ok(verificacion);
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
