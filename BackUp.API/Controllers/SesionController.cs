using BackUp.Application.Dtos.Sesion;
using BackUp.Application.Interfaces.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SesionController : ControllerBase
    {
        private readonly ISesionService _sesionService;

        public SesionController(ISesionService sesionService)
        {
            _sesionService = sesionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ObtenerSesionDTO>>> ObtenerTodas()
        {
            var sesiones = await _sesionService.ObtenerTodasAsync();
            return Ok(sesiones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ObtenerSesionDTO>> ObtenerPorId(int id)
        {
            var sesion = await _sesionService.ObtenerPorIdAsync(id);
            if (sesion == null)
                return NotFound();

            return Ok(sesion);
        }

        [HttpGet("token/{token}")]
        public async Task<ActionResult<ObtenerSesionDTO>> ObtenerPorToken(string token)
        {
            var sesion = await _sesionService.ObtenerPorTokenAsync(token);
            if (sesion == null)
                return NotFound();

            return Ok(sesion);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<ObtenerSesionDTO>>> ObtenerPorUsuario(int usuarioId)
        {
            var sesiones = await _sesionService.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(sesiones);
        }

        [HttpPost]
        public async Task<ActionResult<ObtenerSesionDTO>> Crear([FromBody] SaveSesionDTO saveSesion)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sesion = await _sesionService.CrearAsync(saveSesion);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = sesion.Id }, sesion);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ObtenerSesionDTO>> Actualizar(int id, [FromBody] UpdateSesionDTO updateSesion)
        {
            if (id != updateSesion.Id)
                return BadRequest("ID no coincide");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sesion = await _sesionService.ActualizarAsync(updateSesion);
            if (sesion == null)
                return NotFound();

            return Ok(sesion);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var removeSesion = new RemoveSesionDTO { Id = id };
            var resultado = await _sesionService.EliminarAsync(removeSesion);

            if (!resultado)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("expiradas")]
        public async Task<ActionResult> EliminarExpiradas()
        {
            var resultado = await _sesionService.EliminarExpiradasAsync();
            return Ok(new { eliminadas = resultado });
        }

        [HttpGet("validar/{token}")]
        public async Task<ActionResult<bool>> ValidarToken(string token)
        {
            var esValido = await _sesionService.ValidarTokenAsync(token);
            return Ok(esValido);
        }

        [HttpPost("{id}/extender")]
        public async Task<ActionResult> ExtenderSesion(int id, [FromBody] ExtenderSesionRequest request)
        {
            var resultado = await _sesionService.ExtenderSesionAsync(id, request.NuevaExpiracion);

            if (!resultado)
                return NotFound();

            return Ok(new { mensaje = "Sesión extendida correctamente" });
        }
    }

    public class ExtenderSesionRequest
    {
        public DateTime NuevaExpiracion { get; set; }
    }
}

