using BackUp.Aplication.Dtos.Recuperacion;
using BackUp.Aplication.Interfaces.IService;
using BackUp.Domain.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecuperacionController : ControllerBase
    {
        private readonly IRecuperacionService _recuperacionService;

        public RecuperacionController(IRecuperacionService recuperacionService)
        {
            _recuperacionService = recuperacionService;
        }

        [HttpGet]
        public async Task<ActionResult<OperationResult>> ObtenerTodos()
        {
            var result = await _recuperacionService.ObtenerTodosAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationResult>> ObtenerPorId(int id)
        {
            var result = await _recuperacionService.ObtenerPorIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<OperationResult>> ObtenerPorUsuario(int usuarioId)
        {
            var result = await _recuperacionService.ObtenerTodosAsync();
            // Filtrar por usuarioId si tu servicio base no tiene este método específico
            // O implementar un método específico en el servicio
            return Ok(result);
        }

        [HttpGet("job/{jobId}")]
        public async Task<ActionResult<OperationResult>> ObtenerPorJob(int jobId)
        {
            var result = await _recuperacionService.ObtenerTodosAsync();
            // Filtrar por jobId si tu servicio base no tiene este método específico
            return Ok(result);
        }

        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<OperationResult>> ObtenerPorEstado(string estado)
        {
            var result = await _recuperacionService.ObtenerTodosAsync();
            // Filtrar por estado si tu servicio base no tiene este método específico
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult>> Crear([FromBody] SaveRecuperacionDTO saveRecuperacion)
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationResult.Failure("Datos inválidos"));

            var result = await _recuperacionService.AgregarAsync(saveRecuperacion);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OperationResult>> Actualizar(int id, [FromBody] UpdateRecuperacionDTO updateRecuperacion)
        {
            if (id != updateRecuperacion.Id)
                return BadRequest(OperationResult.Failure("ID no coincide"));

            if (!ModelState.IsValid)
                return BadRequest(OperationResult.Failure("Datos inválidos"));

            var result = await _recuperacionService.ActualizarAsync(updateRecuperacion);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<OperationResult>> Eliminar(int id)
        {
            var removeDto = new RemoveRecuperacionDTO { Id = id };
            var result = await _recuperacionService.EliminarAsync(removeDto);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("{id}/ejecutar")]
        public async Task<ActionResult<OperationResult>> EjecutarRecuperacion(int id)
        {
            var result = await _recuperacionService.EjecutarRecuperacionAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("{id}/simular")]
        public async Task<ActionResult<OperationResult>> SimularRecuperacion(int id)
        {
            var result = await _recuperacionService.SimularRecuperacionAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
